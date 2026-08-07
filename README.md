# HARC Gateway

HARC Gateway, frontend’in API’ye tek bir origin üzerinden erişmesini sağlayan .NET 10 ve YARP 2.3.0 tabanlı reverse proxy uygulamasıdır.

## Görevi

- `/api/{**catch-all}` isteklerini backend API’ye iletir.
- `X-Forwarded-For` ve `X-Forwarded-Proto` header’larını işler.
- CORS policy uygular.
- `Harc.ServiceDefaults` üzerinden service discovery, resilience ve telemetry altyapısını kullanır.

Gateway authentication yapmaz. Bearer token’ı doğrulamaz; header’ı backend API’ye iletir. Kimlik doğrulama ve authorization API’nin sorumluluğundadır.

## Routing

Mevcut `appsettings.json` route’u:

```json
{
  "Path": "/api/{**catch-all}",
  "ClusterId": "backend-cluster"
}
```

Cluster destination varsayılan olarak `http://localhost:5100` adresidir. API başka portta çalışıyorsa `harc-gateway/appsettings.json` içindeki `Address` güncellenmelidir. Aspire ile çalışırken destination/service discovery bilgisi AppHost tarafından sağlanabilir.

## CORS ve forwarded headers

Kodda `SetIsOriginAllowed(origin => true)`, `AllowAnyHeader()`, `AllowAnyMethod()` ve `AllowCredentials()` kullanılır. Bu geliştirme kolaylığı sağlar ancak production için güvenli değildir; izin verilen frontend origin’leri açıkça tanımlanmalıdır.

Middleware sırası:

1. `UseForwardedHeaders`
2. `UseCors("GatewayCorsPolicy")`
3. `MapReverseProxy`

## Çalıştırma

```bash
dotnet restore
dotnet run
```

Gerçek portlar `Properties/launchSettings.json` içindedir. Standalone kullanımda gateway’in backend API ile aynı route ve destination ayarlarına sahip olduğundan emin olun.

Tüm sistemi birlikte başlatmak için:

```bash
cd ../harc-aspire-host
dotnet run --project Harc.AppHost
```

Aspire kullanımında sabit portlar yerine dashboard endpoint’lerini esas alın.

## Sağlık kontrolleri

`Harc.ServiceDefaults` `/health` ve `/alive` mapping’i sağlayan `MapDefaultEndpoints()` extension’ını içerir. Ancak mevcut gateway pipeline’ında bu extension çağrılmamaktadır. Bu nedenle health endpoint’lerinin çalıştığı varsayılmamalıdır; etkinleştirmek için uygulama pipeline’ına bilinçli olarak eklenmelidir.

## Sınırlamalar

- Tek backend destination vardır; load balancing yapılandırılmamıştır.
- Gateway’de authentication, rate limiting veya API versioning yoktur.
- CORS wildcard davranışındadır.
- Backend health check’leri proxy seviyesinde kontrol edilmez.

Ayrıntılı sistem bilgisi için [kök AI_PROJECT_GUIDE.md](../AI_PROJECT_GUIDE.md) dosyasına bakın.
