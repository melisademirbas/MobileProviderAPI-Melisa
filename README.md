# Mobile Provider API Gateway

Bu proje, mobil operatör fatura ödeme sistemi için bir API Gateway uygulamasıdır. .NET 8.0 ile geliştirilmiştir ve Azure SQL Database kullanmaktadır.

## Özellikler

- **API Gateway**: Tüm API endpoint'leri tek bir gateway üzerinden yönetilir
- **JWT Authentication**: Güvenli token tabanlı kimlik doğrulama
- **Rate Limiting**: API çağrılarını sınırlandırma (subscriber bazlı ve genel)
- **Azure SQL Database**: Bulut tabanlı veritabanı desteği
- **Swagger/OpenAPI**: API dokümantasyonu

## Proje Yapısı

```
MobileProviderAPI/
├── MobileProviderAPI.Gateway/     # API Gateway ve Controllers
├── MobileProviderAPI.Services/    # Business Logic
├── MobileProviderAPI.Data/        # Data Access Layer (EF Core)
├── MobileProviderAPI.Models/      # Data Models ve DTOs
└── MobileProviderAPI.Common/      # JWT Service ve ortak utilities
```

## API Endpoints

### Authentication
- `POST /api/authentication/login` - JWT token almak için login

### Mobile Provider App
- `GET /api/mobileapp/querybill?subscriberNo={no}&month={month}` - Fatura sorgula (Auth: YES, Rate Limit: 3/subscriber/day)
- `GET /api/mobileapp/querybilldetailed?subscriberNo={no}&month={month}&pageNumber={page}&pageSize={size}` - Detaylı fatura sorgula (Auth: YES, Paging: YES)

### Banking App
- `GET /api/bankingapp/querybill?subscriberNo={no}` - Ödenmemiş faturaları sorgula (Auth: YES)

### Web Site
- `POST /api/website/paybill` - Fatura öde (Auth: NO)

### Admin
- `POST /api/admin/addbill` - Fatura ekle (Auth: YES, Role: Admin)
- `POST /api/admin/addbillbatch` - CSV dosyasından toplu fatura ekle (Auth: YES, Role: Admin)

## Kurulum

### Gereksinimler
- .NET 8.0 SDK
- Azure SQL Database (veya LocalDB geliştirme için)

### 1. Projeyi Klonlayın
```bash
git clone <repository-url>
cd melisa_mobilproviderapi
```

### 2. NuGet Paketlerini Yükleyin
```bash
dotnet restore
```

### 3. Veritabanı Bağlantı String'ini Yapılandırın

`MobileProviderAPI.Gateway/appsettings.json` dosyasında Azure SQL connection string'i güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=MobileProviderDB;..."
  }
}
```

### 4. Veritabanını Oluşturun

Uygulama ilk çalıştırıldığında veritabanı otomatik olarak oluşturulur (`EnsureCreated`). Production için migration kullanın:

```bash
cd MobileProviderAPI.Data
dotnet ef migrations add InitialCreate --startup-project ../MobileProviderAPI.Gateway
dotnet ef database update --startup-project ../MobileProviderAPI.Gateway
```

### 5. Uygulamayı Çalıştırın
```bash
cd MobileProviderAPI.Gateway
dotnet run
```

Uygulama `https://localhost:5001` (veya `http://localhost:5000`) adresinde çalışacaktır.

Swagger UI: `https://localhost:5001/swagger`

## Authentication

### Login
```bash
POST /api/authentication/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

### API Çağrılarında Token Kullanımı
```bash
GET /api/mobileapp/querybill?subscriberNo=1234567890&month=2024-01
Authorization: Bearer {token}
```

## Varsayılan Kullanıcılar

- **admin** / admin123 (Admin rolü)
- **mobileapp** / mobile123 (MobileApp rolü)
- **bankingapp** / banking123 (BankingApp rolü)

## Rate Limiting

- **Genel Rate Limit**: 100 istek/dakika (tüm endpoint'ler için)
- **Query Bill (Mobile App)**: 3 istek/subscriber/gün

## Azure'a Deploy Etme

### 1. Azure SQL Database Oluşturma

1. Azure Portal'da SQL Database oluşturun
2. Connection string'i kopyalayın
3. Firewall kurallarını yapılandırın (Azure servislerinden erişime izin verin)

### 2. Azure App Service Oluşturma

1. Azure Portal'da App Service oluşturun
2. .NET 8.0 runtime stack seçin
3. Connection string'i App Service Configuration'a ekleyin:
   - Name: `DefaultConnection`
   - Value: Azure SQL connection string

### 3. Deployment

#### Visual Studio ile:
1. Projeye sağ tıklayın → Publish
2. Azure App Service seçin
3. Mevcut App Service'i seçin veya yeni oluşturun

#### Azure CLI ile:
```bash
az webapp deployment source config-zip \
  --resource-group <resource-group> \
  --name <app-name> \
  --src <path-to-zip>
```

#### GitHub Actions ile:
`.github/workflows/azure-deploy.yml` dosyası oluşturun (örnek aşağıda)

## Örnek CSV Format (Batch Add Bill)

```csv
SubscriberNo,Month,TotalAmount
1234567890,2024-01,150.50
1234567890,2024-02,175.00
9876543210,2024-01,200.00
```

## Test

### Postman Collection
Postman collection'ı import ederek tüm endpoint'leri test edebilirsiniz.

### cURL Örnekleri

**Login:**
```bash
curl -X POST https://your-api.azurewebsites.net/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

**Query Bill (Mobile App):**
```bash
curl -X GET "https://your-api.azurewebsites.net/api/mobileapp/querybill?subscriberNo=1234567890&month=2024-01" \
  -H "Authorization: Bearer {token}"
```


