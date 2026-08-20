# HARC Gateway Agent Instructions

Bu repository'de çalışırken `.github/copilot-instructions.md` ve `.github/instructions/` altındaki ortak talimatları uygula.

- YARP reverse proxy route ve destination sözleşmesini koru.
- Gateway'in `/api/{**catch-all}` akışında JWT doğrulaması yapmadığını unutma; authentication API sorumluluğudur.
- Forwarded headers, service discovery ve CORS değişikliklerinin production etkisini kontrol et.
- Doğrulama sonrası `dotnet build harc-gateway.csproj` çalıştır.
