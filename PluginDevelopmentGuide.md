# NestorMSX plugin development guide #

Anyone capable of programming in the .NET platform can develop plugins for NestorMSX. This guide explains how. (It is also strongly recommended to take a look at the source code of the built-in plugins)


## The hello world plugin ##

Let's start with a small tutorial in which we'll build a very simple plugin from scratch. This tutorial assumes that you have Visual Studio and will use C#, but any development environment with support for NuGet and any .NET language are actually fine to develop plugins.

1) Open Visual Studio and create a new solution of type "Class Library". Name it `HelloWorldPluginForNestorMSX`.

2) Add a reference to the `System.Windows.Forms` assembly. (Note: this is not required for all plugins, only for those having its own UI)

3) Install the `NestorMSX.Infrastructure` NuGet package.

4) Add the `HelloWorldPlugin` class with this code:

```
#!c#
using Konamiman.NestorMSX;
using System.Collections.Generic;
using System.Diagnostics;
using Konamiman.NestorMSX.Menus;
using Konamiman.NestorMSX.Misc;
using System.Windows.Forms;
using Konamiman.Z80dotNet;

namespace HelloWorldPluginForNestorMSX
{
    [NestorMSXPlugin("Hello World")]
    public class HelloWorldPlugin
    {
        public HelloWorldPlugin(PluginContext context, IDictionary<string, object> pluginConfig)
        {
            var message = pluginConfig.GetValueOrDefault("message", "Hello world!");
            context.SetMenuEntry(this, new MenuEntry("Say Hello", () => MessageBox.Show(message)));

            context.Cpu.MemoryAccess += (sender, args) =>
            {
                if(args.EventType == MemoryAccessEventType.BeforePortRead && args.Address == 0)
                {
                    Debug.WriteLine("Port 0 read was requested!");
                    args.CancelMemoryAccess = true;
                    args.Value = 34;
                }
            };
        }
    }
}
```

5) Compile the project in debug mode. Copy the resulting .dll and .pdb files to the _plugins_ directory of NestorMSX.

6) Open the `machine.config` file of the _machines/Spanish MSX1 with DiskBASIC_ directory (actually any machine would do) and add the following inside the `plugins` section:

```
#!json
"Hello World": {  }
```

7) Run NestorMSX as the machine where you have configured the plugin (use the menu entry to select the machine or just run this from the command line: `NestorMSX machine="Spanish MSX1 with DiskBASIC"`).

8) Open the _Plugins_ menu. You will see a _Say Hello_ entry, click it and you will be greeted with a "Hello world!" dialog.

9) Go to the BASIC prompt if you aren't already, and execute the following: `print inp(0)`. You should get the value 34.

![Hello world menu entry is available and port 0 output 34](HelloWorldMenuEntry.png)

10) Modify the plugin entry in `machine.config` so that it is as follows:

```
#!json
"Hello World": { "message": "Yadda!" }
```

11) Restart NestorMSX (restart the whole emulator, don't just reset the MSX!). Select the _Say Hello_ menu entry again, and see how the message in the dialog is now "Yadda!".


### Debugging ###

12) Close NestorMSX. Return to Visual Studio and set breakpoints in lines 16 (`var message=...`) and 23 (`Debug.WriteLine...`) of the plugin class.

13) Run NestorMSX from command line as follows: `NestorMSX wd`. See how a new console window opens and NestorMSX doesn't start yet.

14) Select _Debug - Attach to process_ in Visual Studio. Search the NestorMSX process by name or by PID (indicated in the console window).

15) Notice how the first breakpoint is hit. Tell Visual Studio to continue execution.

16) Execute `print inp(0)` from the BASIC prompt. Notice how the second breakpoint is hit.

17) Tell Visual Studio to continue execution. Notice how the "Port 0 read was requested!" message appears in the console.

![Debugging the Hello World plugin](HelloWorldDebugging.png)


## The basics ##

A NestorMSX plugin is a .NET public class decorated with `NestorMsxPluginAttribute`, defined in the _NestorMSX.Infrastructure_ project. This attribute accepts a constructor parameter that will be the friendly name of the plugin.

NestorMSX will search for plugins in all the class library files that exist in its `plugins` directory and also in subdirectories of that directory (so you can cleanly group together the library and its related files, if any).

Besides being decorated with the attribute, the plugin class must either have a constructor with a certain signature, or have a static `GetInstance` method with the same arguments. Thus the minimal plugin can have one of these forms:

```
#!c#
[NestorMSXPlugin("Plugin name")]
public class ThePlugin
{
    public ThePlugin(PluginContext context, IDictionary<string, object> pluginConfig)
    {
    }
}
```

```
#!c#
[NestorMSXPlugin("Plugin name")]
public class ThePlugin
{
    public static ThePlugin GetInstance(PluginContext context, IDictionary<string, object> pluginConfig)
    {
        return AnInstanceOfThePlugin;
    }
}
```

Additionally, plugins intended to be inserted in a slot must have a `GetMemory` method with the following signature:

```
#!c#
public IMemory GetMemory()
{
    return AnInstanceOfAClassImplementingIMemory;
}
```

The returned instance of `IMemory` must be a 64K memory (it can be bigger internally by using a mapper or any other mechanism, but the visible addressing space must be 64K) and will be plugged in the slots system of the emulated machine.

The `PluginContext` class is defined in _NestorMSX.Infrastructure_. The `IMemory` interface comes from the Z80.NET project.


## The plugin context ##

The supplied `PluginContext` instance is what allows the plugin to really do something useful. By using this, the plugin can:

* Subscribe to the various CPU events, such as memory and ports reads and writes. This is the way to emulate a ports-based hardware.
* Access the VDP and the slots system to check (or change) their contents and state.
* Access the window that hosts the emulator. A tipical case for this is to open a new window specifying the emulator window as its parent.
* Receive key press events from the emulator. This is required if your plugin is intended to react to certain keys (the Copy & Paste plugin works this way, for example).
* Access the full list of loaded plugins.
* Set an entry in the emulator "Plugins" menu.

Note that the `PluginContext` class has a `EnvironmentInitializationComplete` event. If your plugin needs to access the loaded plugins list or the slots system, it shouldn't do so until this event is fired. Note also that plugins aren't loaded/initialized in any particular order.

Please take a look at the `PluginContext` class itself for more details.
