using System;
using System.Collections.Generic;
using System.IO;
using Konamiman.NestorMSX.Memories;
using Konamiman.NestorMSX.Misc;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorMSX.Plugins
{
    [NestorMSXPlugin("Special DiskBASIC")]
    public class SpecialDiskBasicPlugin
    {
        private const int BDOS = 0xFB03;    //as defined in dskbasic.mac

        private readonly string fileName;
        private readonly DosFunctionCallExecutor dosFunctionsExecutor;

        public SpecialDiskBasicPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            fileName = pluginConfig.GetMachineFilePath(pluginConfig.GetValueOrDefault<string>("file", "SpecialDiskBasic.rom"));

            context.Cpu.BeforeInstructionFetch += Z80OnBeforeInstructionFetch;
            var filesystemBasePath = pluginConfig.GetValueOrDefault<string>("filesystemBasePath", "$MyDocuments$/NestorMSX/FileSystem").AsAbsolutePath();
            dosFunctionsExecutor = new DosFunctionCallExecutor(context.Cpu.Registers, context.SlotsSystem, filesystemBasePath);
        }

        private void Z80OnBeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs e)
        {
            if (((IZ80Processor)sender).Registers.PC == BDOS)
            {
                dosFunctionsExecutor.ExecuteFunctionCall();
                ((IZ80Processor)sender).ExecuteRet();
            }
        }

        public IMemory GetMemory()
        {
            return new PlainRom(File.ReadAllBytes(fileName), page: 1);
        }
    }
}
