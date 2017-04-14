using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public class DosErrors
    {
        /// <summary>
        /// Incompatible disk
        /// </summary>
        public static readonly byte NCOMP = 0xFF;

        /// <summary>
        /// Write error
        /// </summary>
        public static readonly byte WRERR = 0xFE;

        /// <summary>
        /// Disk error
        /// </summary>
        public static readonly byte DISK = 0xFD;

        /// <summary>
        /// Not ready
        /// </summary>
        public static readonly byte NRDY = 0xFC;

        /// <summary>
        /// Verify error
        /// </summary>
        public static readonly byte VERFY = 0xFB;

        /// <summary>
        /// Data error
        /// </summary>
        public static readonly byte DATA = 0xFA;

        /// <summary>
        /// Sector not found
        /// </summary>
        public static readonly byte RNF = 0xF9;

        /// <summary>
        /// Write protected disk
        /// </summary>
        public static readonly byte WPROT = 0xF8;

        /// <summary>
        /// Unformatted disk
        /// </summary>
        public static readonly byte UFORM = 0xF7;

        /// <summary>
        /// Not a DOS disk
        /// </summary>
        public static readonly byte NDOS = 0xF6;

        /// <summary>
        /// Wrong disk
        /// </summary>
        public static readonly byte WDISK = 0xF5;

        /// <summary>
        /// Wrong disk for file
        /// </summary>
        public static readonly byte WFILE = 0xF4;

        /// <summary>
        /// Seek error
        /// </summary>
        public static readonly byte SEEK = 0xF3;

        /// <summary>
        /// Bad file allocation table
        /// </summary>
        public static readonly byte IFAT = 0xF2;

        /// <summary>
        /// (no description)
        /// </summary>
        public static readonly byte NOUPB = 0xF1;

        /// <summary>
        /// Cannot format this drive
        /// </summary>
        public static readonly byte IFORM = 0xF0;

        /// <summary>
        /// Internal error
        /// </summary>
        public static readonly byte INTER = 0xDF;

        /// <summary>
        /// Not enough memory
        /// </summary>
        public static readonly byte NORAM = 0xDE;

        /// <summary>
        /// Invalid MSX-DOS call
        /// </summary>
        public static readonly byte IBDOS = 0xDC;

        /// <summary>
        /// Invalid drive
        /// </summary>
        public static readonly byte IDRV = 0xDB;

        /// <summary>
        /// Invalid filename
        /// </summary>
        public static readonly byte IFNM = 0xDA;

        /// <summary>
        /// Invalid pathname
        /// </summary>
        public static readonly byte IPATH = 0xD9;

        /// <summary>
        /// Pathname too long
        /// </summary>
        public static readonly byte PLONG = 0xD8;

        /// <summary>
        /// File not found
        /// </summary>
        public static readonly byte NOFIL = 0xD7;

        /// <summary>
        /// Directory not found
        /// </summary>
        public static readonly byte NODIR = 0xD6;

        /// <summary>
        /// Root directory full
        /// </summary>
        public static readonly byte DRFUL = 0xD5;

        /// <summary>
        /// Disk full
        /// </summary>
        public static readonly byte DKFUL = 0xD4;

        /// <summary>
        /// Duplicate filename
        /// </summary>
        public static readonly byte DUPF = 0xD3;

        /// <summary>
        /// Invalid directory move
        /// </summary>
        public static readonly byte DIRE = 0xD2;

        /// <summary>
        /// Read only file
        /// </summary>
        public static readonly byte FILRO = 0xD1;

        /// <summary>
        /// Directory not empty
        /// </summary>
        public static readonly byte DIRNE = 0xD0;

        /// <summary>
        /// Invalid attributes
        /// </summary>
        public static readonly byte IATTR = 0xCF;

        /// <summary>
        /// Invalid . or .. operation
        /// </summary>
        public static readonly byte DOT = 0xCE;

        /// <summary>
        /// System file exists
        /// </summary>
        public static readonly byte SYSX = 0xCD;

        /// <summary>
        /// Directory exists
        /// </summary>
        public static readonly byte DIRX = 0xCC;

        /// <summary>
        /// File exists
        /// </summary>
        public static readonly byte FILEX = 0xCB;

        /// <summary>
        /// File already in use
        /// </summary>
        public static readonly byte FOPEN = 0xCA;

        /// <summary>
        /// File allocation error
        /// </summary>
        public static readonly byte FILE = 0xC8;

        /// <summary>
        /// End of file
        /// </summary>
        public static readonly byte EOF = 0xC7;

        /// <summary>
        /// File access violation
        /// </summary>
        public static readonly byte ACCV = 0xC6;

        /// <summary>
        /// Invalid process id
        /// </summary>
        public static readonly byte IPROC = 0xC5;

        /// <summary>
        /// No spare file handles
        /// </summary>
        public static readonly byte NHAND = 0xC4;

        /// <summary>
        /// Invalid file handle
        /// </summary>
        public static readonly byte IHAND = 0xC3;

        /// <summary>
        /// File handle not open
        /// </summary>
        public static readonly byte NOPEN = 0xC2;

        /// <summary>
        /// Invalid device operation
        /// </summary>
        public static readonly byte IDEV = 0xC1;

        /// <summary>
        /// Invalid environment string
        /// </summary>
        public static readonly byte IENV = 0xC0;

        /// <summary>
        /// Environment string too long
        /// </summary>
        public static readonly byte ELONG = 0xBF;

        /// <summary>
        /// Invalid date
        /// </summary>
        public static readonly byte IDATE = 0xBE;

        /// <summary>
        /// Invalid time
        /// </summary>
        public static readonly byte ITIME = 0xBD;

        /// <summary>
        /// RAM disk (drive H:) already exists
        /// </summary>
        public static readonly byte RAMDX = 0xBC;

        /// <summary>
        /// RAM disk does not exist
        /// </summary>
        public static readonly byte NRAMD = 0xBB;

        /// <summary>
        /// File handle has been deleted
        /// </summary>
        public static readonly byte HDEAD = 0xBA;

        /// <summary>
        /// (no description)
        /// </summary>
        public static readonly byte EOL = 0xB9;

        /// <summary>
        /// Invalid sub-function number
        /// </summary>
        public static readonly byte ISBFN = 0xB8;

        /// <summary>
        /// Ctrl-STOP pressed
        /// </summary>
        public static readonly byte STOP = 0x9F;

        /// <summary>
        /// Ctrl-C pressed
        /// </summary>
        public static readonly byte CTRLC = 0x9E;

        /// <summary>
        /// Disk operation aborted
        /// </summary>
        public static readonly byte ABORT = 0x9D;

        /// <summary>
        /// Error on standard output
        /// </summary>
        public static readonly byte OUTERR = 0x9C;

        /// <summary>
        /// Error on standard input
        /// </summary>
        public static readonly byte INERR = 0x9B;

        /// <summary>
        /// Wrong version of COMMAND
        /// </summary>
        public static readonly byte BADCOM = 0x8F;

        /// <summary>
        /// Unrecognized command
        /// </summary>
        public static readonly byte BADCM = 0x8E;

        /// <summary>
        /// Command too long
        /// </summary>
        public static readonly byte BUFUL = 0x8D;

        /// <summary>
        /// (no description)
        /// </summary>
        public static readonly byte OKCMD = 0x8C;

        /// <summary>
        /// Too many parameters
        /// </summary>
        public static readonly byte INP = 0x8A;

        /// <summary>
        /// Missing parameter
        /// </summary>
        public static readonly byte NOPAR = 0x89;

        /// <summary>
        /// Invalid option
        /// </summary>
        public static readonly byte IOPT = 0x88;

        /// <summary>
        /// Invalid number
        /// </summary>
        public static readonly byte BADNO = 0x87;

        /// <summary>
        /// File for HELP not found
        /// </summary>
        public static readonly byte NOHELP = 0x86;

        /// <summary>
        /// Wrong version of MSX-DOS
        /// </summary>
        public static readonly byte BADVER = 0x85;

        /// <summary>
        /// Cannot concatenate destination file
        /// </summary>
        public static readonly byte NOCAT = 0x84;

        /// <summary>
        /// Cannot create destination file
        /// </summary>
        public static readonly byte BADEST = 0x83;

        /// <summary>
        /// File cannot be copied onto itself
        /// </summary>
        public static readonly byte COPY = 0x82;

        /// <summary>
        /// Cannot overwrite previous destination file
        /// </summary>
        public static readonly byte OVDEST = 0x81;

        public static string NameOf(byte errorCode)
        {
            return errorNamesByCode.ContainsKey(errorCode) ?
                errorNamesByCode[errorCode] :
                $"{errorCode:X2}h";
        }

        public static byte CodeOf(string errorName)
        {
            if (errorName.StartsWith(".")) errorName = errorName.Substring(1);
            var pair = errorNamesByCode.SingleOrDefault(n => n.Value.Equals(errorName, StringComparison.InvariantCultureIgnoreCase));
            if (pair.Value == null)
                throw new ArgumentException($"No DOS error exists with name {errorName}");
            return pair.Key;
        }

        public static bool CodeExists(byte errorCode) => errorNamesByCode.ContainsKey(errorCode);

        public static bool NameExists(string errorName) =>
                errorNamesByCode.SingleOrDefault(
                    n => n.Value.Equals(errorName, StringComparison.InvariantCultureIgnoreCase)
                ).Value != null;

        private static Dictionary<byte, string> errorNamesByCode = new Dictionary<byte, string>
        {
            {0xFF, "NCOMP"},
            {0xFE, "WRERR"},
            {0xFD, "DISK"},
            {0xFC, "NRDY"},
            {0xFB, "VERFY"},
            {0xFA, "DATA"},
            {0xF9, "RNF"},
            {0xF8, "WPROT"},
            {0xF7, "UFORM"},
            {0xF6, "NDOS"},
            {0xF5, "WDISK"},
            {0xF4, "WFILE"},
            {0xF3, "SEEK"},
            {0xF2, "IFAT"},
            {0xF1, "NOUPB"},
            {0xF0, "IFORM"},
            {0xDF, "INTER"},
            {0xDE, "NORAM"},
            {0xDC, "IBDOS"},
            {0xDB, "IDRV"},
            {0xDA, "IFNM"},
            {0xD9, "IPATH"},
            {0xD8, "PLONG"},
            {0xD7, "NOFIL"},
            {0xD6, "NODIR"},
            {0xD5, "DRFUL"},
            {0xD4, "DKFUL"},
            {0xD3, "DUPF"},
            {0xD2, "DIRE"},
            {0xD1, "FILRO"},
            {0xD0, "DIRNE"},
            {0xCF, "IATTR"},
            {0xCE, "DOT"},
            {0xCD, "SYSX"},
            {0xCC, "DIRX"},
            {0xCB, "FILEX"},
            {0xCA, "FOPEN"},
            {0xC8, "FILE"},
            {0xC7, "EOF"},
            {0xC6, "ACCV"},
            {0xC5, "IPROC"},
            {0xC4, "NHAND"},
            {0xC3, "IHAND"},
            {0xC2, "NOPEN"},
            {0xC1, "IDEV"},
            {0xC0, "IENV"},
            {0xBF, "ELONG"},
            {0xBE, "IDATE"},
            {0xBD, "ITIME"},
            {0xBC, "RAMDX"},
            {0xBB, "NRAMD"},
            {0xBA, "HDEAD"},
            {0xB9, "EOL"},
            {0xB8, "ISBFN"},
            {0x9F, "STOP"},
            {0x9E, "CTRLC"},
            {0x9D, "ABORT"},
            {0x9C, "OUTERR"},
            {0x9B, "INERR"},
            {0x8F, "BADCOM"},
            {0x8E, "BADCM"},
            {0x8D, "BUFUL"},
            {0x8C, "OKCMD"},
            {0x8A, "INP"},
            {0x89, "NOPAR"},
            {0x88, "IOPT"},
            {0x87, "BADNO"},
            {0x86, "NOHELP"},
            {0x85, "BADVER"},
            {0x84, "NOCAT"},
            {0x83, "BADEST"},
            {0x82, "COPY"},
            {0x81, "OVDEST"}
        };
    }
}
