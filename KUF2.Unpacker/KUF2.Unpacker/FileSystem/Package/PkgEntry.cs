using System;

namespace KUF2.Unpacker
{
    class PkgEntry
    {
        public UInt64 dwNameHash { get; set; }
        public Int32 dwCompressedSize { get; set; }
        public Int32 dwDecompressedSize { get; set; }
        public Int32 dwFileNum { get; set; }
        public Int32 dwReserved { get; set; }
    }
}
