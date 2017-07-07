using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SecretSharingLogic
{
    //This class adds a functionality for the BigInt and byte[]
    public static class MathExtensions
    {
        private static BigInteger GetBit(int bitPosition)
        {
            return BigInteger.One << bitPosition;
        }

        //Checks if the num set (has 1) at certain bit-position   
        public static bool CheckBit(this BigInteger num, int bitPosition)
        {
            return (num & GetBit(bitPosition)) != 0;
        }

        //Sets the bit at given bit-position for this BigInt num
        public static BigInteger SetBit(this BigInteger num, int bitPosition)
        {
            num = num | GetBit(bitPosition);
            return num;
        }

        //Calculates the length of the given number, and returns number of bits
        public static int GetBitLength(this BigInteger num)
        {
            int bitCount = 0;
            BigInteger remainder = num;
            
            while (remainder > 0)
            {
                bitCount++;
                remainder = remainder >> 1;
            }

            return bitCount;
        }

        
        public static byte[] Reverse(this byte[] byteArray)
        {
            byte[] reversedArr = new byte[byteArray.Length];
            for (int i = 0; i < byteArray.Length; i++)
            {
                reversedArr[(byteArray.Length - i) - 1] = byteArray[i];
            }

            return reversedArr;
        }

        public static byte[] ConcatZeroByte(this byte[] byteArray)
        {
            return byteArray.Concat(new byte[] { 0x00 }).ToArray();
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //Convertations

        //Format the polyValue to the classic polynom-looking string (x^a + x^b + ...)
        public static string ToPolyString(this BigInteger num)
        {
            StringBuilder bld = new StringBuilder();

            //Go thru given num and check for the bit set, and format the string for each iteration
            for (int i = num.GetBitLength(); i >= 0; i--)
            {
                if (num.CheckBit(i))
                {
                    if (bld.Length > 0)
                        bld.Append(" + ");

                    bld.Append((i > 0) ? "x" : "1");

                    if (i > 1)
                    {
                        bld.Append("^");
                        bld.Append(i);
                    }
                }
            }

            if (bld.Length == 0)
                bld.Append("0");

            return bld.ToString();
        }

        //Reverse byte array from lowendian to bigEndian
        public static byte[] ToUnsignedBigEnd(this BigInteger num)
        {
            byte[] bytes = num.ToUnsignedLittleEnd();
            Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToUnsignedLittleEnd(this BigInteger num)
        {
            byte[] byteArray = num.ToByteArray();

            if ((byteArray[byteArray.Length - 1] == 0x00) && (byteArray.Length > 1))
            {
                byte[] byteArrMissingEnd = new byte[byteArray.Length - 1];

                Array.Copy(byteArray, byteArrMissingEnd, byteArrMissingEnd.Length);
                return byteArrMissingEnd;
            }
            return byteArray;
        }

        public static BigInteger FromLittleEndUnsignedBytesToBigInt(this byte[] byteArray)
        {
            //Consider: positive (unsigned) - append a 0 high order byte
            return new BigInteger(byteArray.ConcatZeroByte());
        }

        public static BigInteger FromBigEndUnsignedBytesToBigInt(this byte[] byteArray)
        {
            byte[] litlEnd = byteArray.Reverse();

            return litlEnd.FromLittleEndUnsignedBytesToBigInt();
        }

        //public static string ToHexString(this byte[] byteArray)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (byte b in byteArray)
        //    {
        //        sb.Append(b.ToString("x2"));
        //    }

        //    return sb.ToString();
        //}   

        
    }
}