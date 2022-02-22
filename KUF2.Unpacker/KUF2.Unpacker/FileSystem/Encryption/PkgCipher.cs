using System;

namespace KUF2.Unpacker
{
    class PkgCipher
    {
        public static UInt32 iDecryptUInt32(UInt32 dwValue, PkgHeader m_Header)
        {
            m_Header.dwSeed = m_Header.dwSeed * 0x19660D + 0x3C6EF35F;
            return dwValue + m_Header.dwSeed;
        }

        public static Int32 iDecryptInt32(Int32 dwValue, PkgHeader m_Header)
        {
            m_Header.dwSeed = m_Header.dwSeed * 0x19660D + 0x3C6EF35F;
            return (Int32)(dwValue + m_Header.dwSeed);
        }

        public static UInt64 iDecryptUInt64(UInt64 dwValue, PkgHeader m_Header)
        {
            m_Header.dwSeed = m_Header.dwSeed * 0x19660D + 0x3C6EF35F;
            return dwValue + m_Header.dwSeed;
        }
    }
}
