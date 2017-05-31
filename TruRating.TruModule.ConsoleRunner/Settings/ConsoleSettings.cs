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
using System.Reflection;
using TruRating.TruModule.ConsoleRunner.Device;
using TruRating.TruModule.ConsoleRunner.Environment;

namespace TruRating.TruModule.ConsoleRunner.Settings
{
    public class ConsoleSettings 
    {
        public ConsoleSettings(IConsoleLogger consoleLogger)
        {
            TruModuleSettings = new TruModuleSettings {TerminalId = System.Environment.MachineName};
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            consoleLogger.WriteLine(ConsoleColor.Yellow, "TruModule version : {0}", Version);
            consoleLogger.WriteLine(ConsoleColor.Yellow,
                " consoleSettings ".PadRight((Console.WindowWidth)/2, '=').PadLeft((Console.WindowWidth) - 1, '='));
            consoleLogger.WriteLine(ConsoleColor.Yellow,
                System.Environment.NewLine +
                "- Override any of these consoleSettings from the configuration file - required consoleSettings will require user input");
            consoleLogger.WriteLine(ConsoleColor.Yellow,
                
                "- You will find your PartnerId, MerchantId and TransportKey under your account in https://developer.trurating.com");
            consoleLogger.WriteLine(ConsoleColor.Yellow,
                
                "- Use any value for TerminalId" +
                System.Environment.NewLine);
            consoleLogger.WriteLine(ConsoleColor.Yellow,
                "- Press 's' anytime after the scenario has started print all consoleSettings." +
                System.Environment.NewLine);
            KeyPressReader.Stop();
            SettingsHelpers.SetProperties(this);
            
            SettingsHelpers.SetProperties(TruModuleSettings);
            KeyPressReader.Start();

            var endpoints = TruModuleSettings.TruServiceUrl.Split(',');
            if (endpoints.Length > 1)
            {
                consoleLogger.WriteLine(ConsoleColor.Red, "Multiple TruService endpoints detected, please choose: ");
                var i = 1;
                foreach (var endpoint in endpoints)
                {
                    consoleLogger.WriteLine(ConsoleColor.Gray, i + ". " + endpoint);
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
                            TruModuleSettings.TruServiceUrl = endpoints[option - 1];
                            consoleLogger.WriteLine(ConsoleColor.Green, "Chosen " + TruModuleSettings.TruServiceUrl);
                            break;
                        }
                    }
                }
            }
            consoleLogger.WriteLine(ConsoleColor.Yellow, "".PadRight(Console.WindowWidth - 1, '='));
            KeyPressReader.OnInputOverride += key => SettingsHelpers.PrintSettings(consoleLogger, key, this); 
        }

       

        public TruModuleSettings TruModuleSettings { get; set; }
        public PosIntegration PosIntegration { get; set; }
        [Required]
        public string[] Languages { get; set; }
        public DateTime LastQuestionDateTime { get; set; }
        public string Version { get; set; }
    }
}