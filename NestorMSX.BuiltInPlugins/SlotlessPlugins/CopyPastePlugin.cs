using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Hardware;
using Konamiman.NestorMSX.Menus;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;
using KeyEventArgs = Konamiman.NestorMSX.Hardware.KeyEventArgs;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Copy and Paste")]
    public class CopyPastePlugin
    {
        private static CopyPastePlugin Instance;

        private const int KEYBUF = 0xFB0F;
        private const int GETPNT = 0xF3FA;
        private const int PUTPNT = 0xF3F8;

        private const int LF = 10;

        private readonly IZ80Processor processor;
        private readonly List<Keys> PressedKeys = new List<Keys>();
        private readonly List<byte> PastedText = new List<byte>();
        private readonly Keys CopyKey;
        private readonly Keys PasteKey;
        private readonly Encoding Encoding;
        private int ScreenColumns = 40;
        private MenuEntry CopyMenuEntry;
        private MenuEntry PasteMenuEntry;

        public IExternallyControlledV9938 Vdp { get; set; }

        private CopyPastePlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            var defaultConfig = new Dictionary<string, object>
            {
                { "copyKey", null},
                { "pasteKey", null},
                { "encoding", "ASCII" }
            };

            defaultConfig.MergeInto(pluginConfig);

            CopyKey = ParseKey(pluginConfig.GetValue<string>("copyKey"));
            PasteKey = ParseKey(pluginConfig.GetValue<string>("pasteKey"));

            var encodingName = pluginConfig.GetValue<string>("encoding");

            try
            {
                Encoding = Encoding.GetEncoding(encodingName);
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(
                    $"Encoding '{encodingName}' is not supported by this system", ex);
            }

            if(CopyKey != Keys.None || PasteKey != Keys.None)
                context.KeyEventSource.KeyReleased += KeyEventSourceOnKeyReleased;

            this.Vdp = context.Vdp;
            this.processor = context.Cpu;
            processor.BeforeInstructionFetch += Cpu_BeforeInstructionFetch;

            context.Vdp.ScreenModeChanged += VdpOnScreenModeChanged;

            CopyMenuEntry = new MenuEntry($"Copy{(CopyKey == Keys.None ? "" : " (" + CopyKey + ")")}", CopyScreenAsText) { IsEnabled = false };
            PasteMenuEntry = new MenuEntry($"Paste{(PasteKey == Keys.None ? "" : " (" + PasteKey + ")")}", PasteTextAsKeyboardData) { IsEnabled = false };
            var mainMenuEntry = new MenuEntry("Copy && Paste", new[] {CopyMenuEntry, PasteMenuEntry});
            context.SetMenuEntry(this, mainMenuEntry);
        }

        private void VdpOnScreenModeChanged(object sender, EventArgs eventArgs)
        {
            switch(Vdp.CurrentScreenMode)
            {
                case ScreenMode.Graphic1:
                    ScreenColumns = 32;
                    break;
                case ScreenMode.Text1:
                    ScreenColumns = 40;
                    break;
                case ScreenMode.Text2:
                    ScreenColumns = 80;
                    break;
                default:
                    ScreenColumns = 0;
                    break;
            }

            CopyMenuEntry.IsEnabled = PasteMenuEntry.IsEnabled =
                ScreenColumns != 0;
        }

        private Keys ParseKey(string name)
        {
            if(name == null)
                return Keys.None;

            try
            {
                return (Keys)Enum.Parse(typeof(Keys), name);
            }
            catch(Exception ex)
            {
                throw new ConfigurationException(
                    $"No key is defined with name '{name}'. " +
                    "The key name must be a member of the .NET's System.Windows.Forms enumeration. " +
                    "Run NestorMSX with the \"KeyTest\" argument to check which key names are available."
                    , ex);
            }
        }

        public static CopyPastePlugin GetInstance(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            if (Instance == null)
                Instance = new CopyPastePlugin(context, pluginConfig);

            return Instance;
        }

        private void KeyEventSourceOnKeyReleased(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Value == CopyKey)
                CopyScreenAsText();
            else if (keyEventArgs.Value == PasteKey)
                PasteTextAsKeyboardData();
        }

        private void PasteTextAsKeyboardData()
        {
            var text = Clipboard.GetText();
            PastedText.AddRange(Encoding.GetBytes(text).Where(b => b != LF).ToArray());
        }

        private void CopyScreenAsText()
        {
            var sb = new StringBuilder();
            var screenBytes = Vdp.GetVramContents(Vdp.PatternNameTableAddress, Vdp.PatternNameTableSize);
            var lines = Vdp.PatternNameTableSize/ScreenColumns;
            for (int i = 0; i < lines; i++)
            {
                var lineBytes =
                    screenBytes
                        .Skip(ScreenColumns * i)
                        .Take(ScreenColumns)
                        .ToArray();

                //A 0 will ruin the entire copy operation,
                //so let's get rid of these with a not-very-clean-but-works solution...
                for(int b=0; b<ScreenColumns; b++)
                    if(lineBytes[b] == 0) lineBytes[b] = 1;

                sb.AppendLine(Encoding.GetString(lineBytes));
            }

            Clipboard.SetText(sb.ToString().TrimEnd());
        }

        private void Cpu_BeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs eventArgs)
        {
            var count = Math.Min(PastedText.Count, 8);
            if (count > 0 && (processor.Memory[GETPNT] == processor.Memory[PUTPNT]))
            {
                processor.Memory.SetContents(KEYBUF, PastedText.Take(count).ToArray(), 0, count);
                WriteShort(GETPNT, KEYBUF);
                WriteShort(PUTPNT, KEYBUF + count);
                PastedText.RemoveRange(0, count);
            }
        }

        void WriteShort(ushort address, int value)
        {
            processor.Memory[address] = ((ushort)value).GetLowByte();
            processor.Memory[address + 1] = ((ushort)value).GetHighByte();
        }
    }
}
