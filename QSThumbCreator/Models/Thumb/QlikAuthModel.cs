using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace QSThumbCreator.Models.Thumb
{
    public class QlikAuthModel
    {
        private readonly List<string> _autoItSpecialChars = new List<string>() {"!", "=", "^", "#"};

        public string QlikServerUrl { get; set; }
        public string QlikAdDomain { get; set; }
        public string QlikAdUsername { get; set; }
        public SecureString QlikAdPassword { get; set; }

        /*
         * Might be a security violation below making the use of a SecureString all for nought
         * because a copy is being made
         */
        public string QlikAutoItPassword
        {
            get
            {
                var autoItPass = SecureStringToString(QlikAdPassword);
                _autoItSpecialChars.ForEach(s => { autoItPass = autoItPass.Replace(s, "{" + s + "}"); });
                return autoItPass;
            }
        }

        static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}