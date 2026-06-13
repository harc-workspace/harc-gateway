# HARC Gateway 🚀

## Proje Özeti

**HARC Gateway**, mikro servisler mimarisinde API trafiğini yönetmek ve koordine etmek için tasarlanmış bir **API Gateway** uygulamasıdır. Frontend uygulamalarından gelen istekleri alarak, ilgili backend servislerine yönlendiren bir **Reverse Proxy** olarak çalışır.

- **Tür**: API Gateway / Reverse Proxy
- **.NET Sürümü**: .NET 10.0
- **Ana Kütüphane**: YARP (Yet Another Reverse Proxy) v2.3.0
- **Durum**: Aktif Geliştirme

---

## 🏗️ Mimarisi

### Genel Yapı

```
Frontend (React)
    ↓
HARC Gateway (Port 5000/5001)
    ↓
Backend API Cluster (Port 5100)
```

### Bileşenler

1. **CORS Middleware**
   - Frontend uygulamalarının gateway'den geçişini sağlar
   - Desteklenen Origins: `http://localhost:3000`, `http://localhost:5173`

2. **YARP Reverse Proxy**
   - Gelen istekleri arka uç servislere iletir
   - Path-based routing yapılandırması
   - Otomatik header yönetimi

3. **Forwarded Headers Middleware**
   - X-Forwarded-For ve X-Forwarded-Proto header'larını işler
   - Zero Trust güvenlik modeline uyumlu
   - Gerçek istemci IP'sinin korunmasını sağlar

---

## 🛠️ Teknoloji Stack'i

| Katman | Teknoloji |
|--------|-----------|
| **Framework** | ASP.NET Core 10.0 |
| **Reverse Proxy** | YARP 2.3.0 |
| **Language** | C# |
| **Target Framework** | net10.0 |
| **Runtime** | .NET 10.0 |

### Proje Konfigürasyonu

```xml
<TargetFramework>net10.0</TargetFramework>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
<RootNamespace>_</RootNamespace>
```

---

## 📋 Routing Konfigürasyonu

### API Route

```json
{
  "Routes": {
    "api-route": {
      "ClusterId": "backend-cluster",
      "AuthorizationPolicy": "Anonymous",
      "CorsPolicy": "GatewayCorsPolicy",
      "Match": {
        "Path": "/api/{**catch-all}"
      }
    }
  }
}
```

**Detaylar**:
- Tüm `/api/**` istekleri backend cluster'ına yönlendirilir
- CORS politikası uygulanır
- Anonymous erişime izin verilir

### Backend Cluster

```json
{
  "Clusters": {
    "backend-cluster": {
      "Destinations": {
        "backend-destination": {
          "Address": "http://localhost:5100"
        }
      }
    }
  }
}
```

**Detaylar**:
- Backend API: `http://localhost:5100`
- Tek destination ile basit cluster yapılandırması
- Geliştirilmeye uygun (load balancing eklenebilir)

---

## 🚀 Başlangıç

### Ön Koşullar

- .NET 10.0 SDK
- Visual Studio Code (önerilen) veya Visual Studio 2022+
- Port 5000/5001 ve 5100 kullanılabilir olmalı

### Kurulum

1. **Depoyu klonlayın**
```bash
git clone <repository-url>
cd harc-gateway
```

2. **Bağımlılıkları yükleyin**
```bash
dotnet restore
```

3. **Uygulamayı çalıştırın**
```bash
dotnet run
```

**Çıkış**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

### Geliştirme Ortamında Çalıştırma

```bash
dotnet run --environment Development
```

**Port'lar**:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

---

## 🔧 Konfigürasyon

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ReverseProxy": {
    "Routes": { /* ... */ },
    "Clusters": { /* ... */ }
  }
}
```

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 🔌 API Endpoints

### Gateway Endpoint'leri

| Path | Yönlendirme Hedefi | Açıklama |
|------|-------------------|---------|
| `/api/*` | Backend Cluster (Port 5100) | Tüm API istekleri |

**Örnek İstek**:
```bash
curl http://localhost:5000/api/users
# → http://localhost:5100/api/users
```

---

## 🔐 Güvenlik Özellikleri

### CORS Politikası

```csharp
policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
```

**Özellikler**:
- ✅ Kimliklendirilmiş isteklere izin verilir (AllowCredentials)
- ✅ Tüm HTTP metotları desteklenir
- ✅ Özel header'lar gönderilir
- ✅ Frontend portları belirtilmiş

### Forwarded Headers

```csharp
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                      ForwardedHeaders.XForwardedProto
});
```

**Faydaları**:
- Gerçek istemci IP'si korunur
- Protokol bilgisi (HTTP/HTTPS) iletilir
- Proxy arkasında çalışmaya uygun

---

## 📁 Proje Yapısı

```
harc-gateway/
├── Program.cs                      # Ana uygulama giriş noktası
├── harc-gateway.csproj             # Proje dosyası
├── appsettings.json                # Varsayılan konfigürasyon
├── appsettings.Development.json    # Geliştirme konfigürasyonu
├── Properties/
│   └── launchSettings.json         # Çalıştırma ayarları
├── bin/                            # Derlenmiş çıktılar
├── obj/                            # Ara derleme dosyaları
├── .git/                           # Git deposu
├── .gitignore                      # Git ayarları
└── README.md                       # Bu dosya
```

---

## 📊 Mimarisi Detaylı Açıklaması

### Request Flow (İstek Akışı)

```
1. Frontend → HARC Gateway
   ↓
2. CORS Kontrol
   ↓
3. Forwarded Headers Middleware
   ↓
4. YARP Reverse Proxy
   ↓
5. Backend API Services
   ↓
6. Response ← Backend
   ↓
7. Response ← HARC Gateway
   ↓
8. Response → Frontend
```

### Middleware Sırası

1. **ForwardedHeaders**: X-Forwarded-* header'larını işler
2. **CORS**: Cross-Origin isteklerine izin verir
3. **ReverseProxy**: YARP routing'i yapılandırır

⚠️ **Önemli**: Middleware sırası kritiktir!

---

## 🧪 Test Etme

### Health Check

```bash
curl http://localhost:5000/
# 404 dönebilir (normal, healthcheck yoktur)
```

### API Endpoint Test

```bash
# Frontend (localhost:3000) tarafından
fetch('http://localhost:5000/api/users')
  .then(response => response.json())
  .then(data => console.log(data));
```

### cURL ile Test

```bash
curl -X GET http://localhost:5000/api/endpoint
```

---

## 🔄 Geliştirme Üzerine

### Hot Reload

```bash
dotnet watch run
```

### Debug Modu

Visual Studio Code veya Visual Studio ile debug etmek için `.vscode/launch.json` yapılandırması:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net10.0/harc-gateway.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "serverReadyAction": {
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)",
        "uriFormat": "{0}",
        "action": "openExternally"
      }
    }
  ]
}
```

---

## 📈 İleri Özellikler & Geliştirme Fikirleri

- [ ] **Load Balancing**: Birden fazla backend endpoint'i
- [ ] **Rate Limiting**: İstek sayısını sınırlamak
- [ ] **Authentication/Authorization**: JWT token doğrulaması
- [ ] **Logging & Monitoring**: Centralized logging
- [ ] **Health Checks**: Backend servislerinin sağlık kontrolü
- [ ] **Request/Response Transformation**: İstek/cevap dönüştürme
- [ ] **Circuit Breaker**: Hata toleransı
- [ ] **API Versioning**: Sürüm yönetimi
- [ ] **GraphQL Support**: GraphQL endpoint'leri
- [ ] **WebSocket Support**: Real-time iletişim

---

## 🐛 Troubleshooting

### CORS Hatası

**Hata**: `Access to XMLHttpRequest blocked by CORS policy`

**Çözüm**:
```csharp
// appsettings.json'da frontend portları kontrol edin
"http://localhost:3000" // React
"http://localhost:5173" // Vite
```

### 404 Hatası

**Hata**: `/api/endpoint` 404 döndürüyor

**Çözüm**:
- Backend API'nin çalışıp çalışmadığını kontrol edin (Port 5100)
- Routing konfigürasyonunu kontrol edin
- Backend log'larını inceleyin

### Port Çakışması

**Hata**: `Address already in use`

**Çözüm**:
```bash
# Kullanılan port'u bulun ve kapatın
netstat -ano | findstr :5000  # Windows
# veya
lsof -i :5000  # macOS/Linux
```

---

## 📚 Kaynaklar

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [CORS Documentation](https://learn.microsoft.com/aspnet/core/security/cors)
- [Forwarded Headers Middleware](https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer)

---

## 📝 Lisans

Bu proje [HARC Ekibi](https://github.com) tarafından geliştirilmektedir.

---

## 👥 Katkıda Bulunma

Katkıda bulunmak için:

1. Bu repository'yi fork edin
2. Özellik dalı oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Dalınıza push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın

---

## 📧 İletişim

Sorularınız veya önerileriniz için lütfen bir Issue açınız.

**Son Güncelleme**: 2026-06-10
