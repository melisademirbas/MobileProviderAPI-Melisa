https://github.com/melisademirbas/MobileProviderAPI-Melisa

# Mobile Provider API Gateway

Bu proje, mobil operatör fatura ödeme sistemi için bir API Gateway uygulamasıdır. .NET 8.0 ile geliştirilmiştir ve Azure SQL Database kullanmaktadır.

## Özellikler

- **API Gateway**: Tüm API endpoint'leri tek bir gateway üzerinden yönetilir
- **JWT Authentication**: Güvenli token tabanlı kimlik doğrulama
- **Rate Limiting**: API çağrılarını sınırlandırma (subscriber bazlı ve genel)
- **Azure SQL Database**: Bulut tabanlı veritabanı desteği
- **Swagger/OpenAPI**: API dokümantasyonu

## Proje Yapısı

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


 ER Diyagram (ASCII):

+-------------+ 1 <----> * +--------+ 1 <----> * +---------+
| Subscriber |-------------------| Bill |-------------------| Payment |
+-------------+ +--------+ +---------+
| SubscriberID|                 | BillId |                   |PaymentId|
| SubscriberNo|                 |SubscriberId |              | BillId(FK)
| Name |                       | SubscriberNo|                | SubscriberId(FK)
| Email|                       | TotalAmount |                | SubscriberNo|
                                | PaidAmount                  | Amount|
+-------------+                  | Created At |               | Status | 
                                                             | PaymentDate

+--------+ +---------+

AdminUser (1) --- manages --> Bill (n)

RateLimit (per day) references SubscriberNo
AuditLog logs requests/responses

## Kurulum

### Gereksinimler
- .NET 8.0 SDK
- Azure SQL Database (veya LocalDB geliştirme için)


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

cd MobileProviderAPI.Gateway
dotnet run
```


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



## Örnek CSV Format (Batch Add Bill)

```csv
SubscriberNo,Month,TotalAmount
1234567890,2024-01,150.50
1234567890,2024-02,175.00
9876543210,2024-01,200.00
```

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


