# MVC Identity Demo

**Bu proje, ASP.NET Core Identity kullanarak bir MVC uygulamasının nasıl oluşturulacağını ve bazı önemli yapıları nasıl entegre edeceğinizi göstermektedir.

## 🚀 Özellikler

- **Kullanıcı Kaydı**: Yeni kullanıcıların e-posta ve şifre ile kaydolmasına olanak sağlar.
- **Kullanıcı Girişi**: Kullanıcıların kimlik bilgileriyle giriş yapmasını sağlar.
- **Başarı & Başarısızlık Sayfaları**: Kayıt işlemi sonrası başarı veya başarısızlık sayfalarına yönlendirir.

## 📦 Kullanılan Teknolojiler

- **ASP.NET Core MVC**: Web uygulamasını oluşturmak için kullanılır.
- **ASP.NET Core Identity**: Kullanıcı kimlik doğrulama ve yetkilendirme işlemleri için kullanılır.
- **Entity Framework Core**: Veritabanı işlemleri ve veri erişimi için kullanılır.
- **Mapster**: Nesne dönüştürme işlemleri için kullanılır.
- **Bootstrap**: Duyarlı tasarım ve stil için kullanılır.

## 🔧 Yapılar ve Desenler

- **Utilities**: Projede sonuç yapıları (result structures) kullanılarak işlem sonuçları daha iyi yönetilmektedir.
- **Repository Pattern (Generic)**: Veritabanı erişimini soyutlamak için generic repository pattern kullanılmıştır.
- **DTO Yapısı**: Veri transfer nesneleri (DTOs) kullanılarak uygulamanın farklı katmanları arasında veri taşınmaktadır.
- **Katmanlı Mimari**: Projede iş mantığı, veri erişimi ve UI katmanları arasında net bir ayrım yapılarak katmanlı mimari kullanılmıştır.

## 📂 Proje Yapısı

- **Controllers**: Kullanıcı etkileşimlerini işleyen action metodlarını içerir.
- **Models**: Görünüm modelleri ve veri transfer nesnelerini içerir.
- **Views**: UI'yı render eden Razor görünümlerini içerir.
- **Services**: İş mantığı ve servis katmanı.
- **Repositories**: Veritabanı ile etkileşim için veri erişim katmanı.
- **Utilities**: Sonuç yapıları ve genel yardımcı sınıfları içerir.
- **DTOs**: Veri transfer nesneleri.

