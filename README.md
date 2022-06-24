# HttpService

HttpService is a lightweight web server made to be embedded into your C# projects.
It draws inspiration from the Symfony Framework's syntax making it easy to pick up.

## Usage

```csharp
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
```

Add a reverse proxy such as Apache or NGINX in front to add SSL and you're good to go.

## Roadmap

- [x] Http Server
- [x] Routing
- [ ] Static Files
- [ ] Json Helper
- [ ] Template Engine