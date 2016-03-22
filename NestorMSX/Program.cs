using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Konamiman.NestorMSX.Emulator;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Misc;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using ConfigurationException = Konamiman.NestorMSX.Exceptions.ConfigurationException;

namespace Konamiman.NestorMSX
{
    public class Program
    {
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();

        private const string BasePathInDevelopmentEnvironment = @"c:\VS\Projects\Z80dotNet\NestorMSX\";
        private static MsxEmulationEnvironment emulationEnvironment;
        private static string machineName;
        private static string stateFilePath;
        private static bool consoleAllocated;

        private static Func<string, string, bool> argIs =
            (s1, s2) => s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);

            /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException  += CurrentDomainOnUnhandledException;

            var waitForDebugger = false;
            var allocateConsole = false;

            stateFilePath = Path.Combine(
                ConfigurationManager.AppSettings["stateFileDirectory"].AsApplicationFilePath(), 
                "NestorMSX.state");

            foreach(var arg in args) {
                if(argIs(arg, "keytest")) {
                    Application.Run(new KeyTestForm());
                    return;
                }
                else if(argIs(arg, "sc")) {
                    allocateConsole = true;
                }
                else if(argIs(arg, "wd")) {
                    allocateConsole = true;
                    waitForDebugger = true;
                }
                else if(arg.StartsWith("machine=", StringComparison.InvariantCultureIgnoreCase)) {
                    machineName = arg.Substring("machine=".Length);
                }
            }

            if(machineName == null)
                machineName = GetMachineName();

            if(machineName == null)
                return;

            SaveMachineNameAsState(machineName);

            if(allocateConsole)
                CreateDebugConsole();

            if(waitForDebugger)
                WaitForDebuggerAttachment();

            if(consoleAllocated) {
                Trace.Listeners.Add(new ConsoleTraceListener());
                Trace.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Starting NestorMSX, machine name: {machineName}\r\n");
                if(!waitForDebugger)
                    Console.WriteLine($"My PID is: {Process.GetCurrentProcess().Id}\r\n");
            }

            emulationEnvironment = CreateEmulationEnvironment(args);
            if(emulationEnvironment == null) {
                FreeConsoleIfAllocated();
                return;
            }

            Application.ApplicationExit += Application_ApplicationExit;

            emulationEnvironment.Run();
        }

        private static void CreateDebugConsole()
        {
            if(consoleAllocated) return;

            consoleAllocated = AllocConsole();
            Console.Title = "NestorMSX - " + machineName;
        }

        private static void WaitForDebuggerAttachment()
        {
            Console.WriteLine("Waiting for a debugger to attach (or press any key to skip)");
            Console.WriteLine($"My PID is: {Process.GetCurrentProcess().Id}");

            while (!Debugger.IsAttached)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break;
                }
                Thread.Sleep(100);
            }

            Console.WriteLine("Ok, let's go!\r\n");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var ex = (Exception)unhandledExceptionEventArgs.ExceptionObject;
            Tell($"Unexpected appdomain exception: ({ex.GetType().Name}) {ex.Message}\r\n{ex.StackTrace}");
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            Tell($"Unexpected thread exception: ({ex.GetType().Name}) {ex.Message}\r\n{ex.StackTrace}");
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if(consoleAllocated)
                Trace.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] NestorMSX is closing\r\n");

            FreeConsoleIfAllocated();
            emulationEnvironment.DisposePlugins();
        }

        private static void FreeConsoleIfAllocated()
        {
            if(consoleAllocated) FreeConsole();
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

            if (File.Exists(stateFilePath)) {
                try {
                    var previousMachineName = File.ReadAllText(stateFilePath);
                    if (availableMachineNames.Contains(previousMachineName))
                        return previousMachineName;
                }
                catch (Exception ex) {
                    Tell($"Could not read state file: {ex.Message}\r\n\r\nPlease review the stateFileDirectory setting in NestorMSX.exe.config");
                }
            }

            return ShowMachineSelectionDialog(availableMachineNames);
        }

        public static void SaveMachineNameAsState(string machineName)
        {
            try {
                File.WriteAllText(stateFilePath, machineName);
            }
            catch (Exception ex) {
                Tell($"Could not create state file: {ex.Message}\r\n\r\nPlease review the stateFileDirectory setting in NestorMSX.exe.config");
            }
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
