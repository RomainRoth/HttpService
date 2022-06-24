using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpService
{
    public class HttpRouterEventArgs : EventArgs
    {
        private HttpListenerResponse response;
        private Dictionary<string, string> getVars;

        public HttpRouterEventArgs(HttpListenerResponse response, Dictionary<string, string> getVars)
        {
            this.response = response;
            this.getVars = getVars;
        }

        /// <summary>
        /// Send back HTML code to client
        /// </summary>
        /// <param name="html"></param>
        public void HtmlResponse(string html)
        {
            byte[] data = Encoding.UTF8.GetBytes(String.Format(html));
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;
            response.OutputStream.WriteAsync(data, 0, data.Length);
            response.Close();
        }

        /// <summary>
        /// Finds GET parameters
        /// </summary>
        /// <param name="key">Key asked</param>
        /// <returns>Value sent</returns>
        public string Get(string key) => getVars.GetValueOrDefault(key)!;
    }
}
