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
using TruRating.TruModule.ConsoleRunner.Device;

namespace TruRating.TruModule.ConsoleRunner.Settings
{
    internal static class SettingsHelpers
    {
        internal static string GetValues<T>()
        {
            var sb = new List<string>();
            var array = Enum.GetValues(typeof(T));
            foreach (T en in array)
            {
                sb.Add(en.ToString());
            }
            return string.Join(", ", sb.ToArray());
        }
        internal static string GetValues(Type type)
        {
            var sb = new List<string>();
            var array = Enum.GetValues(type);
            foreach (var en in array)
            {
                sb.Add(en.ToString());
            }
            return string.Join(", ", sb.ToArray());
        }
        internal static object GetEnumValue(Type type, string value)
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
        internal static T GetEnumValue<T>(string value)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid config file. " + e.Message + " Possible values are " +
                                            GetValues<T>());
            }
        }
        internal static bool PrintSettings(IConsoleLogger logger, char key, object instance)
        {
            if (key == 's' || key == 'S')
            {
                logger.WriteLine(ConsoleColor.Yellow,
                    " consoleSettings ".PadRight((Console.WindowWidth) / 2, '=').PadLeft((Console.WindowWidth) - 1, '='));
                foreach (var prop in instance.GetType().GetProperties())
                {
                    var value = prop.GetValue(instance, null);
                    var hasValue = PropHasValue(prop, value);
                    LogSetting(prop, instance, hasValue);
                }
                logger.WriteLine(ConsoleColor.Yellow, "".PadRight(Console.WindowWidth - 1, '='));
                return true;
            }
            return false;
        }
        internal static bool LogSetting(PropertyInfo propertyInfo, object instance, bool hasValue)
        {
            var required = propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0;
            var value = propertyInfo.GetValue(instance, null);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("{0}{1}:", propertyInfo.Name, "".PadRight(20 - propertyInfo.Name.Length));
            if (required && !hasValue)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                var typedValue = Console.ReadLine();
                if (string.IsNullOrEmpty(typedValue))
                {
                    return false;
                }
                return LoadSetting(propertyInfo,instance, typedValue);
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (IsArrayOf<string>(propertyInfo.PropertyType))
            {
                Console.WriteLine(string.Join(",", ((string[])value)));
            }
            else
            {
                Console.WriteLine(value);
            }
            return true;
        }
        internal static bool IsArrayOf<T>(Type type)
        {
            return type == typeof(T[]);
        }
        internal static bool LoadSetting(PropertyInfo propertyInfo, object instance, string value)
        {
            try
            {
                propertyInfo.SetValue(instance, value, null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal static bool LoadSetting(PropertyInfo propertyInfo, object instance)
        {
            try
            {
                var required = propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0;
                var configuredValue = ConfigurationManager.AppSettings[propertyInfo.Name];
                var value = propertyInfo.GetValue(instance, null);
                if (!string.IsNullOrEmpty(configuredValue))
                {
                    if (IsArrayOf<string>(propertyInfo.PropertyType))
                    {
                        value = configuredValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
                propertyInfo.SetValue(instance, value, null);
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
        internal static bool PropHasValue(PropertyInfo propertyInfo, object value)
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
                return ((string[])value).Length != 0;
            }
            return true;
        }
        internal static void SetProperties(object instance)
        {
            foreach (var prop in instance.GetType().GetProperties())
            {
                var hasValue = LoadSetting(prop, instance);
                while (!LogSetting(prop, instance, hasValue))
                {
                }
            }
        }
    }
}