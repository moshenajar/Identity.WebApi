using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Service.Communication
{
    public abstract class BaseResponse
    {
        public bool Succeeded { get; protected set; }
        public string Message { get; protected set; }
        public int ResultCode { get; protected set; }

        public BaseResponse(bool succeeded, string message, int resultCode)
        {
            Succeeded = succeeded;
            Message = message;
            ResultCode = resultCode;
        }
    }
}
