using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Helpers
{
    public static class CustomResponse
    {
        public static dynamic CreateResponse(bool result, string message, dynamic data, int errorCode)
        {
            return new { result, message, data, errorCode };
        }
    }
}
