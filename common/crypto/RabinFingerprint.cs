
namespace Paradigm.Common
{
    using System;
    using System.Text;
    
    internal static class RabinFingerPrint
    {
        private static readonly UInt64 p_lcr = 0x000000000000001BL;
        private static readonly int K = 64;
        private static readonly UInt64 T_K_minus_1 = (UInt64)1L << (K - 1);

        private static UInt64[] tableA_ = new UInt64[256];
        private static UInt64[] tableB_ = new UInt64[256];
        private static UInt64[] tableC_ = new UInt64[256];
        private static UInt64[] tableD_ = new UInt64[256];
        private static UInt64[] tableE_ = new UInt64[256];
        private static UInt64[] tableF_ = new UInt64[256];
        private static UInt64[] tableG_ = new UInt64[256];
        private static UInt64[] tableH_ = new UInt64[256];

        static RabinFingerPrint()
        {
            InitTables();
        }

        private static void InitTables()
        {
            UInt64[] mods = new UInt64[K];

            mods[0] = p_lcr;
            for (int i = 1; i < K; ++i)
            {

                mods[i] = mods[i - 1] << 1;

                if ((mods[i - 1] & T_K_minus_1) != 0)
                    mods[i] ^= p_lcr;
            }

            for (int i = 0; i < 256; ++i)
            {
                int control = i;
                for (int j = 0; j < 8 && control > 0; ++j)
                {
                    if ((control & 1) == 1)
                    {
                        tableA_[i] ^= mods[j + 56];
                        tableB_[i] ^= mods[j + 48];
                        tableC_[i] ^= mods[j + 40];
                        tableD_[i] ^= mods[j + 32];
                        tableE_[i] ^= mods[j + 24];
                        tableF_[i] ^= mods[j + 16];
                        tableG_[i] ^= mods[j + 8];
                        tableH_[i] ^= mods[j];
                    }
                    control >>= 1;
                }
            }
        }

        private static UInt64 ComputeTablesSum(ref UInt64 value)
        {
            value = tableH_[((value) & 0xFF)] ^
                    tableG_[((value >> 8) & 0xFF)] ^
                    tableF_[((value >> 16) & 0xFF)] ^
                    tableE_[((value >> 24) & 0xFF)] ^
                    tableD_[((value >> 32) & 0xFF)] ^
                    tableC_[((value >> 40) & 0xFF)] ^
                    tableB_[((value >> 48) & 0xFF)] ^
                    tableA_[((value >> 56) & 0xFF)];
            return value;
        }

        private static UInt64 Compute(UInt64[] HashArray)
        {
            UInt64 w = 0L;
            for (int s = 0; s < HashArray.Length; ++s)
                w = ComputeTablesSum(ref w) ^ HashArray[s];
            return w;
        }

        internal static UInt64 ComputeFingerPrint(string source)
        {
            byte[] table = Encoding.Unicode.GetBytes(source);
            UInt64[] values = new UInt64[table.LongLength];
            ConvertBytes(ref table, ref values);
            return Compute(values);
        }

        internal static UInt64 ComputeFingerPrint(ref string source)
        {
            return ComputeFingerPrint(source);
        }

        internal static UInt64 ComputeFingerPrint(ref byte[] source)
        {
            UInt64[] values = new UInt64[source.LongLength];
            ConvertBytes(ref source, ref values);
            return Compute(values);
        }

        internal static UInt64 ComputeFingerPrint(byte[] source)
        {
            return ComputeFingerPrint(ref source);
        }

        private static void ConvertBytes(ref byte[] source, ref UInt64[] destination)
        {
            for (long i = 0; i < source.LongLength; i++)
                destination[i] = Convert.ToUInt64(source[i]);
        }
    }
}