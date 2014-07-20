using Moksy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moksy
{
    /// <summary>
    /// Used for manufacturing responses based on the parameters in a SimulationResponse structure. 
    /// </summary>
    internal class HttpResponseMessageFactory
    {
        /// <summary>
        /// Manufacture a new HttpResponseMessage based on the status code, content, headers and other parameters in the SimulationResponse. 
        /// </summary>
        /// <param name="response">The response. If null, a default HttpResponseMessage is returned. </param>
        /// <returns></returns>
        internal static HttpResponseMessage New(SimulationResponse response)
        {
            var result = new HttpResponseMessage();
            if (response == null) return result;

            result.StatusCode = response.HttpStatusCode;

            if (response.Content != null)
            {
                StringContent content = new StringContent(response.Content);
                result.Content = content;
            }

            if (response.ResponseHeaders != null)
            {
                foreach (var h in response.ResponseHeaders)
                {
                    result.Headers.Add(h.Name, h.Value);
                }
            }

            return result;
        }
    }
}
