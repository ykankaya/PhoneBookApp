# Telefon Rehberi Mikroservis Uygulaması

Bu proje, .NET Core kullanılarak geliştirilmiş, basit bir telefon rehberi uygulamasını temsil eden bir mikroservis mimarisi örneğidir. İki ana servisten oluşur: Kişi (Contact) Servisi ve Rapor (Report) Servisi. Servisler arası asenkron iletişim için Kafka kullanılmıştır.

## Özellikler

*   **Kişi Yönetimi (ContactService):**
    *   Rehbere kişi ekleme, silme.
    *   Kişilere iletişim bilgisi (telefon, email, konum) ekleme, silme.
    *   Rehberdeki tüm kişileri listeleme.
    *   Bir kişinin iletişim bilgileriyle birlikte detaylarını getirme.
*   **Rapor Yönetimi (ReportService):**
    *   Konum bazlı istatistik raporu talebi oluşturma (asenkron).
    *   Oluşturulan rapor taleplerini ve durumlarını listeleme.
    *   Tamamlanmış bir raporun detaylarını (konum, kişi sayısı, telefon sayısı) getirme.

## Mimari Genel Bakış

*   **Mikroservisler:** `ContactService` ve `ReportService`.
*   **İletişim:**
    *   Servisler arası senkron iletişim: HTTP REST (API endpoint'leri üzerinden).
    *   Servisler arası asenkron iletişim: Kafka (Rapor talepleri için).
*   **API Katmanı:** Carter kütüphanesi ile Minimal API yaklaşımı.
*   **İş Mantığı:** MediatR ile CQRS (Command Query Responsibility Segregation) deseni.
*   **Veri Erişimi:** Entity Framework Core ile PostgreSQL veritabanları (Her servis için ayrı veritabanı).
*   **Hata Yönetimi:** Polly ile Retry mekanizmaları (HTTP çağrıları için).
*   **Validasyon:** FluentValidation.
*   **Mimari Desen:** Clean Architecture prensiplerine uygun katmanlı yapı (Domain, Application, Infrastructure, Api).

![Mimari Şeması (Opsiyonel - Buraya bir çizim linki veya dosyası ekleyebilirsiniz)](placeholder_diagram.png)

## Kullanılan Teknolojiler

*   .NET 6 (veya üstü)
*   ASP.NET Core
*   Entity Framework Core 6 (veya üstü)
*   PostgreSQL
*   Kafka
*   MediatR
*   Carter
*   FluentValidation
*   Polly
*   xUnit (Unit Testler için)
*   Moq (Mocking için)
*   FluentAssertions (Assertion'lar için)
*   Git
*   Docker & Docker Compose (Opsiyonel - Yerel ortam kurulumu için)

## Ön Gereksinimler

*   .NET SDK (Projede kullanılan sürüm - örn: 6.0 veya 7.0)
*   Git
*   PostgreSQL Sunucusu (ve pgAdmin gibi bir yönetim aracı)
*   Kafka Sunucusu (ve Zookeeper) - Tek node kurulum yeterlidir.
*   Docker ve Docker Compose (Eğer `docker-compose.yml` kullanacaksanız)

## Kurulum ve Yapılandırma

1.  **Projeyi Klonlayın:**
    ```bash
    git clone https://github.com/[kullanici_adiniz]/[repo_adiniz].git
    cd [repo_adiniz]
    ```

2.  **Veritabanlarını Oluşturun:**
    *   PostgreSQL sunucunuzda iki adet boş veritabanı oluşturun:
        *   `ContactDb` (veya `appsettings.json` içinde belirttiğiniz isim)
        *   `ReportDb` (veya `appsettings.json` içinde belirttiğiniz isim)
    *   **Önemli:** Veritabanı kullanıcısının bu veritabanları üzerinde tam yetkisi olduğundan emin olun.

3.  **Kafka Topic Oluşturun:**
    *   Kafka sunucunuzda `report-requests` isimli bir topic oluşturun. Gerekli partition ve replication factor ayarlarını yapabilirsiniz (yerel geliştirme için 1 genellikle yeterlidir).

4.  **Yapılandırma Dosyalarını Ayarlayın (`appsettings.json`):**
    *   `src/ContactService/ContactService.Api/appsettings.json`:
        *   `ConnectionStrings:ContactDbConnection`: PostgreSQL bağlantı bilgilerinizi (Host, Port, Database, Username, Password) güncelleyin.
    *   `src/ReportService/ReportService.Api/appsettings.json`:
        *   `ConnectionStrings:ReportDbConnection`: PostgreSQL bağlantı bilgilerinizi güncelleyin.
        *   `Kafka:BootstrapServers`: Kafka sunucunuzun adresini (örn: `localhost:9092`) güncelleyin.
        *   `ContactService:BaseUrl`: Çalışan `ContactService.Api`'nin adresini (örn: `http://localhost:5001` veya `https://localhost:7001` - Port numarasını kontrol edin!) güncelleyin.
    *   **Geliştirme Ortamı İçin:** Hassas bilgileri (veritabanı şifresi vb.) `appsettings.json` yerine User Secrets kullanarak yönetmeniz önerilir:
        ```bash
        # ContactService için
        cd src/ContactService/ContactService.Api
        dotnet user-secrets init
        dotnet user-secrets set "ConnectionStrings:ContactDbConnection" "Host=...;Username=...;Password=YOUR_SECRET_PASSWORD"
        cd ../../.. # Ana dizine dön

        # ReportService için
        cd src/ReportService/ReportService.Api
        dotnet user-secrets init
        dotnet user-secrets set "ConnectionStrings:ReportDbConnection" "Host=...;Username=...;Password=YOUR_SECRET_PASSWORD"
        cd ../../.. # Ana dizine dön
        ```

5.  **Veritabanı Migration'larını Uygulayın:**
    *   Projenin ana dizinindeyken aşağıdaki komutları çalıştırın:
    ```bash
    # ContactService için
    dotnet ef database update --project ./src/ContactService/ContactService.Infrastructure/ --startup-project ./src/ContactService/ContactService.Api/

    # ReportService için
    dotnet ef database update --project ./src/ReportService/ReportService.Infrastructure/ --startup-project ./src/ReportService/ReportService.Api/
    ```

6.  **Bağımlılıkları Yükleyin:**
    ```bash
    dotnet restore
    ```

## Uygulamayı Çalıştırma

**Yöntem 1: `dotnet run` ile (Her servis ayrı terminalde)**

1.  **ContactService'i Başlat:**
    ```bash
    dotnet run --project ./src/ContactService/ContactService.Api/
    ```
    *Not: Varsayılan olarak `http://localhost:5048`  port ile çalışacaktır (launchSettings.json kontrol edin).*

2.  **ReportService'i Başlat:**
    ```bash
    dotnet run --project ./src/ReportService/ReportService.Api/
    ```
    *Not: Varsayılan olarak `http://localhost:5188`  port ile çalışacaktır (launchSettings.json kontrol edin).*

**Yöntem 2: Docker Compose ile (Eğer `docker-compose.yml` dosyası varsa)**

*   Proje ana dizininde `docker-compose.yml` dosyası varsa (PostgreSQL, Kafka ve servisler için):
    ```bash
    docker-compose up -d --build
    ```
    *Bu komut imajları build eder ve container'ları arka planda başlatır.*
    *Servislerin hangi portlarda map edildiğini `docker-compose.yml` dosyasından kontrol edin.*

## Testleri Çalıştırma

Proje ana dizinindeyken aşağıdaki komutu çalıştırarak tüm unit testleri çalıştırabilirsiniz:

```bash
dotnet test