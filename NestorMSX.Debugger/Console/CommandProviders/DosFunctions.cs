using System;
using System.Collections.Generic;
using System.Linq;

namespace Konamiman.NestorMSX.Z80Debugger.Console.CommandProviders
{
    public class DosFunctions
    {
        /// <summary>
        /// Program Terminate
        /// </summary>
        public static readonly byte TERM0 = 0x00;
    
        /// <summary>
        /// Console Input
        /// </summary>
        public static readonly byte CONIN = 0x01;
    
        /// <summary>
        /// Console Output
        /// </summary>
        public static readonly byte CONOUT = 0x02;
    
        /// <summary>
        /// Auxiliary Input
        /// </summary>
        public static readonly byte AUXIN = 0x03;
    
        /// <summary>
        /// Auxiliary Output
        /// </summary>
        public static readonly byte AUXOUT = 0x04;
    
        /// <summary>
        /// Printer Output
        /// </summary>
        public static readonly byte LSTOUT = 0x05;
    
        /// <summary>
        /// Direct Console I/o
        /// </summary>
        public static readonly byte DIRIO = 0x06;
    
        /// <summary>
        /// Direct Console Input
        /// </summary>
        public static readonly byte DIRIN = 0x07;
    
        /// <summary>
        /// Console Input Without Echo
        /// </summary>
        public static readonly byte INNOE = 0x08;
    
        /// <summary>
        /// String Output
        /// </summary>
        public static readonly byte STROUT = 0x09;
    
        /// <summary>
        /// Buffered Line Input
        /// </summary>
        public static readonly byte BUFIN = 0x0A;
    
        /// <summary>
        /// Console Status
        /// </summary>
        public static readonly byte CONST = 0x0B;
    
        /// <summary>
        /// Return Version Number
        /// </summary>
        public static readonly byte CPMVER = 0x0C;
    
        /// <summary>
        /// Disk Reset
        /// </summary>
        public static readonly byte DSKRST = 0x0D;
    
        /// <summary>
        /// Select Disk
        /// </summary>
        public static readonly byte SELDSK = 0x0E;
    
        /// <summary>
        /// Open File [FCB]
        /// </summary>
        public static readonly byte FOPEN = 0x0F;
    
        /// <summary>
        /// Close File [FCB]
        /// </summary>
        public static readonly byte FCLOSE = 0x10;
    
        /// <summary>
        /// Search For First [FCB]
        /// </summary>
        public static readonly byte SFIRST = 0x11;
    
        /// <summary>
        /// Search For Next [FCB]
        /// </summary>
        public static readonly byte SNEXT = 0x12;
    
        /// <summary>
        /// Delete File [FCB]
        /// </summary>
        public static readonly byte FDEL = 0x13;
    
        /// <summary>
        /// Sequential Read [FCB]
        /// </summary>
        public static readonly byte RDSEQ = 0x14;
    
        /// <summary>
        /// Sequential Write [FCB]
        /// </summary>
        public static readonly byte WRSEQ = 0x15;
    
        /// <summary>
        /// Create File [FCB]
        /// </summary>
        public static readonly byte FMAKE = 0x16;
    
        /// <summary>
        /// Rename File [FCB]
        /// </summary>
        public static readonly byte FREN = 0x17;
    
        /// <summary>
        /// Get Login Vector
        /// </summary>
        public static readonly byte LOGIN = 0x18;
    
        /// <summary>
        /// Get Current Drive
        /// </summary>
        public static readonly byte CURDRV = 0x19;
    
        /// <summary>
        /// Set Disk Transfer Address
        /// </summary>
        public static readonly byte SETDTA = 0x1A;
    
        /// <summary>
        /// Get Allocation Information
        /// </summary>
        public static readonly byte ALLOC = 0x1B;
    
        /// <summary>
        /// Random Read [FCB]
        /// </summary>
        public static readonly byte RDRND = 0x21;
    
        /// <summary>
        /// Random Write [FCB]
        /// </summary>
        public static readonly byte WRRND = 0x22;
    
        /// <summary>
        /// Get File Size [FCB]
        /// </summary>
        public static readonly byte FSIZE = 0x23;
    
        /// <summary>
        /// Set Random Record [FCB]
        /// </summary>
        public static readonly byte SETRND = 0x24;
    
        /// <summary>
        /// Random Block Write [FCB]
        /// </summary>
        public static readonly byte WRBLK = 0x26;
    
        /// <summary>
        /// Random Block Read [FCB]
        /// </summary>
        public static readonly byte RDBLK = 0x27;
    
        /// <summary>
        /// Random Write With Zero Fill [FCB]
        /// </summary>
        public static readonly byte WRZER = 0x28;
    
        /// <summary>
        /// Get Date
        /// </summary>
        public static readonly byte GDATE = 0x2A;
    
        /// <summary>
        /// Set Date
        /// </summary>
        public static readonly byte SDATE = 0x2B;
    
        /// <summary>
        /// Get Time
        /// </summary>
        public static readonly byte GTIME = 0x2C;
    
        /// <summary>
        /// Set Time
        /// </summary>
        public static readonly byte STIME = 0x2D;
    
        /// <summary>
        /// Set/reset Verify Flag
        /// </summary>
        public static readonly byte VERIFY = 0x2E;
    
        /// <summary>
        /// Absolute Sector Read
        /// </summary>
        public static readonly byte RDABS = 0x2F;
    
        /// <summary>
        /// Absolute Sector Write
        /// </summary>
        public static readonly byte WRABS = 0x30;
    
        /// <summary>
        /// Get Disk Parameters
        /// </summary>
        public static readonly byte DPARM = 0x31;
    
        /// <summary>
        /// Find First Entry
        /// </summary>
        public static readonly byte FFIRST = 0x40;
    
        /// <summary>
        /// Find Next Entry
        /// </summary>
        public static readonly byte FNEXT = 0x41;
    
        /// <summary>
        /// Find New Entry
        /// </summary>
        public static readonly byte FNEW = 0x42;
    
        /// <summary>
        /// Open File Handle
        /// </summary>
        public static readonly byte OPEN = 0x43;
    
        /// <summary>
        /// Create File Handle
        /// </summary>
        public static readonly byte CREATE = 0x44;
    
        /// <summary>
        /// Close File Handle
        /// </summary>
        public static readonly byte CLOSE = 0x45;
    
        /// <summary>
        /// Ensure File Handle
        /// </summary>
        public static readonly byte ENSURE = 0x46;
    
        /// <summary>
        /// Duplicate File Handle
        /// </summary>
        public static readonly byte DUP = 0x47;
    
        /// <summary>
        /// Read From File Handle
        /// </summary>
        public static readonly byte READ = 0x48;
    
        /// <summary>
        /// Write To File Handle
        /// </summary>
        public static readonly byte WRITE = 0x49;
    
        /// <summary>
        /// Move File Handle Pointer
        /// </summary>
        public static readonly byte SEEK = 0x4A;
    
        /// <summary>
        /// I/o Control For Devices
        /// </summary>
        public static readonly byte IOCTL = 0x4B;
    
        /// <summary>
        /// Test File Handle
        /// </summary>
        public static readonly byte HTEST = 0x4C;
    
        /// <summary>
        /// Delete File Or Subdirectory
        /// </summary>
        public static readonly byte DELETE = 0x4D;
    
        /// <summary>
        /// Rename File Or Subdirectory
        /// </summary>
        public static readonly byte RENAME = 0x4E;
    
        /// <summary>
        /// Move File Or Subdirectory
        /// </summary>
        public static readonly byte MOVE = 0x4F;
    
        /// <summary>
        /// Get/Set File Attributes
        /// </summary>
        public static readonly byte ATTR = 0x50;
    
        /// <summary>
        /// Get/Set File Date And Time
        /// </summary>
        public static readonly byte FTIME = 0x51;
    
        /// <summary>
        /// Delete File Handle
        /// </summary>
        public static readonly byte HDELETE = 0x52;
    
        /// <summary>
        /// Rename File Handle
        /// </summary>
        public static readonly byte HRENAME = 0x53;
    
        /// <summary>
        /// Move File Handle
        /// </summary>
        public static readonly byte HMOVE = 0x54;
    
        /// <summary>
        /// Get/Set File Handle Attributes
        /// </summary>
        public static readonly byte HATTR = 0x55;
    
        /// <summary>
        /// Get/Set File Handle Date And Time
        /// </summary>
        public static readonly byte HFTIME = 0x56;
    
        /// <summary>
        /// Get Disk Transfer Address
        /// </summary>
        public static readonly byte GETDTA = 0x57;
    
        /// <summary>
        /// Get Verify Flag Setting
        /// </summary>
        public static readonly byte GETVFY = 0x58;
    
        /// <summary>
        /// Get Current Directory
        /// </summary>
        public static readonly byte GETCD = 0x59;
    
        /// <summary>
        /// Change Current Directory
        /// </summary>
        public static readonly byte CHDIR = 0x5A;
    
        /// <summary>
        /// Parse Pathname
        /// </summary>
        public static readonly byte PARSE = 0x5B;
    
        /// <summary>
        /// Parse Filename
        /// </summary>
        public static readonly byte PFILE = 0x5C;
    
        /// <summary>
        /// Check Character
        /// </summary>
        public static readonly byte CHKCHR = 0x5D;
    
        /// <summary>
        /// Get Whole Path String
        /// </summary>
        public static readonly byte WPATH = 0x5E;
    
        /// <summary>
        /// Flush Disk Buffers
        /// </summary>
        public static readonly byte FLUSH = 0x5F;
    
        /// <summary>
        /// Fork To Child Process
        /// </summary>
        public static readonly byte FORK = 0x60;
    
        /// <summary>
        /// Rejoin Parent Process
        /// </summary>
        public static readonly byte JOIN = 0x61;
    
        /// <summary>
        /// Terminate With Error Code
        /// </summary>
        public static readonly byte TERM = 0x62;
    
        /// <summary>
        /// Define Abort Exit Routine
        /// </summary>
        public static readonly byte DEFAB = 0x63;
    
        /// <summary>
        /// Define Disk Error Handler Routine
        /// </summary>
        public static readonly byte DEFER = 0x64;
    
        /// <summary>
        /// Get Previous Error Code
        /// </summary>
        public static readonly byte ERROR = 0x65;
    
        /// <summary>
        /// Explain Error Code
        /// </summary>
        public static readonly byte EXPLAIN = 0x66;
    
        /// <summary>
        /// Format A Disk
        /// </summary>
        public static readonly byte FORMAT = 0x67;
    
        /// <summary>
        /// Create Or Destroy Ramdisk
        /// </summary>
        public static readonly byte RAMD = 0x68;
    
        /// <summary>
        /// Allocate Sector Buffers
        /// </summary>
        public static readonly byte BUFFER = 0x69;
    
        /// <summary>
        /// Logical Drive Assignment
        /// </summary>
        public static readonly byte ASSIGN = 0x6A;
    
        /// <summary>
        /// Get Environment Item
        /// </summary>
        public static readonly byte GENV = 0x6B;
    
        /// <summary>
        /// Set Environment Item
        /// </summary>
        public static readonly byte SENV = 0x6C;
    
        /// <summary>
        /// Find Environment Item
        /// </summary>
        public static readonly byte FENV = 0x6D;
    
        /// <summary>
        /// Get/Set Disk Check Status
        /// </summary>
        public static readonly byte DSKCHK = 0x6E;
    
        /// <summary>
        /// Get MSX-DOS Version Number
        /// </summary>
        public static readonly byte DOSVER = 0x6F;
    
        /// <summary>
        /// Get/Set Redirection State
        /// </summary>
        public static readonly byte REDIR = 0x70;

        public static string NameOf(byte functionCode)
        {
            return functionNamesByCode.ContainsKey(functionCode) ?
                functionNamesByCode[functionCode] :
                $"{functionCode:X2}h";
        }

        public static byte CodeOf(string functionName)
        {
            if (functionName.StartsWith("_")) functionName = functionName.Substring(1);
            var pair = functionNamesByCode.SingleOrDefault(n => n.Value.Equals(functionName, StringComparison.InvariantCultureIgnoreCase));
            if(pair.Value == null)
                throw new ArgumentException($"No DOS function exists with name {functionName}");
            return pair.Key;
        }

        public static bool CodeExists(byte functionCode) => functionNamesByCode.ContainsKey(functionCode);

        public static bool NameExists(string functionName) =>
                functionNamesByCode.SingleOrDefault(
                    n => n.Value.Equals(functionName, StringComparison.InvariantCultureIgnoreCase)
                ).Value != null;

        private static Dictionary<byte, string> functionNamesByCode = new Dictionary<byte, string>()
        {
            {0x00, "TERM0"},
            {0x01, "CONIN"},
            {0x02, "CONOUT"},
            {0x03, "AUXIN"},
            {0x04, "AUXOUT"},
            {0x05, "LSTOUT"},
            {0x06, "DIRIO"},
            {0x07, "DIRIN"},
            {0x08, "INNOE"},
            {0x09, "STROUT"},
            {0x0A, "BUFIN"},
            {0x0B, "CONST"},
            {0x0C, "CPMVER"},
            {0x0D, "DSKRST"},
            {0x0E, "SELDSK"},
            {0x0F, "FOPEN"},
            {0x10, "FCLOSE"},
            {0x11, "SFIRST"},
            {0x12, "SNEXT"},
            {0x13, "FDEL"},
            {0x14, "RDSEQ"},
            {0x15, "WRSEQ"},
            {0x16, "FMAKE"},
            {0x17, "FREN"},
            {0x18, "LOGIN"},
            {0x19, "CURDRV"},
            {0x1A, "SETDTA"},
            {0x1B, "ALLOC"},
            {0x21, "RDRND"},
            {0x22, "WRRND"},
            {0x23, "FSIZE"},
            {0x24, "SETRND"},
            {0x26, "WRBLK"},
            {0x27, "RDBLK"},
            {0x28, "WRZER"},
            {0x2A, "GDATE"},
            {0x2B, "SDATE"},
            {0x2C, "GTIME"},
            {0x2D, "STIME"},
            {0x2E, "VERIFY"},
            {0x2F, "RDABS"},
            {0x30, "WRABS"},
            {0x31, "DPARM"},
            {0x40, "FFIRST"},
            {0x41, "FNEXT"},
            {0x42, "FNEW"},
            {0x43, "OPEN"},
            {0x44, "CREATE"},
            {0x45, "CLOSE"},
            {0x46, "ENSURE"},
            {0x47, "DUP"},
            {0x48, "READ"},
            {0x49, "WRITE"},
            {0x4A, "SEEK"},
            {0x4B, "IOCTL"},
            {0x4C, "HTEST"},
            {0x4D, "DELETE"},
            {0x4E, "RENAME"},
            {0x4F, "MOVE"},
            {0x50, "ATTR"},
            {0x51, "FTIME"},
            {0x52, "HDELETE"},
            {0x53, "HRENAME"},
            {0x54, "HMOVE"},
            {0x55, "HATTR"},
            {0x56, "HFTIME"},
            {0x57, "GETDTA"},
            {0x58, "GETVFY"},
            {0x59, "GETCD"},
            {0x5A, "CHDIR"},
            {0x5B, "PARSE"},
            {0x5C, "PFILE"},
            {0x5D, "CHKCHR"},
            {0x5E, "WPATH"},
            {0x5F, "FLUSH"},
            {0x60, "FORK"},
            {0x61, "JOIN"},
            {0x62, "TERM"},
            {0x63, "DEFAB"},
            {0x64, "DEFER"},
            {0x65, "ERROR"},
            {0x66, "EXPLAIN"},
            {0x67, "FORMAT"},
            {0x68, "RAMD"},
            {0x69, "BUFFER"},
            {0x6A, "ASSIGN"},
            {0x6B, "GENV"},
            {0x6C, "SENV"},
            {0x6D, "FENV"},
            {0x6E, "DSKCHK"},
            {0x6F, "DOSVER"},
            {0x70, "REDIR"}
        };
    }
}
