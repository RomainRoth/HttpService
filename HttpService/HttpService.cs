using System.Net;
using System.Text;

namespace HttpService
{
    public class HttpServer
    {
        // Singleton
        private static readonly Lazy<HttpServer> instance = new Lazy<HttpServer>(() => new HttpServer());
        public static HttpServer Instance => instance.Value;

        // Port Listener
        private HttpListener httpListener = new HttpListener();

        // Was the service killed ?
        private bool forceQuit = false;

        /// <summary>
        /// Initialisation
        /// </summary>
        private HttpServer()
        {
        }

        /// <summary>
        /// Adds an Endpoint to listen to
        /// </summary>
        /// <param name="url">Endpoint (eg. http://localhost:8080)</param>
        public void ListenTo(string url)
        {
            httpListener.Prefixes.Add(url);
        }

        /// <summary>
        /// Starts the Web Server
        /// </summary>
        public void Start()
        {
            httpListener.Start();
            Thread thread = new Thread(BackgroundThread);
            thread.Start();
        }

        private void BackgroundThread()
        {
            Task listenTask = HandleIncomingConnections();
            try
            {
                listenTask.GetAwaiter().GetResult();
                httpListener.Close();
            }catch(Exception)
            {
                if(!forceQuit)
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Stop the service through force
        /// </summary>
        public void Kill()
        {
            forceQuit = true;
        }

        private async Task HandleIncomingConnections()
        {
            bool keepAlive = true;

            while (keepAlive)
            {
                // Sleeping until client connect
                HttpListenerContext context = await httpListener.GetContextAsync();
                // If client connects, dispatch the response through the router
                HttpRouter.Service.Dispatch(context);
            }
        }

        public void Route(string route, EventHandler<HttpRouterEventArgs> callback)
        {
            Route(route, "GET", callback);
        }

        public void Route(string route, string methods, EventHandler<HttpRouterEventArgs> callback)
        {
            HttpRouter.Service.Register(route, methods, callback);
        }
    }
}