# HARC Gateway Instructions

Bu repository HARC'ın YARP tabanlı ASP.NET Core reverse proxy'sidir. Ortak kurallar `.github/instructions/` altındaki coding, architecture, security ve documentation dosyalarındadır.

## Proje kuralları

- `/api/{**catch-all}` route sözleşmesini ve backend destination yapılandırmasını koru.
- JWT doğrulamasını gateway'e taşımadan API authentication akışını koru.
- `UseForwardedHeaders`, service discovery ve resilience yapılandırmalarını değiştirirken runtime akışını kontrol et.
- CORS wildcard + credentials davranışının production riski olduğunu dikkate al.
- Secret ve gerçek backend credential'larını configuration'a gömme.
