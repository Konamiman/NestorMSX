# NestorMSX #


## What is this? ##

NestorMSX is an extremely simple, incomplete, underperforming MSX emulator. Its main purpose is to serve as an example of an emulation environment built around [the Z80.NET package](https://bitbucket.org/konamiman/z80dotnet). It is written in C# and targets the .NET Framework 4 Client Profile.


## What does it emulate? ##

The emulated machine is:

- A first generation MSX computer
- With a text-only display (only SCREEN 0 and SCREEN1, without sprites, are emulated)
- With four primary slots
- With a keyboard
- ...and that's it: no sound, cassette, joystick, printer or any other hardware or peripheral is emulated.

The slots configuration is:

- Slot 0 contains the MSX BIOS and BASIC interpreter
- Slot 1 contains a special DiskBASIC ROM (more on that below)
- Slot 2 is empty by default, but you can attach a ROM file (more on that below)
- Slot 3 contains 64K RAM


## How to use ##

1. Review the `NestorMSX.config` file and tweak it if appropriate. Each configuration key is appropriately (I hope!) explained.
2. You may want to modify the key mappings file, `KeyMappings.txt`. If so, running NestorMSX in key test mode will be useful for you (more on that below).
3. Run `NestorMSX.exe`, optionally passing the appropriate arguments (more on that below).


## NestorMSX execution arguments ##

You can run `NestorMSX.exe` as follows:

#### NestorMSX ####

Start the emulator using `NestorMSX.config` for configuration and leaving slot 2 empty.

#### NestorMSX keytest ####

Run the key test mode. A window will open in which you can see the names of the keys that you press. This is very useful if you want to create your own key mappings file.

#### NestorMSX config=<filename> ####

Start the emulator using the specified configuration file and leaving slot 2 empty.

#### NestorMSX slot2=<filename> ####

Start the emulator using `NestorMSX.config` for configuration and inserting the specified filename as a ROM in slot 2. It overrides the file specified in the configuration file, if any. The maximum file size is 48KBytes.

#### NestorMSX config=<filename> slot2=<filename> ####

A combination of the two above.


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


## Future plans ##

- Emulate MSX2 (but again, text modes only)
- Develop a plugins system


## Last but not least...

...if you like this project **[please consider donating!](http://www.konamiman.com#donate)** My kids need moar shoes!

## But who am I? ##

I'm [Konamiman, the MSX freak](http://www.konamiman.com). No more, no less.
