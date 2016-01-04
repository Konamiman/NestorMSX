using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using Konamiman.NestorMSX.Emulator;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Misc;

namespace Konamiman.NestorMSX
{
    public class Program
    {
        private const string BasePathInDevelopmentEnvironment = @"c:\VS\Projects\Z80dotNet\NestorMSX\";
        private static Configuration config;
        private static MsxEmulationEnvironment emulationEnvironment;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();

            if (args.Length > 0 && args[0].ToLower() == "keytest") {
                Application.Run(new KeyTestForm());
                return;
            }

            emulationEnvironment = CreateEmulationEnvironment(args);
            if(emulationEnvironment == null)
                return;

            Application.ApplicationExit += Application_ApplicationExit;

            emulationEnvironment.Run();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            var disposablePlugins =
                emulationEnvironment.GetLoadedPlugins().Where(p => p is IDisposable).Cast<IDisposable>();

            foreach(var plugin in disposablePlugins)
                try
                {
                    plugin.Dispose();
                }
                catch
                {
                    
                }
        }

        private static MsxEmulationEnvironment CreateEmulationEnvironment(string[] args)
        {
            MsxEmulationEnvironment environment = null;
            try {
                environment = (new Program()).CreateEmulationEnvironmentCore(args);
            }
            catch(ConfigurationException ex) {
                Tell("Sorry, something went wrong with the configuration file:\r\n\r\n{0}", ex.Message);
            }
            catch(EmulationEnvironmentCreationException ex) {
                Tell("Sorry, I couldn't create the emulation environment:\r\n\r\n{0}", ex.Message);
            }
            catch(Exception ex) {
                Tell("Ooops, something went wrong and I am not sure why... here are the ugly details:\r\n\r\n{0}",
                    ex.ToString().Replace(BasePathInDevelopmentEnvironment, "", StringComparison.InvariantCultureIgnoreCase));
            }
            return environment;
        }

        private MsxEmulationEnvironment CreateEmulationEnvironmentCore(string[] args)
        {
            var configFileName = "NestorMSX.config".AsAbsolutePath();

            ParseArgs(args, ref configFileName);

            var configDictionary = ReadConfig(configFileName);

            return new MsxEmulationEnvironment(configDictionary, Tell);
        }

        private void ParseArgs(string[] args, ref string configFileName)
        {
            var configKey = "config=";

            foreach(var arg in args) {
                if(arg.StartsWith(configKey, StringComparison.InvariantCultureIgnoreCase))
                    configFileName = arg.Substring(configKey.Length).AsAbsolutePath();
            }
        }

        private static IDictionary<string, object> ReadConfig(string configFileName)
        {
            try {
                var configFileContents = File.ReadAllText(configFileName);
                var configDictionary = (IDictionary<string, object>)JsonParser.Parse(configFileContents);
                return configDictionary;
            }
            catch(FileNotFoundException ex) {
                throw new ConfigurationException(
                    $"Configuration file not found. It is supposed to be here:\r\n{configFileName}", ex);
            }
            catch(Exception ex) {
                throw new ConfigurationException(
                    $"Error when parsing configuration file: ({ex.GetType().Name}) {ex.Message}", ex);
            }
        }

        public static void Tell(string message, params object[] parameters)
        {
            MessageBox.Show(message.FormatWith(parameters), "NestorMSX");
        }
    }
}
