using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SecretSharingLogic
{
    // The class is used to represent a 2-d point applying finite field polynomials
    public class FFPoint
    {
        public FFPolynom X { get; private set; }
        public FFPolynom Y { get; private set; }

        //Regex for formatted representation of share
        internal const string RegFormat = @"(?<x>[0-9]+).(?<y>[0-9a-fA-F]+)";

        //Checks for matching with Regex and proccess the , and returns FiniteFieldPoint if success; otherwise null.
        internal static bool TryParse(Match matchedShare, out FFPoint res)
        {
            if (!matchedShare.Success)
            {
                res = null;
                return false;
            }
            try
            {
                //Parse the share string: x - number by order, y - the string after '.' sign
                //Gets the string matched by the particular expression. Change to lowcase and apply casing invariant
                string xStr = matchedShare.Groups["x"].Value.ToLowerInvariant();
                string yStr = matchedShare.Groups["y"].Value.ToLowerInvariant();

                //Remove initial 0s to compare with ordinal = 4
                while (xStr.StartsWith("0", StringComparison.Ordinal))
                {
                    xStr = xStr.Substring(1);
                }

                // 1hexChar = 4 bits, so degree in bits = degree * 4
                int polynomialDegree = yStr.Length * 4;

                IrreduciblePolynom irp = new IrreduciblePolynom(polynomialDegree);

                FFPolynom x = new FFPolynom(irp, BigInteger.Parse(xStr));

                // Create an array of bites (big endian) with the length in bytes as initial phrase, initialize to 0
                byte[] yArr = new byte[yStr.Length / 2];

                for (int i = 0; i < yStr.Length; i += 2)
                {
                    //Convert a hex-string to byte representation (1hex = 4 bits).. that's why i/2
                    yArr[i / 2] = Byte.Parse(yStr.Substring(i, 2), NumberStyles.HexNumber);
                }

                //Convert yArr to BigInteger and create a finite field polynom
                FFPolynom y = new FFPolynom(irp, yArr.FromBigEndUnsignedBytesToBigInt());

                res = new FFPoint(x, y);
                return true;
            }
            catch (Exception e)
            {
                res = null;
                return false;
            }
        }

        //Helper, tryies to parse the string representation of share. Returns true if success
        private static bool TryParse(string s, out FFPoint result)
        {
            Match m = Regex.Match(s, RegFormat);
            return TryParse(m, out result);
        }

        //Converts to formatted string value at a point of field 
        private string ToString(int totalPoints)
        {
            StringBuilder bld = new StringBuilder();

            //Aamount of decimal digits the total points require
            for (int remainder = totalPoints; remainder > 0; remainder /= 10)
            {
                bld.Append('0');
            }

            //Format of the value
            string f = bld.ToString();

            //Amount of the expected bytes
            int numOfBytes = Y.PrimePolynom.SizeInBytes;
            byte[] pointBytes = Y.PolyValue.ToUnsignedBigEnd();

            //Add 0s to fill all bytes
            IEnumerable<byte> prefixedPointBytes = Enumerable.Range(0, numOfBytes - pointBytes.Length).Select(ix => (byte)0).Concat(pointBytes);

            //Index of the share
            string index = ((long)X.PolyValue).ToString(f);

            //Format the share definition (actual text)
            string definition = String.Join("", prefixedPointBytes.Select(b => b.ToString("x2")));

            //Construct a string representation for a share with format: index.<definition>
            return index + "." + definition;
        }

        //Constructs the point using two polynoms for x and y dimension respectfully
        public FFPoint(FFPolynom x, FFPolynom y)
        {
            X = x; Y = y;
        }

        //String representation of X-value
        public override string ToString()
        {
            return ToString((int)X.PolyValue);
        }

        //Parse the string share to FiniteFieldPoint
        public static FFPoint Parse(string s)
        {
            FFPoint res;
            if (!TryParse(s, out res))
            {
                throw new ArgumentException("Parsing error");
            }

            return res;
        }
    }
}

