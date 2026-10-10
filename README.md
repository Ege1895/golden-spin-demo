# Vertigo Games – Game Developer Demo

Bu proje, Vertigo Games tarafından gönderilen Game Developer Demo görevi kapsamında geliştirilmiştir.
Oyun mekaniği, Critical Strike içerisindeki "Card Game" modundan referans alınarak Wheel of Fortune deneyimi şeklinde yeniden tasarlanmıştır.

Proje başlangıçta Unity 6 LTS kullanılarak geliştirildi. Geliştirme sürecinde demo dokümanı detaylı şekilde incelendikten sonra, istenen teknik gereksinimlere tam uyum sağlamak amacıyla proje Unity 2021 LTS sürümüne taşındı. Aynı süreçte Vertigo Games oyunlarının genel kullanıcı deneyimi göz önünde bulundurularak arayüz yapısı landscape düzene göre yeniden tasarlandı.

---

# Oyun Mekaniği

Oyunun temel gameplay döngüsü risk-ödül sistemi üzerine kuruludur.

Oyuncu her zone'da ödül dilimleri ve bomba dilimi bulunan çarkı çevirmek zorundadır. Zone ilerledikçe ödül çarpanları otomatik olarak artar ve oyuncu daha yüksek ödüller kazanma fırsatı elde eder.

Oyuncu bomba dilimine denk geldiğinde:

* o ana kadar biriktirdiği tüm ödülleri kaybeder,
* oyun sona erer,
* isterse yeniden başlayabilir veya revive olabilir.

Özel zone sistemleri:

* Her 5. zone bir **Silver Zone** olarak çalışır ve bomba içermez.
* Her 30. zone bir **Golden Zone** olarak çalışır; özel ödüller içerir ve bomba bulunmaz.

Silver ve Golden zone'larda oyuncuya:

* mevcut ödülleri toplayıp güvenli şekilde çıkma,
* veya çarkı tekrar çevirerek daha yüksek ödüller için risk alma

seçeneği sunulur.

Ek bonus özellik olarak revive sistemi geliştirilmiştir. Oyuncu bomba patlattığında sistem, bomba öncesindeki reward snapshot'ını saklar ve oyuncu:

* reklam izleyerek,
* veya oyun içi para harcayarak

önceki state'e geri dönebilmektedir.

Demo kapsamı gereği projeye aktif bir reklam SDK entegrasyonu eklenmemiştir. Ancak revive sistemi buna uygun şekilde modüler olarak tasarlanmıştır ve gerekli durumda Google Ads / rewarded ad entegrasyonu kolayca bağlanabilecek yapıdadır. Bu konuda production seviyesinde entegrasyon deneyimine sahibim.

---

# Teknik Mimari

Proje geliştirilirken temel odak noktaları:

* SOLID prensipleri,
* sürdürülebilir mimari,
* modüler yapı,
* ölçeklenebilirlik,
* ve test edilebilirlik

olmuştur.

Sistem tamamen bağımsız modüller halinde yapılandırılmıştır:

* GameModule
* WheelModule
* ZoneModule
* RewardModule
* CurrencyModule
* AudioModule
* HapticModule
* ConfigurationModule
* UIModule
* InstallerModule

Her modül kendi içerisinde:

* Controller katmanı,
* DTO yapıları,
* UI katmanı,
* ve ScriptableObject konfigürasyonlarını

barındırmaktadır.

Bu yapı sayesinde sistemler birbirinden bağımsız geliştirilebilir ve kolay şekilde genişletilebilir hale getirilmiştir.

---

# Dependency Injection & Design Patterns

Projede dependency injection sistemi olarak Zenject kullanılmıştır.

`AppInstaller` ana composition root olarak çalışmakta ve:

* servisler,
* controller'lar,
* factory yapıları,
* UI panelleri,
* ve runtime bağımlılıklarını

tek merkezden bind etmektedir.

Projede yoğun şekilde:

* interface tabanlı mimari,
* Factory Pattern,
* Null Object Pattern,
* ScriptableObject tabanlı konfigürasyon,
* ve runtime dependency yönetimi

kullanılmıştır.

Runtime sırasında oluşturulan tüm wheel slice UI item'ları Zenject Factory yapıları üzerinden üretilmektedir.

---

# UI Mimarisi

Tüm UI panelleri ortak bir `BasePanel` abstract sınıfından türetilmiştir.

Bu yapı:

* fade-in / fade-out geçişleri,
* CanvasGroup yönetimi,
* DOTween animasyonları,
* `OnShow` / `OnHide` lifecycle hook'ları

gibi ortak davranışları merkezi şekilde sağlamaktadır.

Paneller yalnızca görsel sunumdan sorumludur. Tüm gameplay ve iş mantığı controller katmanında tutulmuştur.

UI tasarımı, sağlanan asset'ler kullanılarak hazırlanmış ve Claude Design desteğiyle iteratif olarak geliştirilmiştir.

Demo dokümanında belirtilen UI gereksinimlerine uygun şekilde:

* Canvas Expand modu,
* doğru anchor/pivot kullanımı,
* Safe Area desteği,
* TextMeshPro kullanımı,
* responsive layout yapısı,
* ve non-stretched UI yaklaşımı

uygulanmıştır.

Proje aşağıdaki aspect ratio'larda test edilmiştir:

* 20:9
* 16:9
* 4:3

Tüm ekran oranlarında layout bozulmadan çalışacak şekilde optimize edilmiştir.

---

# Wheel Sistemi

Çark sistemi tamamen ScriptableObject tabanlı konfigürasyon ile geliştirilmiştir:

* `WheelConfigSO`
* `WheelSliceSO`

Bu yapı sayesinde:

* reward balancing,
* içerik düzenleme,
* ve wheel içerik yönetimi

kod değiştirmeden editor üzerinden yönetilebilir hale getirilmiştir.

Spin sonucu weighted random algoritmasıyla belirlenmektedir.

Çark animasyon sistemi içerisinde:

* DOTween animasyonları,
* easing kullanımı,
* slice geçiş feedback'leri,
* ratchet indicator animasyonları,
* ses efektleri,
* ve haptic feedback desteği

yer almaktadır.

Haptic feedback Android platformunda native olarak çalışırken diğer platformlarda Null Object yaklaşımı kullanılmıştır.

---

# Addressables & Optimizasyon

Runtime asset yönetimi için Addressables kullanılmıştır.

APK boyutunu minimize etmek amacıyla:

* texture compression,
* Addressables bundle optimizasyonları,
* build ayarları,
* UI optimizasyonları,
* ve memory-conscious asset kullanımı

üzerinde çalışmalar yapılmıştır.

Final release APK boyutu **19.2 MB** olacak şekilde optimize edilmiştir.

---

# Test Süreci

Kritik gameplay sistemleri için test double'lar kullanılarak izole unit test'ler yazılmıştır.

Test yapısı özellikle:

* reward akışı,
* revive sistemi,
* wheel sonuçları,
* controller davranışları,
* ve modüller arası etkileşimleri

doğrulamak amacıyla hazırlanmıştır.

---

# Teslim İçeriği

Repository içerisinde:

* Unity 2021 LTS proje dosyaları
* Release APK build'i ([GitHub Releases](https://github.com/Ege1895/golden-spin-demo/releases/latest))
* Gameplay videosu
* 20:9 / 16:9 / 4:3 aspect ratio ekran görüntüleri

yer almaktadır.

---

# Not

Bu proje yalnızca çalışır bir prototype üretmek amacıyla değil; production-oriented, sürdürülebilir ve ölçeklenebilir bir Unity mimarisi oluşturma hedefiyle geliştirilmiştir.

Temel amaç:

* tekrar kullanılabilir sistemler,
* sürdürülebilir gameplay mimarisi,
* maintainable UI yapısı,
* ve profesyonel Unity geliştirme standartlarına uygun temiz bir codebase

oluşturmaktır.
