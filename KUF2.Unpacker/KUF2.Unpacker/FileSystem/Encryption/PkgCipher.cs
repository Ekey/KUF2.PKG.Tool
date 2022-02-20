using System;

namespace KUF2.Unpacker
{
    class PkgCipher
    {
        public static UInt32 iDecryptUInt32(UInt32 dwValue, PkgHeader m_Header)
        {
            UInt32 dwResult = m_Header.dwSeed * 0x19660D + 0x3C6EF35F;
            m_Header.dwSeed = dwResult;
            return dwValue + dwResult;
        }

        public static Int32 iDecryptInt32(Int32 dwValue, PkgHeader m_Header)
        {
            UInt32 dwResult = m_Header.dwSeed * 0x19660D + 0x3C6EF35F;
            m_Header.dwSeed = dwResult;
            return (Int32)(dwValue + dwResult);
        }

        public static UInt64 iDecryptUInt64(UInt64 dwValue, PkgHeader m_Header)
        {
            UInt32 dwResult = m_Header.dwSeed * 0x19660D + 0x3C6EF35F;
            m_Header.dwSeed = dwResult;
            return dwValue + dwResult;
        }
    }
}
