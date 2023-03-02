using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace HttpClientPostLargeFileInBackgroundService.Errors
{
    public class HttpErrorStatusCodeException : HttpRequestException
    {
        public HttpErrorStatusCodeException(HttpStatusCode errorStatusCode)
        {
            ErrorStatusCode = errorStatusCode;
        }
        public HttpStatusCode ErrorStatusCode { get; set; }
    }
}