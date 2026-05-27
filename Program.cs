using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// 1. KURUMSAL CORS POLİTİKASI (React önyüzünün kapıdan geçebilmesi için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // React'ın olası portları
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 2. YARP GATEWAY SERVİSİ
// YARP varsayılan olarak X-Forwarded-* header'larını arkaya otomatik uçurur.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// 3. ZERO TRUST İÇİN HEADER FORWARDING MIDDLEWARE
// Gateway'in, gelen kullanıcının gerçek IP'sini (X-Forwarded-For) manipüle etmeden arkaya iletmesini sağlar.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// 4. CORS ve YARP KULLANIMI (Sıralama çok önemlidir!)
app.UseCors("GatewayCorsPolicy");

app.MapReverseProxy();

app.Run();