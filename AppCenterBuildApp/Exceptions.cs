using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AppCenterBuildApp
{
    /// <summary>
    /// Invalid input parameters exception
    /// </summary>
    internal class InputDataParameterException : Exception
    {
        internal InputDataParameterException(string parameterName, Exception innerException) : base($"Failed to read input parameter '{parameterName}'!", innerException) { }
    }

    internal class AppCenterApiResponseError : Exception
    {
        /// <summary>
        /// Error code
        /// </summary>
        public HttpStatusCode ErrorCode { get; private set; }
        /// <summary>
        /// API endpoint 
        /// </summary>
        public string Endpoint { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        internal AppCenterApiResponseError(string endpoint, HttpResponseMessage response) : base($"Failed to perform App Center API {response.RequestMessage.Method.Method} request {endpoint} with error '{response.ReasonPhrase}'. StatusCode: {response.StatusCode}")
        {
            this.ErrorCode = response.StatusCode;
            this.Endpoint = endpoint;
        }
    }
}
