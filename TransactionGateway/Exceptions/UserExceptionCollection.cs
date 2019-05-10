using System.Collections.Generic;

namespace TransactionGateway.Exceptions
{
    public class UserExceptionCollection : UserException
    {
        public List<UserException> Exceptions = new List<UserException>();
    }
}