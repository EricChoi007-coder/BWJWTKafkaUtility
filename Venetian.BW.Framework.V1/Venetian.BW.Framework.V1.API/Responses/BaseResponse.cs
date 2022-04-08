using System;
using Venetian.BW.Framework.V1.Domain.Exceptions;

namespace Venetian.BW.Framework.V1.API.Responses
{
    public class BaseResponse<T>
    {
        public bool isSuccess { get; private set; }
        public string errorCode { get; private set; }
        public string errorMessage { get; private set; }
        public T data { get; private set; }

        public BaseResponse()
        {
            data = default; //? "" 0 e
            isSuccess = false;
            errorCode = null;
            errorMessage = "uninitialise";
        }

        public void SetData(T data)
        {
            this.data = data;
            isSuccess = true;
            errorCode = null;
            errorMessage = null;
        }

        public void SetError(Exception e) //e
        {
            data = default;   //??
            isSuccess = false;
            errorMessage = e.Message;
            if (e is BaseException)
            {
                var be = (BaseException)e; //
                errorCode = be.ErrorCode.ToErrorString(); //=>??
            }

        }
    }
}
