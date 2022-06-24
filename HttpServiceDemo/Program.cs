using System.Diagnostics;

var s = HttpService.HttpServer.Instance;

s.ListenTo("http://localhost:8080/");
s.Start();

s.Route(route:"/", callback:(s,e) =>
{
    e.HtmlResponse("<h1>Hello World</h1>");
});

s.Route(route: "/api/client/{ClientId}/item/{ItemId}/", callback: (s, e) =>
{
    e.HtmlResponse($"<h1>ClientId: { e.Get("ClientId") }</h1><h2>ItemId: { e.Get("ItemId") }</h2>");
});

// Starts WebBrowser for demo purposes
Process.Start("explorer.exe", "http://localhost:8080");
Process.Start("explorer.exe", "http://localhost:8080/api/client/0001/item/Potato");