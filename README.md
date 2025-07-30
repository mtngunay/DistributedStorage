DistributedStorage - Dosya Chunking ve Dağıtık Depolama Sistemi

Proje Hakkında
DistributedStorage, büyük dosyaların otomatik olarak küçük parçalara (chunk) ayrıldığı, bu parçaların farklı depolama sağlayıcılarına dağıtıldığı ve gerektiğinde orijinal dosya bütünlüğü korunarak tekrar birleştirildiği .NET Console Application tabanlı bir altyapıdır.

Bu proje, dosya yedekleme, dağıtık depolama ve veri güvenliği konularında temel yapı taşlarını soyutlayarak, genişletilebilir ve modüler bir mimari sunmayı hedeflemektedir.

Özellikler
Dinamik Chunk’lama
Dosya boyutuna göre optimal chunk boyutu hesaplanır ve dosyalar otomatik olarak bu parçalara bölünür.

Farklı Depolama Sağlayıcıları
Chunk’lar, dosya sistemi, veritabanı veya bulut tabanlı depolama (S3, MinIO vb.) sağlayıcılarına kolayca dağıtılabilir. (Şuanda sadece veritabanı üzerinden işlem görmektedir.)

Checksum ve Veri Bütünlüğü
SHA256 tabanlı hash algoritmasıyla hem chunk’ların hem de bütün dosyanın doğruluğu garanti edilir.

Bağımlılık Injection & Soyutlama
Tüm temel bileşenler interface’lerle soyutlanmış, Microsoft.Extensions.DependencyInjection ile DI uygundur.

Validation & Güvenlik
FluentValidation ile dosya adı, mime tipi, dosya boyutu ve gerçek dosya içeriği (magic numbers) doğrulanır.

Transaction ve Rollback
Dosya yükleme işlemi UnitOfWork pattern’i ile transaction içerisinde yapılır hata durumunda rollback yapılır.

Logging
Yükleme, indirme ve hata işlemleri ILogger üzerinden detaylı loglanır. (İstenilirse DB ve dosyayada kayıt atılabilir.)

Proje Yapısı ve Katmanlar
src/
├── Application/                    # İş mantığı, servisler, DTOlar, validation
│   ├── Abstractions/               # Interfaceler (IFileUploadService, IUnitOfWork, IStorageProvider...)
│   ├── Services/                   # İş kuralları ve servis implementasyonları
│   ├── Validators/                 # FluentValidation kuralları
│   └── Helpers/                    # Yardımcı fonksiyonlar (örn: FileSignatureHelper)
│
├── Domain/                        # Domain entity ve enum tanımları
│   ├── Entities/                   # File, Chunk, Parameter gibi veri modelleri
│   └── Enums/
│
├── Infrastructure/                # Veri erişimi, repository, depolama providerları, transaction yönetimi
│   ├── Persistence/                # EF Core DbContext, Seeder, Repository implementasyonları
│   ├── Services/                  # Fiziksel dosya sistemi veya diğer storage provider implementasyonları
│   └── Helpers/                   # Encryption, Checksum gibi alt seviye işlemler
│
├── Tests/                        # Unit ve integration testler
│   └── Helpers/                   # ChunkHelper testleri gibi yardımcı test sınıfları
│
├── ConsoleApp/                   # Program.cs, kullanıcı konsol arayüzü
│
└── README.md                    # Proje açıklaması


Ana Bileşenler ve Akış
1. Dosya Yükleme (Upload)
Kullanıcı dosya seçer ve konsolda yükleme başlatılır.

FileUploadRequestValidator ile dosya adı, mime türü, dosya boyutu, ve gerçek dosya imzası (magic numbers) kontrol edilir.

Dosya, ChunkHelper ile dinamik boyutlarda parçalara bölünür. (DB üzerinden dinamik olarak ayarlandı ve default değeri dosyanın mevcut boyutunun %1 dir.)

Her parça IStorageProvider üzerinden uygun depolama sağlayıcısına kaydedilir.

Chunk ve dosya meta bilgileri veritabanına UnitOfWork ve GenericRepository patternleri ile kaydedilir.

SHA256 checksumlar hesaplanarak veri bütünlüğü sağlandı.

Tüm işlem transaction içinde yönetilir, hata durumunda rollback yapılır.

İşlem loglanır.

2. Dosya İndirme (Download)
Kullanıcı dosya IDsini girer.

Chunklar veritabanından çekilir, sıralanır.

Her chunk uygun depolama providerdan okunur.

Her chunkın checksum doğrulaması yapılır. (DB üzerinden değiştirildiğinde anında hata fark ediliyor.)

Chunklar bellekte birleştirilerek orijinal dosya elde edilir.

İndirme ve checksum doğrulama süreçleri loglanır.

Öne Çıkan Teknolojiler ve Tasarım Kalıpları
.NET 9 Console Application

Entity Framework Core (Code First, Repository & Unit of Work pattern)

FluentValidation

Dependency Injection (Microsoft.Extensions.DependencyInjection)

SOLID Prensipleri & OOP Tasarımı

SHA256 Checksum

Logging (ILogger)

Transaction & Rollback Yönetimi

Interface ile Soyutlama & Test Edilebilirlik

Chunking & MemoryStream ile Veri Parçalama & Birleştirme

Projenin Çalıştırılması
Gerekli .NET SDK (9 veya üzeri) kurulmalıdır.

Proje klasöründe terminal açılarak:

dotnet restore
dotnet build
dotnet run --project ConsoleApp
Konsol arayüzünden yönergeleri takip ederek dosya yükleyebilir veya indirebilirsiniz.

