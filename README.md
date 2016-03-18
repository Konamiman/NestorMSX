# NestorMSX #


## What is this? ##

NestorMSX is a very simple, incomplete (but extensible via plugins), underperforming MSX emulator. It was originally developed with the purpose of serving as an example of an emulation environment built around [the Z80.NET package](https://bitbucket.org/konamiman/z80dotnet). It is written in C# and targets the .NET Framework 4 Client Profile.


## What does it emulate? ##

The base emulated machine is a MSX2 computer with a keyboard and a text-only display (only SCREEN 0 and SCREEN1, without sprites, are emulated). And that's it: there is no emulation for sound, cassette, joystick, printer or any other hardware.

However, NestorMSX comes with a plugin system that allows to extend it to emulate additional hardware or to provide some nice extras (see the copy & paste plugin for example). NestorMSX comes with the following built-in plugins:

- Clock IC chip (date and time are took from the host machine's clock)
- Copy & paste from/to the host machine clipboard (text only)
- Plain RAM (size from 1 byte to 64K)
- Plain ROM (size from 1 byte to 64K)
- Standadr mapped RAM
- MegaROM mappers: ASCII8, ASCII16 and MSX-DOS2

Notice how the different memory types are implemented via plugins. Actually, the core NestorMSX emulated components are just the VDP, the keyboard and the slots system; everything else is handled via plugins.


## How to use ##

1. Review the `NestorMSX.config` file and tweak it if appropriate. Each configuration key is appropriately (I hope!) explained.
2. Take a look at the `plugins` directory. Each subdirectory here contains a `machine.config` file that holds a machine definition. The file for the _MSX2 with Nextor_ machine is fully annotated, take a look at it to understand how to create new machines or modify existing ones.
3. Run `NestorMSX.exe`. The first time you will be asked to choose a machine. You can run a different machine via the appropriate menu option.

**NOTE:** Since only text modes are emulated, you will not see the MSX logo at startup. Instead, you will go directly to the BASIC/DOS/Nextor prompt after a few seconds.


## Built-in plugins##

### Copy & Paste ###

* _Friendly name_: "Copy and Paste"
* _Full class name_: "Konamiman.NestorMSX.Plugins.CopyPastePlugin"
* _Configuration keys_:
    * "copyKey": The key used for copy (optional)
    * "pasteKey": The key used for paste (optional)
    * "encoding": The encoding used to convert from bytes to text and viceversa (optional, default: "ASCII")

This plugin only works in text modes. It copies the MSX screen context to the clipboard (copy) and the other way around (paste):

- Copy just converts the contents of the pattern name table in VRAM to text and inserts it in the clipboard, adding one line break at the end of each physical line (each 40 or 80 characters in SCREEN 0, 32 in SCREEN 1).
- Paste converts the clipboard text contents into bytes and inserts these in the keyboard buffer, so the emulated system acts as if the user had effectively typed the text.

The possible values for "copyKey" and "pasteKey" are the members of the [.NET's Keys enumeration](https://msdn.microsoft.com/en-us/library/system.windows.forms.keys). If no keys are provided, copy & paste is only available via menu entry.


### Clock IC ###

* _Friendly name_: "Clock IC"
* _Full class name_: "Konamiman.NestorMSX.Plugins.ClockIcPlugin"
* _Configuration keys_:
    * "useDataFile": Tells the plugin whether to persist clock IC data or not (optional, default: true)
    * "dataFileName": The name of the file to use to persist data (optional, default: "clock-ic.dat")
    * "useSingleDataFileForAllMachines": If true, one single data file will be used to persist data for all machines (optional, default: false)
   
This plugins emulates the MSX Clock IC that maintains the current date and time and stores certain screen configuration parameters. Note that the date and time are always in sync with the host and cannot be changed; the persistence settings affect the screen configuration only. 

Note that a MSX2 machine will not boot if this plugin is not active.


### Plain RAM ###

* _Friendly name_: "RAM"
* _Full class name_: "Konamiman.NestorMSX.Plugins.PlainRamPlugin"
* _Configuration keys_:
    * "baseAddress": The address where the memory will start (optional)
    * "size": The size of the memory (optional)

This plugin is intended to be inserted in a slot and emulates a plain, non-mapped RAM with any size from 1 byte to 64K bytes. You have three options for the configuration:

1. Specify only base address: the size will then be 64K - base address.
2. Specify only size: the base address will then be 64K - size.
3. Don't specify either: it will be a 64K memory.


### Plain ROM ###

* _Friendly name_: "ROM"
* _Full class name_: "Konamiman.NestorMSX.Plugins.PlainRomPlugin"
* _Configuration keys_:
    * "page": The Z80 page where the memory will start (optional, default: 0)
    * "file": The name of the file with the ROM contents

This plugin is intended to be inserted in a slot and emulates a plain, non-mapped ROM with any size from 1 byte to 64K bytes.




WIP...

## Host filesystem integration ##

NestorMSX comes with a special DiskBASIC ROM that is connected to slot 1. It allows to access the host filesystem directly from BASIC, bypassing the 8.3 filename format limitation along the way. So you can for example execute `SAVE "C:\users\konamiman\My Documents\My amazing program.bas"` from inside the emulated BASIC interpreter; go to that folder in the host system and the file will be there!

You can load and save files (LOAD/SAVE, BLOAD/BSAVE) and use BASIC commands for accessing file contents (OPEN/CLOSE, GET/PUT, etc). However the filesystem management commands (DSKF, FILES, NAME, KILL, COPY) and the low level disk access commands (DSKI$, DSKO$) are not available.

Oh, and you can also do `SAVE"$MyDocuments$\My amazing program.bas"`. See the explanation in `NestorMSX.config` for more details.




## Known problems ##

- Poor performance
- Inexact timing
- Uber ugly application icon
- The "Reset" menu entry often does not work


## Future plans ##

- Partial emulation of graphic modes (to run MultiMente and similar tools)
- Develop a state-of-the-art debugger


## Last but not least...

...if you like this project **[please consider donating!](http://www.konamiman.com#donate)** My kids need moar shoes!

## But who am I? ##

I'm [Konamiman, the MSX freak](http://www.konamiman.com). No more, no less.