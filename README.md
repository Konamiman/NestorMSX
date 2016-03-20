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
- Standard mapped RAM
- MegaROM mappers: ASCII8, ASCII16 and MSX-DOS2

Notice how the different memory types are implemented via plugins. Actually, the core NestorMSX emulated components are just the VDP, the keyboard and the slots system; everything else is handled via plugins.


## How to use ##

1. Review the `NestorMSX.config` file and tweak it if appropriate. Each configuration key is appropriately (I hope!) explained.
2. Take a look at the `plugins` directory. Each subdirectory here contains a `machine.config` file that holds a machine definition. The file for the _MSX2 with Nextor_ machine is fully annotated, take a look at it to understand how to create new machines or modify existing ones.
3. Run `NestorMSX.exe`. The first time you will be asked to choose a machine. You can run a different machine via the appropriate menu option.

**NOTE:** Since only text modes are emulated, you will not see the MSX logo at startup. Instead, you will go directly to the BASIC/DOS/Nextor prompt after a few seconds.


## Built-in plugins##

You can get a grasp how how the built-in plugins work by looking at the `machine.config` files, but here is the reference of all the available configuration options for them.

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


### Mapped RAM ###

* _Friendly name_: "Mapped RAM"
* _Full class name_: "Konamiman.NestorMSX.Plugins.MappedRamPlugin"
* _Configuration keys_:
    * "sizeInKb": The size of the memory in K, must be one of: 64, 128, 256, 512, 1024, 2048, 4096 (optional, default: 3096)

This plugin is intended to be inserted in a slot and emulates a standard mapped RAM memory with any size between 64K and 4096K.


### Ascii8 and Ascii16 ROMs ###

* _Friendly name_: "Ascii8", "Ascii16"
* _Full class name_: "Konamiman.NestorMSX.Plugins.Ascii8RomPlugin", "Konamiman.NestorMSX.Plugins.Ascii16RomPlugin" 
* _Configuration keys_:
    * "file": The name of the file with the ROM contents

These plugins are intended to be inserted in a slot and emulate a MegaROM cartridge with either the Ascii8 or the Ascii16 mapper, depending on the plugin chosen.


### MSX-DOS 1 ###

* _Friendly name_: "MSX-DOS"
* _Full class name_: "Konamiman.NestorMSX.Plugins.MsxDosPlugin"
* _Configuration keys_:
    * "kernelFile": The name of the file with the MSX-DOS 1 kernel (optional, default: "MsxDosKernel.rom")
	* "numberOfDrives": The number of emulated DOS drives (optional, default: 2)
	* "diskImageFiles": An array with the names of the disk image files that will be visible in the drives (optional, default: no files)
	* "diskImagesDirectory": The directory where disk image files with relative name will be searched (optional, default: ""). The usual relative directory resolution rules apply.
	* "addressOfCallInihrd": The offset within the kernel file where the CALL INIHRD instruction is (optional, default: 0x176F)
	* "addressOfCallDrives": The offset within the kernel file where the CALL DRIVES instruction is (optional, default: 0x1850)

This plugin emulates a floppy disk drive or any other MSX-DOS based storage controller. It uses disk image files to emulate storage devices.

Note that this is for MSX-DOS 1 only. If you want MSX-DOS 2, plug also the "Deviceless MSX-DOS 2" plugin in other slot. 

Note that disk formatting is not supported. The disk image files should hold a proper FAT filesystem already.


### MSX-DOS 2 ###

* _Friendly name_: "Deviceless MSX-DOS 2"
* _Full class name_: "Konamiman.NestorMSX.Plugins.DevicelessMsxDos2RomPlugin"
* _Configuration keys_:
    * "kernelFile": The name of the file with the MSX-DOS 2 kernel (optional, default: "MsxDos2Kernel.rom")

This plugin emulates a standalone MSX-DOS 2 kernel. You can't attach emulated floppy disks or other storage devices to this plugin; use the MSX-DOS 1 plugin in another slot for that.

The kernel file should have the standard MSX-DOS 2 mapper.


### Nextor ###

* _Friendly name_: "Nextor"
* _Full class name_: "Konamiman.NestorMSX.Plugins.NextorPlugin"
* _Configuration keys_:
    * "kernelFile": The name of the file with the Nextor kernel, must be an Ascii8 ROM (optional, default: "NextorKernel.rom")
	* "diskImageFiles": An array with up to 7 names for the disk image files that will be visible as connected storage devices (optional, default: no files)
	* "diskImagesDirectory": The directory where disk image files with relative name will be searched (optional, default: ""). The usual relative directory resolution rules apply.

This plugin emulates a Nextor kernel with up to 7 storage devices connected. It uses disk image files to emulate these devices.

The disk image files don't need to be partitioned or formatted upfront, you will be able to partition them by using the FDISK tool that Nextor has built-in (`CALL FDISK` from the BASIC prompt)

No particular storage controller is emulated; instead, the driver entry points are patched so that the emulator accesses the disk image files directly. Therefore, you can use any kernel file, as long as it is intended to work with an Ascii8 mapper. The standalone Ascii8 kernel will work fine.

More information about Nextor is available at [Konamiman's MSX page](http://www.konamiman.com/msx/msx-e.html#nextor).


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