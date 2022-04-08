using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.Domain.Exceptions
{
    public enum ErrorCode
    {
        ApplicationNotFound,
        MessageNotFound
    }

    public static class ErrorCodeExtension
    {
        public static string ToErrorString(this ErrorCode errorCode) =>
            errorCode switch
            {
                ErrorCode.ApplicationNotFound => "application_not_found",
                ErrorCode.MessageNotFound => "message_not_found",
                _ => "unknown_error"
            };
    }
}
