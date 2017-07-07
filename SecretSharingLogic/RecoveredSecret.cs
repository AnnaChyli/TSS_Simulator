using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharingLogic
{
    //The class represents a recovered secret phrase
    public class RecoveredSecret
    {
        public RecoveredSecret(byte[] recoveredBytes)
        {
            Bytes = recoveredBytes;
        }
               
        public byte[] Bytes { get; private set; }

        public string RecoveredText
        {
            get { return Encoding.UTF8.GetString(Bytes); }
        }
    }
}
