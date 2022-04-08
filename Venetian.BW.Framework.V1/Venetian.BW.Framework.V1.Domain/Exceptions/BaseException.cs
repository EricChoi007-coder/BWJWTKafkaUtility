using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.Domain.Exceptions
{
    public class BaseException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public BaseException(ErrorCode errorCode, string message = null, 
            Exception e = null) : base(message, e)
        {
            this.ErrorCode = errorCode;
        }
    }
}
