using System;
using System.Text.RegularExpressions;


namespace SecretSharingLogic
{
    // Represents the secret shares distributed to others
    public class Share
    {
        public FFPoint Point { get; private set; }
        internal string ParsedVal { get; private set; }
        internal const string RegexPattern = FFPoint.RegFormat;
        private static readonly Regex RegParser = new Regex(RegexPattern);

        public Share(FFPoint point)
        {
            Point = point;
        }

        public override string ToString()
        {
            return Point.ToString();
        }


        public static bool TryParse(string s, out Share share)
        {
            //Check format for string of future share 
            Match match = RegParser.Match(s);

            FFPoint point;
            
            if (!FFPoint.TryParse(match, out point) || !match.Success)
            {
                share = null;
                return false;
            }

            share = new Share(point);
           
            share.ParsedVal = s;
            return true;
        }

        public static Share Parse(string s)
        {
            Share share;

            if (TryParse(s, out share))
                return share;
            
            throw new InvalidOperationException(s);
        }
    }
}
