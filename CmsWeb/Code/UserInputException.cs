using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Code
{
    public class UserInputException : Exception
    {
    public UserInputException()
    {
    }

    public UserInputException(string message)
        : base(message)
    {
    }

    public UserInputException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
}