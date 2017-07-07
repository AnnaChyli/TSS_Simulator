using System.Collections.Generic;
using System.Numerics;


namespace SecretSharingLogic
{
    public class SharedSecret
    {
        private readonly IrreduciblePolynom _irreduciblePolynom;
        private readonly FFPolynom[] _allCoefficients;
        
        public int Threshold { get; private set; }
     //   internal FFPolynom[] AllCoefficients { get { return _allCoefficients; } }


        public SharedSecret(int threshold, IrreduciblePolynom irreduciblePolynomial, FFPolynom[] allCoefficients)
        {
            Threshold = threshold;
            _irreduciblePolynom = irreduciblePolynomial;
            _allCoefficients = allCoefficients;
        }

        //Returns enumerator of shares by calculating (x, y) pair points for a given amount of shares 
        public IEnumerable<Share> GetShares(int totalShares)
        {
            for (int i = 1; i <= totalShares; ++i)
            {
                FFPolynom x = new FFPolynom(_irreduciblePolynom, new BigInteger(i));
                FFPolynom y = FFPolynom.HornerEvaluateAt(_allCoefficients, i);

                yield return new Share(new FFPoint(x, y));
            }
        }     
    }
}
