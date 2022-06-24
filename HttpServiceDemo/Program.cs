using System.Diagnostics;

var s = HttpService.HttpServer.Instance;

// Start listening to localhost on port 8080
s.ListenTo("http://localhost:8080/");
s.Start();

// Force web server to check for static files in the public directory
s.StaticFileDirectory += "\\public";

s.Route(route:"/", callback:(s,e) =>
{
    string html =
    @"<!DOCTYPE>
        <html>
            <head>
                <title>HttpServiceDemo</title>
                <link rel='icon' type='image/ico' href='ico/favicon.ico' />
            </head>
            <body>
                <h1>Hello World</h1>
                <h2>Static File Demo</h2>
                <img src='/img/GrafxKid_CC0.png' />
                <br>
                <a href='https://opengameart.org/content/platformer-baddies'>[Platformer Baddies] by [GrafxKid] licenced under [CC0] [2015]</a>
            </body>
        </html>
    ";
    e.HtmlResponse(html);
});

s.Route(route: "/api/client/{ClientId}/item/{ItemId}/", callback: (s, e) =>
{
    e.HtmlResponse($"<h1>ClientId: { e.Get("ClientId") }</h1><h2>ItemId: { e.Get("ItemId") }</h2>");
});

// Starts WebBrowser for demo purposes
Process.Start("explorer.exe", "http://localhost:8080");
Process.Start("explorer.exe", "http://localhost:8080/api/client/0001/item/Potato");