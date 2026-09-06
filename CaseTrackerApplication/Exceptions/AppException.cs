using System;
using System.Collections.Generic;
using System.Text;

namespace CaseTrackerApplication.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message)
            : base(message)
        {
        }
    }
}