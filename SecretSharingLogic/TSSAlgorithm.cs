using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SecretSharingLogic
{
    //The class implements the logic for TSS secret sharing scheme 
    //implementing 
    public class TSSAlgorithm
    {
        //Share the secret phrase with given number of shares, and threshold to recover it
        public string[] Share(string message, int threshold, int numberOfShares)
        {
            //Encode a given text into a sequence of bytes
            byte[] encodedMessage = Encoding.UTF8.GetBytes(message);
            SharedSecret secret = ShareImpl(encodedMessage, threshold, numberOfShares);
            IEnumerable<Share> shares = secret.GetShares(numberOfShares);

            return ToStringArray(shares.ToArray());
        }

        //Combines shares to recover the secret
        public RecoveredSecret Recover(IEnumerable<string> allShares)
        {
            //For a list of all shares, do for each: check for the share format. If matches, parse the share and add to the container with good shares
            IEnumerable<Share> shares = allShares.Select(share =>
                {
                    return Regex.Match(share, SecretSharingLogic.Share.RegexPattern).Value;
                })
                  .Select(SecretSharingLogic.Share.Parse);

            //Check to see if obtained list of shares has duplicates
            Share[] enumerable = shares as Share[] ?? shares.ToArray();
            for (int i = 0; i + 1 < enumerable.ToArray().Length; i++)
            {
                if (!enumerable[i].ParsedVal.Equals(enumerable[i + 1].ParsedVal))
                    continue;

                return null;
            }

            return RecoverImpl(enumerable); 
        }

        //Shares the secret respectfully to number of shares and threshold
        protected virtual SharedSecret ShareImpl(byte[] secret, int threshold, int numberOfShares)
        {
            //Define irred polynom for a secret
            IrreduciblePolynom irreduciblePolynom = IrreduciblePolynom.GiveFromBytes(secret.Length);

            BigInteger rawSecret = secret.FromBigEndUnsignedBytesToBigInt();
            FFPolynom secretCoeff = new FFPolynom(irreduciblePolynom, rawSecret);

            //Generate random polynom with corresponding irred. polynom and given threshold
            IEnumerable<FFPolynom> randPolynom = GenerateRandomPolynom(irreduciblePolynom, threshold - 1);

            //Construct an array of all coefficients represented as finite field polynoms
            FFPolynom[] consTerm = new[] {secretCoeff};

            //Concatenate previously created random coefficients converting them as an array
            FFPolynom[] allCoefficients = consTerm.Concat(randPolynom).ToArray();  

            return new SharedSecret(threshold, irreduciblePolynom, allCoefficients);
        }

        //Converts array of shares to array of strings
        private string[] ToStringArray(Share[] shares)
        {
            List<string> list = new List<string>();
            foreach (Share sh in shares)
            {
                list.Add(sh.ToString());
            }
            return list.ToArray();
        }

        //Generates enumerable for random finite field polynoms using a given Irreducible polynom and size
        private static IEnumerable<FFPolynom> GenerateRandomPolynom(IrreduciblePolynom irreduciblePolynomial, int total)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            for (int i = 0; i < total; i++)
            {
                //Size in bytes = Degree/8 to accomodate the length of the secret phrase
                byte[] randomCoeffs = new byte[irreduciblePolynomial.SizeInBytes];
                rng.GetBytes(randomCoeffs);
                yield return new FFPolynom(irreduciblePolynomial, randomCoeffs.FromLittleEndUnsignedBytesToBigInt());
            }
        }

        //Combine shares to recover the secret by applying Lagrange Interpolation for all FFPoints
        private RecoveredSecret RecoverImpl(IEnumerable<Share> shares)
        {
            Share[] allShares = shares.ToArray();

            //Create a collection of finite field points out from generated shares
            IEnumerable<FFPoint> collection = allShares.Select(s => s.Point);

            //Recover the secret phrase using the points -> f(0)
            FFPolynom secretCoeff = LagrangeInterpolation.LagrInterpolate(collection);

            byte[] secret = secretCoeff.PolyValue.ToUnsignedBigEnd();
            
            RecoveredSecret newRecSecret = new RecoveredSecret(secret);
            return newRecSecret;
        }
    }   
}


