using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class FailedVerificationException : Exception
    {
        public FailedVerificationException() : base()
        {
        }
        public FailedVerificationException(string message) : base(message)
        {
        }
    }
}
