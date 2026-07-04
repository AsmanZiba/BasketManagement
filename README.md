# 🛒 سامانه مدیریت سبد خرید (Basket Service)

## 📋 معرفی پروژه
یک سامانه مدیریت سبد خرید با معماری **Vertical Slice Architecture** و **CQRS** که با استفاده از **MediatR**، **FluentValidation**، **Entity Framework Core** و **Redis/IMemoryCache** پیاده‌سازی شده است.


## ✨ قابلیت‌ها
- ✅ افزودن/ویرایش/حذف آیتم از سبد خرید
- ✅ پاک کردن کامل سبد خرید
- ✅ انقضای خودکار سبدهای غیرفعال (هر ۱ دقیقه)
- ✅ کش کردن سبد خرید برای افزایش کارایی
- ✅ رویدادمحور (Event-Driven) با انتشار رویدادهای یکپارچه‌سازی
- ✅ اعتبارسنجی ورودی‌ها با FluentValidation
- ✅ مدیریت تراکنش خودکار با TransactionBehavior
- ✅ سرویس پس‌زمینه برای انقضای سبدها
- ✅ تست‌پذیری بالا با Moq


## 🏗️ معماری پروژه
```
BasketManagement/
├── 📁 BasketManagement.Domain/          # لایه دامنه (Entities, Enums, Events)
├── 📁 BasketManagement.Application/     # لایه اپلیکیشن (Features, Behaviors, Contracts)
├── 📁 BasketManagement.Infrastructure/  # لایه زیرساخت (Persistence, Cache, Messaging)
└── 📁 BasketManagement.Api/             # لایه Presentation (Controllers, Program.cs)
```

### الگوهای معماری استفاده شده:
- Vertical Slice Architecture
- CQRS (Command Query Responsibility Segregation)
- Event-Driven Architecture
- Repository Pattern
- Unit of Work Pattern


## 🚀 تکنولوژی‌های استفاده شده

| تکنولوژی | کاربرد |
| :--- | :--- |
| .NET 8 | پلتفرم اصلی |
| Entity Framework Core | ORM و دسترسی به دیتابیس |
| MediatR | پیاده‌سازی CQRS و Pipeline Behaviors |
| FluentValidation | اعتبارسنجی ورودی‌ها |
| IMemoryCache / Redis | کش کردن داده‌ها |
| RabbitMQ | پیام‌رسانی و رویدادها |
| Scalar.AspNetCore | مستندسازی API |
| xUnit + Moq | تست‌نویسی |


## 📦 نصب و راه‌اندازی

### پیش‌نیازها
- .NET 8 SDK
- SQL Server (یا هر دیتابیس دیگر)
- RabbitMQ (برای رویدادها)
- Redis (cache)

### مراحل نصب

#### 1. تنظیم Connection String
فایل appsettings.json را ویرایش کنید:

```json
{
   "ConnectionStrings": {
   "DefaultConnection": "Server=localhost,1433;Database=SimagarBasket;User Id=sa;Password=***;TrustServerCertificate=true;",
   "Redis": "localhost:6379"
 },
 "RabbitMQ": {
   "Host": "localhost",
   "Port": 5672,
   "UserName": "guest",
   "Password": "guest",
   "ExchangeName": "basket_events",
   "QueueName": "basket_expiration_queue",
   "TimerIntervalMinutes": 1,
   "ExpirationMinutes": 30
 },
}
```
#### ۳. اجرای Migration
```
dotnet ef database update -p BasketManagement.Infrastructure -s BasketManagement.Api
```

#### ۴. اجرای پروژه
```
cd BasketManagement.Api
dotnet run
```

#### ۵. مشاهده مستندات API
- Scalar UI: https://localhost:5001/scalar


## 🎯 ساختار Features (Vertical Slices)

هر ویژگی به صورت یک Slice مجزا پیاده‌سازی شده است:
```
Features/
├── 📁 AddItemToBasket/
│   ├── AddItemToBasketCommand.cs
│   ├── AddItemToBasketValidator.cs
│   └── AddItemToBasketHandler.cs
├── 📁 UpdateBasketItemQuantity/
│   ├── UpdateBasketItemQuantityCommand.cs
│   ├── UpdateBasketItemQuantityValidator.cs
│   └── UpdateBasketItemQuantityHandler.cs
├── 📁 RemoveBasketItem/
│   ├── RemoveBasketItemCommand.cs
│   └── RemoveBasketItemHandler.cs
├── 📁 ClearBasket/
│   ├── ClearBasketCommand.cs
│   └── ClearBasketHandler.cs
├── 📁 ExpireBaskets/
│   ├── ExpireBasketsCommand.cs
│   └── ExpireBasketsHandler.cs
└── 📁 GetBasket/
    ├── GetBasketQuery.cs
    └── GetBasketHandler.cs
```

## 📡 API Endpoints

| متد | مسیر | توضیح |
| :--- | :--- | :--- |
| GET | /api/basket/{userId} | دریافت سبد خرید کاربر |
| POST | /api/basket/add-item | افزودن آیتم به سبد |
| PUT | /api/basket/update-quantity | به‌روزرسانی تعداد آیتم |
| DELETE | /api/basket/remove-item | حذف آیتم از سبد |
| DELETE | /api/basket/clear/{userId} | پاک کردن کل سبد |

### نمونه درخواست‌ها

#### افزودن آیتم به سبد
```
POST /api/basket/add-item
Content-Type: application/json
```
```json
{
  "userId": 1,
  "item": {
    "productId": 100,
    "quantity": 2
  }
}
```
#### دریافت سبد خرید
`GET /api/basket/1`

#### پاسخ موفق
```json
{
  "isSuccess": true,
  "value": {
    "id": 1,
    "userId": 1,
    "status": "Active",
    "totalPrice": 3000,
    "totalItems": 2,
    "items": [
      {
        "productId": 100,
        "productName": "Laptop",
        "quantity": 2,
        "unitPrice": 1500,
        "totalPrice": 3000
      }
    ]
  }
}
```

## 🔄 جریان داده (Data Flow)

### Command Flow (Write)
```
Request (Command)
    ↓
ValidationBehavior (اعتبارسنجی)
    ↓
TransactionBehavior (شروع تراکنش)
    ↓
Handler (منطق اصلی)
    ↓
ذخیره در دیتابیس
    ↓
پاک کردن کش
    ↓
انتشار رویداد (Event)
    ↓
Commit Transaction
    ↓
Response (ServiceResult)
```

### Query Flow (Read)
```
Request (Query)
    ↓
ValidationBehavior (اعتبارسنجی)
    ↓
Handler
    ↓
آیا در کش موجود است؟
    ↓        ↓
   بله       خیر
    ↓        ↓
 بازگشت    خواندن از دیتابیس
 از کش         ↓
          ذخیره در کش
              ↓
          بازگشت نتیجه
```

## 🧪 تست‌ها

### اجرای تست‌ها
```
dotnet test
```

## 🐳 اجرا با Docker
### اجرا در محیط مستقل
```
docker-compose up -d
```


## 📂 ساختار دیتابیس

### جدول Baskets
| ستون | نوع | توضیح |
| :--- | :--- | :--- |
| Id | int | کلید اصلی |
| UserId | int | شناسه کاربر |
| Status | int | وضعیت سبد (1=Active, 2=Expired, 3=CheckedOut) |
| CreatedAt | datetime2 | تاریخ ایجاد |
| LastUpdatedAt | datetime2 | آخرین به‌روزرسانی |

### جدول BasketItems
| ستون | نوع | توضیح |
| :--- | :--- | :--- |
| Id | int | کلید اصلی |
| BasketId | int | کلید خارجی به Baskets |
| ProductId | int | شناسه محصول |
| ProductName | nvarchar(200) | نام محصول |
| Quantity | int | تعداد |
| UnitPrice | decimal(18,2) | قیمت واحد |

---
توسعه داده شده با ❤️ توسط علی دهقانی
