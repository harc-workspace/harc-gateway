var builder = WebApplication.CreateBuilder(args);

// YARP Servisini ekliyoruz
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// YARP Ara katmanını (Middleware) aktif ediyoruz
app.MapReverseProxy();

app.Run();