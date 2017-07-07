using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SecretSharingLogic
{
    //The class represents Lagrange Interpolation method 
    public class LagrangeInterpolation
    {
        /// <summary>
        /// /Calculate f(0) in a finite field
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static FFPolynom LagrInterpolate(IEnumerable<FFPoint> points)
        {
            FFPoint[] originalPoints = points.ToArray();

            if (originalPoints.Length == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            int threshold = originalPoints.Length;

            //"Correct" these points by removing the high term monomial
            FFPoint[] adjustedPoints = originalPoints.Select(p => AdjustPoint(threshold, p)).ToArray();

            //Use Lagrange interpolating polynomials ( http//en.wikipedia.org/wiki/Lagrange_polynomial )
            //to solve:

            //        (x-x2)(x-x3)...(x-xn)         (x-x1)(x-x3)...(x-xn)
            //P(x) = ------------------------ y1 + -------------------------- y2 + ... + 
            //        (x1-x2)(x1-x3)...(x1-xn)      (x2-x1)(x2-x3)...(x2-xn-1)

            //Simplifying things is that x is 0 since we want to find the constant term            

            //Polynomial that belongs to the GF[2] and represents the initially constructed polynom
           FFPolynom originalPolynom = originalPoints[0].Y;

            FFPolynom total = originalPolynom.ReturnValueInField(0); 

            for (int iPoint = 0; iPoint < threshold; iPoint++)// where iPoint is currently processing point
            {
                FFPolynom processingNumerator = originalPolynom.ReturnValueInField(1); 
                FFPolynom processingDenominator = originalPolynom.ReturnValueInField(1); 
                FFPoint processingPoint = adjustedPoints[iPoint];

                for (int tempPoint = 0; tempPoint < threshold; tempPoint++)
                {
                    //Skip if both are the same
                    if (iPoint == tempPoint)
                        continue;
                    
                    //numerator needs multiplied by 
                    //(0-x_i) = -x_i = x_i 
                    //subtraction and addition are the same in GF[2]
                    processingNumerator *= adjustedPoints[tempPoint].X;
                    processingDenominator *= (processingPoint.X + adjustedPoints[tempPoint].X);
                }

                //Dividing is just multiplying by the inverse
                FFPolynom denomInv = processingDenominator.GetInverse();

                FFPolynom fraction = processingNumerator * denomInv;

                //Multiply the fraction by the corresponding y_i
                FFPolynom currentTermValue = fraction * processingPoint.Y;
                total += currentTermValue;
            }

            return total;
        }

        //Adjusts the finite field points by removing the high term monomial
        private static FFPoint AdjustPoint(int total, FFPoint point)
        {
            FFPolynom corrector = new FFPolynom(point.Y.PrimePolynom, BigInteger.One);
            FFPolynom corrFactor = point.X;

            for (int i = 1; i <= total; i++)
            {
                corrector = corrector * corrFactor;
            }

            FFPolynom newY = point.Y + corrector;
            FFPoint newPoint = new FFPoint(point.X, newY);
            return newPoint;
        }
    
    }
}
