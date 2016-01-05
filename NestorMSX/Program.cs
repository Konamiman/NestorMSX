using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static string machineName;
        private static string stateFilePath;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();

            stateFilePath = "NestorMSX.state".AsApplicationFilePath();

            if (args.Length > 0 && args[0].ToLower() == "keytest") {
                Application.Run(new KeyTestForm());
                return;
            }

            if(args.Length > 0)
                machineName = args[0].Trim();
            else
                machineName = GetMachineName();

            if(machineName == null)
                return;

            SaveMachineNameAsState(machineName);

            emulationEnvironment = CreateEmulationEnvironment(args);
            if(emulationEnvironment == null)
                return;

            Application.ApplicationExit += Application_ApplicationExit;

            emulationEnvironment.Run();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
           emulationEnvironment.DisposePlugins();
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
            var configFileName = "NestorMSX.config".AsApplicationFilePath();

            ParseArgs(args, ref configFileName);

            var configDictionary = ReadConfig(configFileName);

            return new MsxEmulationEnvironment(configDictionary, Tell, machineName);
        }

        private void ParseArgs(string[] args, ref string configFileName)
        {
            var configKey = "config=";

            foreach(var arg in args) {
                if(arg.StartsWith(configKey, StringComparison.InvariantCultureIgnoreCase))
                    configFileName = arg.Substring(configKey.Length).AsApplicationFilePath();
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

        private static string GetMachineName()
        {
            var availableMachineNames = GetAvailableMachineNames();

            if(File.Exists(stateFilePath)) {
                var previousMachineName = File.ReadAllText(stateFilePath);
                if(availableMachineNames.Contains(previousMachineName))
                    return previousMachineName;
            }

            return ShowMachineSelectionDialog(availableMachineNames);
        }

        public static void SaveMachineNameAsState(string machineName)
        {
            File.WriteAllText(stateFilePath, machineName);
        }

        public static string ShowMachineSelectionDialog(string[] availableMachineNames = null)
        {
            var form = new MachineSelectionForm();
            form.SetMachinesListItems(availableMachineNames ?? GetAvailableMachineNames());
            var dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
                return form.SelectedMachine;
            else
                return null;
        }

        private static string[] GetAvailableMachineNames()
        {
            var machinesDirectory = "machines".AsApplicationFilePath();
            var folders = Directory
                .GetDirectories(machinesDirectory)
                .Where(d => File.Exists(Path.Combine(d, "machine.config")))
                .Select(d => Path.GetFileName(d));

            return folders.ToArray();
        }
    }
}
