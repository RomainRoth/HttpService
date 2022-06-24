using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpService
{
    /// <summary>
    /// Event wrapper to store used to store delegates into a dictionary
    /// </summary>
    internal class Listener
    {
        public event EventHandler<HttpRouterEventArgs>? Event;

        public void BindCallback(EventHandler<HttpRouterEventArgs> eventHandler)
        {
            Event += eventHandler;
        }

        public virtual void OnEvent(Object sender, HttpRouterEventArgs e)
        {
            EventHandler<HttpRouterEventArgs>? handler = Event;
            handler?.Invoke(sender, e);
        }
    }

    internal class HttpRouter
    {
        // Singleton template
        private static readonly Lazy<HttpRouter> service = new Lazy<HttpRouter>(() => new HttpRouter());
        public static HttpRouter Service => service.Value;

        // Dictionary used to store Event System
        private Dictionary<string, Listener> events = new Dictionary<string, Listener>();
        // Dictionary used to store Parameters
        private Dictionary<string, string[]> parametersSchemas = new Dictionary<string, string[]>();

        /// <summary>
        /// Register a new route
        /// </summary>
        /// <param name="route">Web path (eg. "/home")</param>
        /// <param name="methods">HTTP Method (eg. "GET,POST,PUT,HEAD...")</param>
        /// <param name="callback">Response</param>
        public void Register(string route, string methods, EventHandler<HttpRouterEventArgs> callback)
        {
            string key = "/";
            string trueKey = "";
            List<string> parameters = new List<string>();

            string[] bits = route.Split("/", StringSplitOptions.RemoveEmptyEntries);

            // Separate route path and parameters
            foreach (string bit in bits)
            {
                var keyPart = bit;

                if (bit.StartsWith("{"))
                {
                    parameters.Add(bit.Replace("{", "").Replace("}", ""));
                    keyPart = "$";
                }

                key += keyPart + "/";
            }

            // Foreach Method (GET, POST, PUT...) available, bind an event
            foreach (string method in methods.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                // Exemple "GET/home/"
                // Or even "GET/api/client/$/item/$/"
                trueKey = method.Trim().ToUpper() + key;

                // ParametersSchemas Binding
                if (!parametersSchemas.ContainsKey(trueKey))
                {
                    parametersSchemas.Add(trueKey, parameters.ToArray());
                }

                // Events Binding
                if (!events.ContainsKey(trueKey))
                {
                    events.Add(trueKey, new Listener());
                    events.GetValueOrDefault(trueKey)?.BindCallback(callback);
                }
            }
        }

        /// <summary>
        /// Determines from context the content the server needs to return to the client
        /// </summary>
        /// <param name="context">HTTP Context</param>
        public void Dispatch(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string key = "";

            string[] bits = (request.HttpMethod + request?.Url?.AbsolutePath).Split("/", StringSplitOptions.RemoveEmptyEntries);

            // Find Matching route
            foreach(string route in events.Keys)
            {
                string[] rBits = route.Split("/", StringSplitOptions.RemoveEmptyEntries);

                bool fullFound = true;
                bool partialFound = false;

                // Compare route with request
                if(rBits.Length==bits.Length)
                {
                    for(int i=0; i<rBits.Length; i++)
                    {
                        if (rBits[i] == bits[i] || rBits[i] == "$")
                        {
                            if (rBits[i] == "$")
                            {
                                partialFound = true;
                            }
                        }
                        else
                        {
                            fullFound = false;
                            partialFound = false;
                        }
                    }
                }
                else
                {
                    fullFound = false;
                    partialFound = false;
                }

                // If we found an exact match, save it and leave
                if(fullFound)
                {
                    key = route;
                    break;
                }
                // If we found a partial match, save it only if we haven't found a match previously
                else if (partialFound && string.IsNullOrEmpty(key))
                {
                    key = route;
                }
            }

            // If we have a key, send the query
            if(!string.IsNullOrEmpty(key))
            {
                // Parse GET parameters from URI
                string[] parametersSchema = parametersSchemas.GetValueOrDefault(key)!;
                Dictionary<string, string> get = new Dictionary<string, string>();
                int pIndex = 0;
                string[] kBits = key.Split("/", StringSplitOptions.RemoveEmptyEntries);
                for(int i=0; i<kBits.Length; i++)
                {
                    if (kBits[i]=="$")
                    {
                        get.Add(parametersSchema[pIndex], bits[i]);
                        pIndex++;
                    }
                }
                // Generate Args
                HttpRouterEventArgs args = new HttpRouterEventArgs(response, get);
                // Call response
                events.GetValueOrDefault(key)?.OnEvent(request!, args);
            }
            else
            {
                HttpRouterEventArgs args = new HttpRouterEventArgs(response, new Dictionary<string, string>());
                args.HtmlResponse("<h1>404</h1>");
            }
        }
    }
}
