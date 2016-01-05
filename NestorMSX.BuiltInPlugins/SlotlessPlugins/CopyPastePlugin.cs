using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Konamiman.NestorMSX.Exceptions;
using Konamiman.NestorMSX.Hardware;
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

        private const int ScreenLines = 24;

        private readonly IZ80Processor processor;
        private readonly List<Keys> PressedKeys = new List<Keys>();
        private readonly List<byte> PastedText = new List<byte>();
        private readonly Keys CopyKey;
        private readonly Keys PasteKey;
        private readonly Encoding Encoding;

        public IExternallyControlledTms9918 Vdp { get; set; }

        private CopyPastePlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            var defaultConfig = new Dictionary<string, object>
            {
                { "copyKey", "F11"},
                { "pasteKey", "F12"},
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

            context.KeyEventSource.KeyReleased += KeyEventSourceOnKeyReleased;

            this.Vdp = context.Vdp;
            this.processor = context.Cpu;
            processor.BeforeInstructionFetch += Cpu_BeforeInstructionFetch;
        }

        private Keys ParseKey(string name)
        {
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
            var lineWidth = screenBytes.Length / ScreenLines;
            for (int i = 0; i < ScreenLines; i++)
            {
                var lineBytes =
                    screenBytes
                        .Skip(lineWidth * i)
                        .Take(lineWidth)
                        .ToArray();
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
