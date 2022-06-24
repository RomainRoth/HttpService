using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpService
{
    /// <summary>
    /// Management of Static Files
    /// </summary>
    public partial class HttpServer
    {
        private bool allowStaticFileServing = true;
        /// <summary>
        /// Enable/Disable static file serving
        /// </summary>
        public bool AllowStaticFileServing { get => allowStaticFileServing; set => allowStaticFileServing = value; }
        
        private string staticFileDirectory = Directory.GetCurrentDirectory();
        /// <summary>
        /// Root path where to look for files
        /// </summary>
        public string StaticFileDirectory { get => staticFileDirectory; set => staticFileDirectory = value; }

        // Mime Types
        private Dictionary<string, string> mimeTypes = new Dictionary<string, string>()
        {
            { ".aac", "audio/aac" },
            { ".abw", "application/x-abiword" },
            { ".arc", "application/octet-stream" },
            { ".avi", "video/x-msvideo" },
            { ".azw", "application/vnd.amazon.ebook" },
            { ".bin", "application/octet-stream" },
            { ".bmp", "image/bmp" },
            { ".bz", "application/x-bzip" },
            { ".bz2", "application/x-bzip2" },
            { ".csh", "application/x-csh" },
            { ".css", "text/css" },
            { ".csv", "text/csv" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".eot", "application/vnd.ms-fontobject" },
            { ".epub", "application/epub+zip" },
            { ".gif", "image/gif" },
            { ".htm .html", "text/html" },
            { ".ico", "image/x-icon" },
            { ".ics", "text/calendar" },
            { ".jar", "application/java-archive" },
            { ".jpeg .jpg", "image/jpeg" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".mid .midi", "audio/midi" },
            { ".mpeg", "video/mpeg" },
            { ".mpkg", "application/vnd.apple.installer+xml" },
            { ".odp", "application/vnd.oasis.opendocument.presentation" },
            { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
            { ".odt", "application/vnd.oasis.opendocument.text" },
            { ".oga", "audio/ogg" },
            { ".ogv", "video/ogg" },
            { ".ogx", "application/ogg" },
            { ".otf", "font/otf" },
            { ".png", "image/png" },
            { ".pdf", "application/pdf" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".rar", "application/x-rar-compressed" },
            { ".rtf", "application/rtf" },
            { ".sh", "application/x-sh" },
            { ".svg", "image/svg+xml" },
            { ".swf", "application/x-shockwave-flash" },
            { ".tar", "application/x-tar" },
            { ".tif .tiff", "image/tiff" },
            { ".ts", "application/typescript" },
            { ".ttf", "font/ttf" },
            { ".vsd", "application/vnd.visio" },
            { ".wav", "audio/x-wav" },
            { ".weba", "audio/webm" },
            { ".webm", "video/webm" },
            { ".webp", "image/webp" },
            { ".woff", "font/woff" },
            { ".woff2", "font/woff2" },
            { ".xhtml", "application/xhtml+xml" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".xml", "application/xml" },
            { ".xul", "application/vnd.mozilla.xul+xml" },
            { ".zip", "application/zip" },
            { ".3gp", "video/3gpp audio/3gpp dans le cas où le conteneur ne comprend pas de vidéo" },
            { ".3g2", "video/3gpp2 audio/3gpp2 dans le cas où le conteneur ne comprend pas de vidéo" },
            { ".7z", "application/x-7z-compressed" }
        };

        /// <summary>
        /// Prevents Web Server from serving static files with this extension
        /// </summary>
        /// <param name="extension">File extension (eg: ".png", ".gif")</param>
        public void DisableFileExtension(string extension)
        {
            mimeTypes.Remove(extension);
        }

        /// <summary>
        /// Allows Web Server to serve static files with this extension
        /// </summary>
        /// <param name="extension">File extension (eg: ".png", ".gif")</param>
        /// <param name="mime">MIME Type (eg: "image/png")</param>
        public void EnableFileExtension(string extension, string mime)
        {
            mimeTypes.Add(extension, mime);
        }

        /// <summary>
        /// Disable all existing file extensions
        /// </summary>
        public void DisableAllFileExtensions()
        {
            mimeTypes.Clear();
        }

        public bool ServeStaticFile(string file, HttpListenerResponse response)
        {
            // Check if serving is enabled
            if(!allowStaticFileServing)
                return false;

            // Chroot + Check
            string filePath = Path.GetFullPath(staticFileDirectory + file);
            if(!filePath.StartsWith(staticFileDirectory))
                return false;

            // Check extension
            string extension = Path.GetExtension(filePath);
            if(!mimeTypes.ContainsKey(extension))
                return false;

            // File exists ?
            if(!File.Exists(filePath))
                return false;

            byte[] data = File.ReadAllBytes(filePath);
            response.ContentType = mimeTypes.GetValueOrDefault(extension);
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;
            response.OutputStream.WriteAsync(data, 0, data.Length);
            response.Close();
            return true;
        }
    }
}
