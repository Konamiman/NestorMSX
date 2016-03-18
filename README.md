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


## Built-in plugins##

### Copy & Paste ###

* _Friendly name_: "Copy and Paste"
* _Full class name_: "Konamiman.NestorMSX.Plugins.CopyPastePlugin"
* _Configuration keys_:
   * "copyKey": The key used for copy (optional)
   * "pasteKey": The key used for paste (optional)
   * "encoding": The encoding used to convert from bytes to text and viceversa (optional, default: "ASCII")

WIP...


## Host filesystem integration ##

NestorMSX comes with a special DiskBASIC ROM that is connected to slot 1. It allows to access the host filesystem directly from BASIC, bypassing the 8.3 filename format limitation along the way. So you can for example execute `SAVE "C:\users\konamiman\My Documents\My amazing program.bas"` from inside the emulated BASIC interpreter; go to that folder in the host system and the file will be there!

You can load and save files (LOAD/SAVE, BLOAD/BSAVE) and use BASIC commands for accessing file contents (OPEN/CLOSE, GET/PUT, etc). However the filesystem management commands (DSKF, FILES, NAME, KILL, COPY) and the low level disk access commands (DSKI$, DSKO$) are not available.

Oh, and you can also do `SAVE"$MyDocuments$\My amazing program.bas"`. See the explanation in `NestorMSX.config` for more details.


## Copy & Paste ##

NestorMSX has support for Copy & Paste operations. It works the following way:

- Copy just converts the contents of the pattern name table in VRAM to text and inserts it in the clipboard, adding one line break at the end of each physical line (each 40 characters in SCREEN 0, or 32 in SCREEN 1).

- Paste converts the clipboard text contents into bytes and inserts these in the keyboard buffer, so the emulated system acts as if the user had effectively typed the text.

The default keys are `F11` for Copy and `F12` for Paste, but you can change this in the configuration file. You can also configure the text encoding that will be used for converting between text and bytes.


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