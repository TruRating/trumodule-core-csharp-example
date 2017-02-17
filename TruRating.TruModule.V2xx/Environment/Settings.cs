// The MIT License
// 
// Copyright (c) 2017 TruRating Ltd. https://www.trurating.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace TruRating.TruModule.V2xx.Environment
{
    public interface ISettings
    {
        PosIntegration PosIntegration { get; set; }
        string PartnerId { get; set; }
        string MerchantId { get; set; }
        string Endpoint { get; set; }
        string[] Languages { get; set; }
        DateTime LastQuestionDateTime { get; set; }
        bool Verbose { get; set; }
        string TerminalId { get; set; }
        string Version { get; set; }
        bool Automatic { get; set; }
        string TransportKey { get; set; }
        bool RegistrationCode { get; set; }
        DateTime ActivationRecheck { get; set; }
        bool IsActivated { get; set; }
        TsiVersion TsiVersion { get; set; }
    }

    public class Settings : ISettings
    {
        private readonly ILogger _logger;

        public Settings(ILogger logger)
        {
            _logger = logger;
            TerminalId = System.Environment.MachineName;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            logger.Write(ConsoleColor.Yellow, "TruModule version : {0}", Version);
            logger.Write(ConsoleColor.Yellow,
                " Settings ".PadRight((Console.WindowWidth)/2, '=').PadLeft((Console.WindowWidth) - 1, '='));
            logger.Write(ConsoleColor.Yellow,
                System.Environment.NewLine + "Override any of these settings from the configuration file");
            logger.Write(ConsoleColor.Yellow,
                "Press 's' anytime after the scenario has started print all settings." + System.Environment.NewLine);
            var errors = new List<string>();
            foreach (var prop in GetType().GetProperties())
            {
                var hasValue = LoadSetting(prop);
                if (!hasValue)
                {
                    errors.Add(prop.Name);
                }
                LogSetting(prop, hasValue);
            }
            if (errors.Count > 0)
            {
                throw new Exception("Please edit the config file and add your " + string.Join(", ", errors.ToArray()));
            }
            var endpoints = Endpoint.Split(',');
            if (endpoints.Length > 1)
            {
                logger.Write(ConsoleColor.Red, "Multiple TruService endpoints detected, please choose: ");
                var i = 1;
                foreach (var endpoint in endpoints)
                {
                    logger.Write(ConsoleColor.Gray, i + ". " + endpoint);
                    i++;
                }
                while (true)
                {
                    var key = KeyPressReader.ReadKey();
                    int option;
                    if (int.TryParse(key.KeyChar.ToString(), out option))
                    {
                        if (option >= 1 && option - 1 < endpoints.Length)
                        {
                            Endpoint = endpoints[option - 1];
                            logger.Write(ConsoleColor.Green, "Chosen " + Endpoint);
                            break;
                        }
                    }
                }
            }
            logger.Write(ConsoleColor.Yellow, "".PadRight(Console.WindowWidth - 1, '='));
            KeyPressReader.OnInputOverride += PrintSettings;
        }

        public PosIntegration PosIntegration { get; set; }

        [Required]
        public string PartnerId { get; set; }

        [Required]
        public string MerchantId { get; set; }

        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string[] Languages { get; set; }

        public DateTime LastQuestionDateTime { get; set; }
        public bool Verbose { get; set; }

        [Required]
        public string TerminalId { get; set; }

        public string Version { get; set; }
        public bool Automatic { get; set; }

        [Required]
        public string TransportKey { get; set; }

        public bool RegistrationCode { get; set; }
        public DateTime ActivationRecheck { get; set; }
        public bool IsActivated { get; set; }

        [Required]
        public TsiVersion TsiVersion { get; set; }

        public static string GetValues<T>()
        {
            var sb = new List<string>();
            var array = Enum.GetValues(typeof (T));
            foreach (T en in array)
            {
                sb.Add(en.ToString());
            }
            return string.Join(", ", sb.ToArray());
        }

        public static string GetValues(Type type)
        {
            var sb = new List<string>();
            var array = Enum.GetValues(type);
            foreach (var en in array)
            {
                sb.Add(en.ToString());
            }
            return string.Join(", ", sb.ToArray());
        }

        public object GetEnumValue(Type type, string value)
        {
            try
            {
                return Enum.Parse(type, value);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid config file. " + e.Message + " Possible values are " +
                                            GetValues(type));
            }
        }

        public T GetEnumValue<T>(string value)
        {
            try
            {
                return (T) Enum.Parse(typeof (T), value);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid config file. " + e.Message + " Possible values are " +
                                            GetValues<T>());
            }
        }

        private bool PrintSettings(char key)
        {
            if (key == 's')
            {
                _logger.Write(ConsoleColor.Yellow,
                    " Settings ".PadRight((Console.WindowWidth)/2, '=').PadLeft((Console.WindowWidth) - 1, '='));
                foreach (var prop in GetType().GetProperties())
                {
                    var value = prop.GetValue(this, null);
                    var hasValue = PropHasValue(prop, value);
                    LogSetting(prop, hasValue);
                }
                _logger.Write(ConsoleColor.Yellow, "".PadRight(Console.WindowWidth - 1, '='));
                return true;
            }
            return false;
        }


        private void LogSetting(PropertyInfo propertyInfo, bool hasValue)
        {
            var required = propertyInfo.GetCustomAttributes(typeof (RequiredAttribute), true).Length > 0;
            var value = propertyInfo.GetValue(this, null);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("{0}{1}:", propertyInfo.Name, "".PadRight(20 - propertyInfo.Name.Length));
            if (required && !hasValue)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("MISSING");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                if (IsArrayOf<string>(propertyInfo.PropertyType))
                {
                    Console.WriteLine(string.Join(",", ((string[]) value)));
                }
                else
                {
                    Console.WriteLine(value);
                }
            }
        }

        public static bool IsArrayOf<T>(Type type)
        {
            return type == typeof (T[]);
        }

        private bool LoadSetting(PropertyInfo propertyInfo)
        {
            try
            {
                var required = propertyInfo.GetCustomAttributes(typeof (RequiredAttribute), true).Length > 0;
                var configuredValue = ConfigurationManager.AppSettings[propertyInfo.Name];
                var value = propertyInfo.GetValue(this, null);
                if (!string.IsNullOrEmpty(configuredValue))
                {
                    if (IsArrayOf<string>(propertyInfo.PropertyType))
                    {
                        value = configuredValue.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else if (propertyInfo.PropertyType.IsEnum)
                    {
                        value = GetEnumValue(propertyInfo.PropertyType, configuredValue);
                    }
                    else
                    {
                        value = Convert.ChangeType(configuredValue, propertyInfo.PropertyType);
                    }
                }
                propertyInfo.SetValue(this, value, null);
                if (required)
                {
                    return PropHasValue(propertyInfo, value);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool PropHasValue(PropertyInfo propertyInfo, object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is string && string.IsNullOrEmpty(value as string))
            {
                return false;
            }
            if (IsArrayOf<string>(propertyInfo.PropertyType))
            {
                return ((string[]) value).Length != 0;
            }
            return true;
        }
    }
}