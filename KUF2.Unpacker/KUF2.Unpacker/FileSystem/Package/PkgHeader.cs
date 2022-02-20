using System;

namespace KUF2.Unpacker
{
    class PkgHeader
    {
        public UInt32 dwMagic { get; set; } // BEAR (0x52414542)
        public UInt16 wMinorVersion { get; set; } // 1
        public UInt16 wMajorVersion { get; set; } // 2
        public Int64 dwTotalFiles { get; set; }
        public Int64 dwTableSize { get; set; }
        public UInt32 dwSeed { get; set; }
    }
}
