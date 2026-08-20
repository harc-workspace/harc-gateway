---
applyTo: "**/*.cs,**/*.json"
---

# HARC Gateway Instructions

- YARP route'larını değiştirmeden önce frontend endpoint kullanımını kontrol et.
- Forwarded headers ve proxy header davranışını güvenlik açısından değerlendir.
- Backend destination değerlerini environment/configuration üzerinden yönet.
- CORS değişikliklerinde allowed origins ve credentials politikasını açıkça belirt.
