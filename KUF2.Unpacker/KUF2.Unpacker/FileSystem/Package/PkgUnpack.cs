using System;
using System.IO;
using System.Collections.Generic;

namespace KUF2.Unpacker
{
    class PkgUnpack
    {
        static List<PkgEntry> m_EntryTable = new List<PkgEntry>();

        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            PkgHashList.iLoadProject();
            using (FileStream TPkgStream = File.OpenRead(m_Archive))
            {
                var lpHeader = TPkgStream.ReadBytes(16);
                var m_Header = new PkgHeader();

                using (var THeaderReader = new MemoryStream(lpHeader))
                {
                    m_Header.dwMagic = THeaderReader.ReadUInt32();
                    m_Header.wMinorVersion = THeaderReader.ReadUInt16();
                    m_Header.wMajorVersion = THeaderReader.ReadUInt16();
                    m_Header.dwTotalFiles = THeaderReader.ReadInt64();

                    if (m_Header.dwMagic != 0x52414542)
                    {
                        throw new Exception("[ERROR]: Invalid magic of PKG file!");
                    }

                    if (m_Header.wMinorVersion != 1)
                    {
                        throw new Exception("[ERROR]: Invalid minor version of PKG file!");
                    }

                    if (m_Header.wMajorVersion != 2)
                    {
                        throw new Exception("[ERROR]: Invalid major version of PKG file!");
                    }

                    m_Header.dwTableSize = (m_Header.dwTotalFiles * 3) << 3;
                    m_Header.dwSeed = (UInt32)TPkgStream.Length;

                    THeaderReader.Dispose();
                }

                m_EntryTable.Clear();
                for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                {
                    UInt64 dwNameHash = TPkgStream.ReadUInt64();
                    Int32 dwCompressedSize = TPkgStream.ReadInt32();
                    Int32 dwDecompressedSize = TPkgStream.ReadInt32();
                    Int32 dwFileNum = TPkgStream.ReadInt32();
                    Int32 dwReserved = TPkgStream.ReadInt32();

                    var TEntry = new PkgEntry
                    {
                        dwNameHash = PkgCipher.iDecryptUInt64(dwNameHash, m_Header),
                        dwCompressedSize = PkgCipher.iDecryptInt32(dwCompressedSize, m_Header),
                        dwDecompressedSize = PkgCipher.iDecryptInt32(dwDecompressedSize, m_Header),
                        dwFileNum = PkgCipher.iDecryptInt32(dwFileNum, m_Header),
                        dwReserved = PkgCipher.iDecryptInt32(dwReserved, m_Header),
                    };

                    m_EntryTable.Add(TEntry);
                }

                foreach (var m_Entry in m_EntryTable)
                {
                    String m_FileName = PkgHashList.iGetNameFromHashList(m_Entry.dwNameHash);
                    String m_FullPath = m_DstFolder + m_FileName;

                    Utils.iSetInfo("[UNPACKING]: " + m_FileName);
                    Utils.iCreateDirectory(m_FullPath);

                    Int32 dwFileSize = 0;
                    Int32 dwTempPos = 0;
                    Byte[] lpDstBuffer = new Byte[m_Entry.dwDecompressedSize];
                    do
                    {
                        UInt32 dwChunkOffset = TPkgStream.ReadUInt32();
                        Int32 dwChunkCompressedSize = TPkgStream.ReadInt32();
                        Int32 dwChunkDecompressedSize = TPkgStream.ReadInt32();
                        Int64 dwChunkTableOffset = TPkgStream.Position;

                        dwChunkOffset = PkgCipher.iDecryptUInt32(dwChunkOffset, m_Header);
                        dwChunkCompressedSize = PkgCipher.iDecryptInt32(dwChunkCompressedSize, m_Header);
                        dwChunkDecompressedSize = PkgCipher.iDecryptInt32(dwChunkDecompressedSize, m_Header);

                        TPkgStream.Seek(dwChunkOffset, SeekOrigin.Begin);

                        if (dwChunkCompressedSize == 0)
                        {
                            var lpTemp = TPkgStream.ReadBytes(dwChunkDecompressedSize);
                            Array.Copy(lpTemp, 0, lpDstBuffer, dwTempPos, lpTemp.Length);
                            dwTempPos += lpTemp.Length;
                            dwFileSize += lpTemp.Length;
                        }
                        else
                        {
                            var lpSrcTemp = TPkgStream.ReadBytes(dwChunkCompressedSize);
                            var lpDstTemp = ZLIB.iDecompress(lpSrcTemp);
                            Array.Copy(lpDstTemp, 0, lpDstBuffer, dwTempPos, lpDstTemp.Length);
                            dwTempPos += lpDstTemp.Length;
                            dwFileSize += lpDstTemp.Length;
                        }

                        TPkgStream.Seek(dwChunkTableOffset, SeekOrigin.Begin);
                    }
                    while (dwFileSize != m_Entry.dwDecompressedSize);

                    File.WriteAllBytes(m_FullPath, lpDstBuffer);
                }

                TPkgStream.Dispose();
            }
        }
    }
}
