🚀 **E-Ticaret Platformu**



Bu proje, ASP.NET Core MVC üzerinde geliştirilmiş, modüler bir e-ticaret uygulamasıdır. Ürünleri, kategorileri ve kullanıcıları yönetmek için kapsamlı bir yönetim paneli sunarken, müşteriler için de sorunsuz bir alışveriş deneyimi sağlar. Ölçeklenebilirlik ve sürdürülebilirlik göz önünde bulundurularak tasarlanmış olup, modern yazılım mimarisi prensiplerine uygun olarak geliştirilmiştir.

✨ **Temel Özellikler**

Güvenli Kullanıcı Kimlik Doğrulama ve Yetkilendirme: Güçlü kullanıcı yönetimi için ASP.NET Core Identity ile uygulandı; rol tabanlı erişim kontrolü (Admin, Müşteri rolleri) içerir.
Ürün Yaşam Döngüsü Yönetimi: Özel bir yönetici paneli aracılığıyla ürünler için tam CRUD (Oluştur, Oku, Güncelle, Sil) işlemleri; detaylı ürün listelemelerini ve dinamik görsel yüklemelerini destekler.
Kategori Yönetimi: Ürünlerin düzenli organizasyonunu sağlamak için ürün kategorilerinin verimli bir şekilde oluşturulması, düzenlenmesi ve silinmesi.
Alışveriş Sepeti İşlevselliği: Kullanıcıların ödeme yapmadan önce ürünleri sepete eklemesine, çıkarmasına ve yönetmesine olanak tanıyan sezgisel alışveriş sepeti mekanizması.
Dinamik Görsel İşleme: Ürün başına birden fazla görsel yüklemeyi destekleyerek ürün sunumunu zenginleştirir.
Kapsamlı Veri Doğrulama: Veri bütünlüğünü sağlamak için hem sunucu tarafı hem de istemci tarafı doğrulama için DataAnnotations kullanır.
Katmanlı Mimari: Gelişmiş modülerlik, test edilebilirlik ve sorumlulukların ayrılması için temiz, N-katmanlı bir mimari desen (Application, Domain, Infrastructure, WebUI) takip eder.
Otomatik Nesne Eşleme: DTO (Veri Transfer Nesnesi) ile Entity arasında sorunsuz ve verimli veri aktarımı için AutoMapper kullanır, tekrar eden kodları azaltır.
Veritabanı Yönetimi: ORM için Entity Framework Core tarafından desteklenir, SQL Server ile verimli veri kalıcılığı ve alımını kolaylaştırır.

**🛠️ Teknolojiler ve Bağımlılıklar**

**Backend:**

ASP.NET Core 8.0 MVC: Web uygulaması geliştirme için temel framework.
Entity Framework Core: Veritabanı etkileşimleri için ORM.
SQL Server: İlişkisel veritabanı yönetim sistemi.
ASP.NET Core Identity: Kullanıcı kimlik doğrulama ve yetkilendirme sistemi.
AutoMapper: Nesneden nesneye eşleme kütüphanesi.

**Frontend:**

Bootstrap 5: Modern kullanıcı arayüzleri için duyarlı CSS framework'ü.
jQuery / Vanilla JavaScript: Etkileşimli öğeler için istemci tarafı betik dili.

**🚀 Kurulum ve Çalıştırma**

Projeyi yerel ortamınızda kurmak ve çalıştırmak için aşağıdaki adımları izleyin:

**Ön Gereksinimler:**

.NET 8 SDK
SQL Server (veya SQL Server Express/Developer Sürümü)
Visual Studio 2022 (önerilir) veya uyumlu bir IDE/düzenleyici.,

**Kurulum Adımları**

1-Depoyu Klonlayın:
```
Bash

git clone https://github.com/furkannky/ecommerce.git
cd ecommerce
```

2-Veritabanı Bağlantısını Yapılandırın:

ECommerce.WebUI projesindeki appsettings.json dosyasını açın.
DefaultConnection bağlantı dizesini kendi yerel SQL Server örneğinize göre güncelleyin:
```
JSON

"ConnectionStrings": {
    "DefaultConnection": "Server=SİZİN_SQL_SUNUCU_ADINIZ;Database=ECommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
},
```
SİZİN_SQL_SUNUCU_ADINIZ kısmını kendi SQL Server örnek adınızla değiştirin.

Veritabanı Migrasyonlarını Uygulayın:

Visual Studio'da Paket Yöneticisi Konsolu'nu açın (Araçlar > NuGet Paket Yöneticisi > Paket Yöneticisi Konsolu).
"Varsayılan proje" açılır listesinin ECommerce.Infrastructure olarak ayarlandığından emin olun.
Veritabanı şemanızı oluşturmak ve güncellemek için aşağıdaki komutları çalıştırın:
```
PowerShell

Add-Migration InitialSetup
Update-Database
```
İlk Yönetici Kullanıcısını Kurma:

Uygulama, ilk başlatıldığında otomatik olarak bir yönetici kullanıcısı oluşturma mantığı içerir.
Yönetici kimlik bilgilerini appsettings.json dosyasında yapılandırın:
```
JSON

"AdminSettings": {
    "Email": "admin@example.com",
    "Password": "AdminPassword123!!"
}
```
Yönetici hesabınız için güçlü, benzersiz bir şifre kullandığınızdan emin olun.

Uygulamayı Çalıştırın:


komut satırından çalıştırın:
```
Bash

dotnet run --project ECommerce.WebUI
```

🤝 Katkıda Bulunma

Bu projeyi geliştirmek isterseniz katkılarınızı bekliyoruz! Lütfen aşağıdaki adımları izleyin:


Depoyu Fork'layın (Çatallayın).
Özelliğiniz veya hata düzeltmeniz için yeni bir dal (branch) oluşturun: git checkout -b ozellik/harika-ozellik-adi
Değişikliklerinizi açık ve özlü mesajlarla Commit'leyin: git commit -m 'feat: Yeni harika bir özellik eklendi'
Dalınızı (branch) çatal depoya (forked repository) itin (push edin): git push origin ozellik/harika-ozellik-adi
Bu depodaki main dalına (branch) karşı bir Çekme İsteği (Pull Request) açın.
Lütfen kodunuzun projenin kodlama standartlarına uyduğundan ve uygun durumlarda testleri içerdiğinden emin olun.



📞 İletişim

Herhangi bir soru veya geri bildiriminiz için iletişime geçmekten çekinmeyin:


Proje Yöneticisi: Furkan KAYA - furkannkayaa49@gmail.com

LinkedIn: www.linkedin.com/in/furkanky
