using System;

namespace TransactionGateway.Exceptions
{
    public class UserException : Exception
    {
        public UserException() : base() { }
        public UserException(string msg) : base(msg) { }
    }
}
