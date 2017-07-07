using System.Numerics;

namespace SecretSharingLogic
{
    // The class is used to represent a finite field polynom mod irreducible polynom
    public class FFPolynom
    {
        //Represents a prime polynom - when the only factors a polynomial are 1 and itself
        public IrreduciblePolynom PrimePolynom { get; }

        //Represents polynom coefficients thru BigInteger
        public BigInteger PolyValue
        {
            get { return _polynomCoefficients; }
        }

        //Represent a numeric value of all coefficients
        private readonly BigInteger _polynomCoefficients;

        //Constructor to create FiniteFieldPolynom object passing the irreducible polynom and its polyn value
        public FFPolynom (IrreduciblePolynom primePoly, BigInteger poly)
        {
            _polynomCoefficients = poly;
            PrimePolynom = primePoly;      
        }

        //Returns a finite field polynom with PolymnomValue = n
        public FFPolynom ReturnValueInField(long n)
        {
            return new FFPolynom(PrimePolynom, new BigInteger(n));
        }
        
        //Exchange two given numbers
        private static void Exchange(ref BigInteger num1, ref BigInteger num2)
        {
            BigInteger temp = num2;
            num2 = num1;
            num1 = temp;
        }

        //Returns a new object of FiniteFieldPolynom by creating a new polynom with the same properties as caller has
        public FFPolynom DeepCopy()
        {
            return new FFPolynom(PrimePolynom, _polynomCoefficients);
        }

        //Evaluate polynom of coefficients for certain x using Horner's method http//en.wikipedia.org/wiki/Horner_scheme
        public static FFPolynom HornerEvaluateAt(FFPolynom[] coefficients, long x)
        {
            FFPolynom xPoly = coefficients[0].ReturnValueInField(x);

            //The coefficient for the highest mono-polynom = 1            
            FFPolynom resValue = xPoly.DeepCopy();

            //For all the coeffs, go thru them and add lowest first coeff, further multiolying by x-polyn
            //Start backwards
            for (int i = coefficients.Length - 1; i > 0; i--)
            {
                resValue = resValue + coefficients[i];
                resValue = resValue * xPoly;
            }

            resValue = resValue + coefficients[0];

            return resValue;
        }

        //Operator + overload. Represents finite field polynoms addition by using theirs polynom values/coefficients.
        //^ is a bitwise exclusive-OR. XOR with more than one digit, performs the exclusive-OR column by column
        public static FFPolynom operator +(FFPolynom left, FFPolynom right)
        {
            BigInteger result = left._polynomCoefficients ^ right._polynomCoefficients;
            return new FFPolynom(left.PrimePolynom, result);
        }

        //Operator *. Represents multiplication of two finite field polynoms using modified AncientEgyptian/Russian peasant multiplication method.
        //It decomposes one of the multiplicands (preferably the smaller) into a sum of powers of two and creates a table of doublings 
        //of the second multiplicand. This method may be called mediation and duplation, where mediation means halving one number, 
        //and duplation means doubling the other number.
        public static FFPolynom operator *(FFPolynom left, FFPolynom right)
        {
            //Invariant is (b * a + p) is product. Continue to 2*a and taking b/2. 
            //If b is odd number => calculate a + p

            BigInteger a = left._polynomCoefficients;
            BigInteger b = right._polynomCoefficients;
            BigInteger product = BigInteger.Zero; //p

            //Degree is a length of the entered phrase in bytes * 8
            int degree = left.PrimePolynom.Degree;

            //Shift a BigInt value a specified number of bits to the left (preserves the sign) 
            //"BigInteger.One << degree" means 1 * 2^degree
            //- BigInteger.One means to invert all bits
            // 2^degree -1 => length in bits w/o sign
            BigInteger mask = (BigInteger.One << degree) - BigInteger.One;

            for (int i = 0; i < degree; i++)
            {
                if ((a == BigInteger.Zero) || (b == BigInteger.Zero))
                {
                    break;
                }

                //Take lowest bit of b, and check if it set meaning b is odd
                if (b.CheckBit(0))
                {
                    //b odd, do a + product
                    product = product ^ a;
                }
                
                //Check if the highest bit set for a
                bool isHighBitSet = a.CheckBit(degree - 1);

                a = a << 1; // a*2
                a = a & mask; // remove the highest bit 

                if (isHighBitSet)
                {
                    a = a ^ left.PrimePolynom.PolynomValue;
                    a = a & mask;
                }

                b = b >> 1; // b/2
            }

            product = product & mask;

            return new FFPolynom(left.PrimePolynom, product);
        }

        //Computes an inverse of the given polynom mod irreducible polynom using the Euclidean algorithm
        public FFPolynom GetInverse()
        {
            //Use monic polynpm (like x^2) and divide by it
            //Apply shifts for multiplication/division
            //Find inverse h, where g * h ≡ 1 (mod f), or equivalently gh + kf = 1

            BigInteger rMinus2 = PrimePolynom.PolynomValue; //irreducible
            BigInteger rMinus1 = _polynomCoefficients;
            BigInteger aMinus2 = BigInteger.Zero;
            BigInteger aMinus1 = BigInteger.One;

            while (!rMinus1.Equals(BigInteger.One))
            {
                //Find number of shifts to further exchange two nums
                int numOfShifts = rMinus2.GetBitLength() - rMinus1.GetBitLength();

                if (numOfShifts < 0)
                {
                    Exchange(ref rMinus2, ref rMinus1);
                    Exchange(ref aMinus2, ref aMinus1);
                    numOfShifts = -numOfShifts;
                }

                // Now r_minus2 should be as big or bigger than r_minus1
                // q = BigInteger.One.ShiftLeft(numOfShifts)
                BigInteger rMinus1TimesQ = rMinus1 << numOfShifts;
                BigInteger rNew = rMinus1TimesQ ^ rMinus2;

                BigInteger aNew = (aMinus1 << numOfShifts) ^ aMinus2;

                rMinus2 = rMinus1;
                rMinus1 = rNew;
                aMinus2 = aMinus1;
                aMinus1 = aNew;
            }

            return new FFPolynom(PrimePolynom, aMinus1);
        }

        ////Returns value in field at 1
        //public FFPolynom AtOne
        //{
        //    get { return ReturnValueInField(1); }
        //}

        ////Returns value in field at 0
        //public FFPolynom AtZero
        //{
        //    get { return ReturnValueInField(0); }
        //}

        //Converts polynom coeff. to string representation
        //Returns the polynomial coefficients formated as a string
        public override string ToString()
        {
            return _polynomCoefficients.ToPolyString();
        }

        //Overriden just in case (can be deleted)
        public override int GetHashCode()
        {
            return PolyValue.GetHashCode() ^ PrimePolynom.GetHashCode();
        }



        //Returns whether two finite fields polynoms are equal
        public override bool Equals(object obj)
        {
            FFPolynom fpp = obj as FFPolynom;
            if (fpp == null)
                return base.Equals(obj);
          
            bool res = (PrimePolynom == fpp.PrimePolynom) && (PolyValue.Equals(fpp.PolyValue));
            return res;
        }

        
    }
}
