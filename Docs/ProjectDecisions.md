# KitchenChaos — Güncel proje kararları

Son güncelleme: 20 Eylül 2026.

## Bu belgenin amacı ve nasıl kullanılacağı

Bu dosya, kullanıcıyla onaylanan tasarım kararlarının kalıcı kaydıdır. Yeni bir çalışma oturumunda, bağlam kaybından sonra ve hareket/savaş/bölüm tasarımı öncesinde okunmalıdır. Önceki sohbetin eksiksiz hatırlanmasına güvenilmez.

- Tasarım kararlarında bu belge, eski tarihli yol haritalarının çelişen maddelerinin yerine geçer. Kullanıcının daha yeni açık kararı her zaman önceliklidir.
- Onaylanmış hedef, uygulanmış özellik demek değildir. Aşağıdaki durum tablosunu dikkate al; çalışmaya başlamadan ilgili kod, prefab ve sahneyi kontrol et.
- Kullanıcı yeni bir karar verdiğinde bu dosyayı güncelle. Bir özellik uygulanınca ve kullanıcı tarafından test edilince durumunu ayrı ayrı kaydet.
- Eski teknik notlar tarihsel uygulama ayrıntılarıdır; güncel Inspector değerleri veya tamamlanma durumu yerine doğrudan kullanılmamalıdır.

## 1. Oyun kimliği — onaylandı

- Unity ile geliştirilen, mutfak temalı 2D aksiyon platformer.
- Uzun vadeli hedef yaklaşık 4 saatlik oyun; şu an öncelik yaklaşık 10 dakikalık tek oynanabilir örnek bölüm. Süre henüz elde edilmiş veya ölçülmüş kabul edilmez.
- Cuphead benzeri zor ama öğrenilebilir düşman ve boss mücadeleleri. Cuphead'in birebir kopyası değil: yakın ve uzak dövüş birlikte temel rol oynar.
- Kolay öğrenilen kontroller; saldırı işaretini okuma, konum seçme, kaçınma ve doğru saldırıyı kullanma üzerinden derinlik.
- Zorluk yalnızca düşman canını artırarak, tepki sürelerini aşırı kısaltarak veya rotayı uzatarak üretilmez.
- Ana karakter: insan, yaşlı, göbekli, beyaz bıyıklı şef; lekeli önlük, onaylı A konsepti ve sonradan onaylanan daha sinirli yüz. Yaşı/gövdesi kontrollerin ağır ve gecikmeli olmasını gerektirmez.

## 2. Dünya ve bölüm yapısı — onaylanan yön

- Küçük bir mutfak merkezi (hub) ve buradan seçilen bağımsız bölümler.
- Bölümler kendi içinde çoğunlukla lineer; kısa isteğe bağlı yan yollar olabilir.
- Bölümler belirli sırayla açılır; tamamlananlara tekrar girilebilir.
- Merkezde silah/yük seçimi ve bölüm seçimi bulunur. Büyük kasaba, kapsamlı görev sistemi veya çok sayıda dükkân ilk kapsamda yok.
- Metroidvania tipi birbirine bağlı dev harita ve yeni yetenekle eski bölümlere zorunlu dönüş hedeflenmiyor.
- Temel hareketler erken öğretilir; çift zıplama/dash gibi hareketleri uzun süre kilitli tutmak üzerinden bölüm ilerlemesi kurulmaz.
- Kiler, soğuk hava deposu, fırın gibi yerler yalnızca tema örnekleri; bölüm adları, sayısı ve kesin sırası henüz kararlaştırılmadı.
- İlk demo hub olmadan doğrudan örnek bölümü açabilir. Geçici başlat/tekrar oyna ekranı yeterli; hub üretimi daha sonra.

### Hikâye kararı — bölüm tasarımından önce

- Kullanıcıyla hikâyenin temeli, ayrıntılı bölüm tasarımına geçmeden önce kararlaştırılacak. Şimdiki öncelik hareket ve savaş paketidir.
- Bölüm tasarımına başlamanın koşulu: şefin amacı, yiyeceklerin neden canlandığı/saldırdığı ve finalde neyin değişeceği netleşmiş ve kullanıcı tarafından onaylanmış olmalı.
- Ayrıntılı senaryo, diyaloglar, giriş/final çizimleri ve anlatım sahneleri daha sonra hazırlanabilir. İlk demo için kısa bir amaç açıklaması yeterli olabilir.
- Konuşmada geçen “son servis gecesinde yanlışlıkla hazırlanan eski tarifin mutfağı canlandırması” yalnızca örnektir; seçilmiş hikâye değildir.

## 3. Hareket paketi — uygulanması onaylandı

| Mekanik | Kararlaştırılan başlangıç davranışı |
| --- | --- |
| Yürüme/koşma | Analog az itilince yürüme, tam itilince koşma. Klavyede yön tuşları normal koşma hızını verir. Ayrı sprint tuşu yok. |
| Zıplama | Tuşu tutma süresine göre değişken yükseklik; havada yön kontrolü. |
| Çift zıplama | Havada bir ek zıplama. İlk testte ikinci sıçrama yetersiz bulundu: artık ilk sıçramayla aynı başlangıç hızı ve kısa basışta da tam yükseliş deneniyor. Yere inince yenilenir, düşmana vurunca sınırsız yenilenmez. |
| Yer + hava dash'i | Kullanıcı hava dash'ini zorunlu olarak ekledi; zıplamadan/çift zıplamadan sonra kullanılabilir. İlk revizyon: havalanış başına bir kullanım, yere inince yenilenme, kısa yatay hareket ve bekleme süresi. Kısa bir bölümünde düşman hasarına karşı koruma; dash sırasında saldırı yok. Bölümler hava dash'i erişimine göre tasarlanacak. |
| Dash sınırları | Duvarlardan geçmez; çukur/diken/lavı genel dokunulmazlıkla etkisizleştirmez. Ayrıntılı süre/mesafe ve saldırı iptal pencereleri testte ayarlanacak. |
| Eğilme | Gerçek vurulabilir yüksekliği azaltır; yüksek mermiler ve alçak siperler için işlevsel. Eğilerek yatay atış mümkün. Her saldırıyı engellemez. |
| Kontrol toleransı | Kenardan yeni ayrılmışken kısa zıplama toleransı ve inişten hemen önce verilen zıplama komutunu hatırlama hedeflenir. Kesin değerler testle seçilir. |

Duvar zıplaması/tırmanma ve ayrı stamina sistemi ilk demoya dahil değil. Hava dash'i artık kapsamda; önceki yalnız yerde dash kararı kullanıcının yeni talebiyle değişti. Hava dash'i, çift zıplama ve koşma hızının toplam erişimi bölüm tasarımında birlikte değerlendirilecek.

## 4. Savaş paketi — uygulanması onaylandı

### Yakın saldırı: spatula

- Yerde ve havada kullanılabilir; hızlı, okunaklı ve kısa menzilli.
- Uzak saldırıya göre daha yüksek hasar verimi ve daha hızlı özel enerji kazanımı hedeflenir; yaklaşma riski ödüllendirilir.
- Uygun düşmanların saldırısını kesebilir; sürekli sersemletip karşılık vermelerini tamamen engellememeli.
- İlk demoda mevcut tek saldırı temel alınır. Uzun kombo zinciri veya ayrı ağır saldırı zorunlu değil.

### Uzak saldırı: çatal fırlatma

- Kullanıcı sos şişesi önerisini değiştirdi: ilk uzak silah ÇATAL. Bıçak ilk silah değil.
- Sivri uçları ileri bakacak şekilde düz uçuş; basılı tutunca belirli aralıklarla atış.
- İlk sürümde cephane/şarjör sınırı yok; atış aralığı var.
- İlk düşmana veya duvara çarpınca kaybolur; delip geçmez.
- Yakın saldırıdan daha düşük hasar verimi; avantajı mesafe. Her isabet saldırı kesmez.
- Yerde ve havada kullanılabilir. İlk hedef yönler sağ/sol ve yukarı/çapraz yukarı; serbest 360 derece nişan sistemi ilk kapsamda yok.
- Menzil, kamera ve düşman algılama mesafesi birlikte tasarlanır: tepki veremeyen ekran dışı düşmanı risksiz öldürmek baskın yöntem olmamalı.

### Özel saldırı: spatulayla dönüş

- Tek dönüşle çevredeki birden fazla yakın düşmana vurma; küçük düşmanları geri itme.
- Bir kullanımda aynı düşmana kontrolsüz çoklu hasar vermez; bossu sürekli sersemletmez.
- Otomatik dokunulmazlık sağlamaz; enerji harcar.
- Tek özel enerji göstergesi: isabetli saldırılar doldurur, yakın saldırı daha fazla katkı sağlar. Kesin miktarlar testte belirlenecek.
- İlk aşamada seçili silahtan bağımsız şef yeteneği. Her silaha ayrı özel animasyon gerekmiyor.
- Ek alan saldırısı sistemi şart değil; dönüş zaten ilk alan saldırısıdır.

### Şefin darbe tepkisi — ilk test paketinin zorunlu parçası

- Kullanıcı özellikle hareket testlerine eklenmesini istedi.
- Önce mevcut hasar sistemi/görsel tepkisi incelenecek; var olan koruma, ölüm ve respawn sistemi yeniden yazılmayacak.
- Kısa acı ifadesi/gövde tepkisi; can kaybı, ses ve görsel tepki uyumlu.
- Hasar sonrası geçici korunma anlaşılır gösterilir.
- Havada/dash sırasında alınan hasar kalıcı kontrol kilitlenmesi yaratmaz. Ölüm animasyonu önceliklidir.
- Sadece hasar animasyonu oynamasıyla oynanış sersemlemesi aynı şey değildir; bunlar ayrı yönetilir.
- Ölüm darbesi çömelmişken gelirse karakter doğrulmadan alçak pozdan yere yığılmalı; çömelerek hareket etmek bu kuralı değiştirmez. Ölüm başlangıcındaki poz seçimi, tuş bırakılmasıyla değişmez. Ayakta ölüm ayrı kalır.

## 5. Silah açılması ve yük seçimi — onaylandı

- Oyunun başında spatula ve çatal açık. Hepsi başlangıçtan açık değil.
- Yeni silahlar ilerledikçe, bazı bölüm/boss tamamlamalarıyla açılır. Kesin ödül takvimi henüz yok.
- Hub'da açılmış silahlardan bir yakın ve bir uzak silah seçilir.
- Yeni silahlar eskilerin zorunlu daha güçlü sürümü değil; farklı hız, menzil, risk ve kullanım avantajları sunar. İlk silahlar sonlara kadar kullanılabilir kalmalı.
- Yanlış silah seçimi bölümü/bossu geçilemez yapmamalı. Boss yakın dövüşe fırsat da vermeli.
- Başarısız deneme sonrasında yük değiştirmek kolay olmalı; her denemede hub'a geri yürüme zorunluluğu yok. Bunun kesin arayüzü henüz tasarlanmadı.
- İlk demoda spatula + çatal + dönüş yeterli. Mağaza, çok sayıda silah, yetenek ağacı veya kapsamlı yük ekranı gerekmiyor.
- Tava, delici bıçak, tuzluk/biberlik saçması, dönen tabak, un torbası yalnızca gelecek silah örnekleri; üretim taahhüdü değil.

## 6. Her bölüm ve boss için tasarım kontrol listesi

Ön koşul: Hikâyenin temeli kullanıcıyla onaylanmış mı? Mekanik test alanları bu koşulu beklemez; gerçek bölümün tema, amaç ve ilerleme tasarımı bekler.

1. Yerleşim güncel koşma hızı, çift zıplama yayı, dash mesafesi ve kamera ile tasarlanmış mı?
2. Yeni hareket/düşman önce güvenli ve tek başına öğretilip sonra diğerleriyle birleştiriliyor mu?
3. İniş noktaları ve tehlikeler görülebiliyor mu; kör inişte kaçınılmaz hasar var mı?
4. Saldırı hazırlığı görsel/sesle anlaşılır mı; kaçınmak için alan ve süre var mı?
5. Kaçınmayı başaran oyuncuya saldırı fırsatı veriliyor mu?
6. Eğilme, zıplama ve dash farklı durumlarda işe yarıyor mu; tek davranış her sorunu çözüyor mu?
7. Domates ve bezelye birbirini destekliyor mu; birlikte kaçınılmaz hasar oluşturmadan baskı kuruyor mu?
8. Hem yakın hem uzak silahla ilerlemek mümkün mü; menzil dışından risksiz atış veya sürekli sersemletme baskın mı?
9. Görsel darbe tepkisi ile gerçekten saldırı kesilmesi ayırt edilebiliyor mu?
10. Checkpoint ve boss tekrar yolu kısa/makul mü; bölüm bitişi açık mı?
11. Süre gerçek oynanışla ölçülüyor mu; tekrar deneme süresi yeni içerik gibi sayılıyor mu?

## 7. Mevcut durum — 20 Eylül 2026 sohbet/test kaydı

Bu tablo son kullanıcı bildirimlerine dayanır; yeni oturumda ilgili dosyalarla doğrulanır.

| Alan | Durum |
| --- | --- |
| Şef | Önceki idle/koşu/zıplama/saldırı/ölüm sunumu onaylı. Dört karelik çömelerek yürüyüş kullanıcı tarafından kesik/doğal olmayan olarak değerlendirildi. Yerine sekiz yeni ara poz, mesafeye bağlı döngü (tam hızda yaklaşık 14,5 kare/sn) eklendi. Ayakta ve çömelmiş darbe için ayrı üç aşamalı pozlar ve sürekli görsel geri çekilme uygulandı. Bu yeni animasyonlar kullanıcı testi bekliyor; onaylandı sayma. |
| Temel oyun | Hareket, zıplama, hareketli platform, tehlike, toplama, can, checkpoint, ölüm ve respawn mevcut. Köşeye takılma ve çukur ölümü sorunlarının düzeltildiğini kullanıcı bildirdi. |
| Domates | Yakın temas/devriye düşmanı; darbe tepkisi ve parçalanarak ölüm kullanıcı tarafından onaylı. |
| Bezelye | Eski biberin yerine B/saf ifadeli bezelye. Eğilerek ağızdan atış, darbe ifadeleri, kabuğun açılıp tanelerin dağılması kullanıcı tarafından onaylı. |
| Bezelye dengesi | Hazırlık 0,45 sn; atış sonrası bekleme 0,45 sn; can 3. İlk hasarda 0,30 sn sersemleme; sonrasında 0,90 sn sersemleme direnci, fakat hasar almaya devam eder. Darbe sonrası ek atış toparlanması kaldırıldı. Bunlar değişmez kurallar değil, son onaylı başlangıç ayarları. |
| Bezelye mermisi | Son kayıt: hız 9,5; hasar 1; ömür 1,4 sn; yarıçap 0,16. Mevcut mermi bitmeden yeni atış hazırlığı başlamadığı için gerçek atış aralığı yalnız hazırlık+bekleme toplamı değildir. |
| Kontrolcü | Kullanıcı Xbox Series S kontrolcüsüyle mevcut hareket/A zıplama/X saldırı tuşlarını test etti, çalıştığını bildirdi. |
| Yeni hareket paketi | Kullanıcı ilk testte çoğu mekaniğin çalıştığını bildirdi. Revizyon: test sahnesi koşma hızı 5→7, hıza bağlı koşu animasyonu yaklaşık 10→14 kare/sn; eğilerek yürüme kareleri; tam güçlü ve tuş bırakmayla kesilmeyen ikinci sıçrama; bir hava dash'i uygulandı. Yeni revizyonun oynanış testi bekleniyor. |
| Çatal ve dönüş | Onaylı hedefler; bu karar aşamasında uygulanmadı. |
| 10 dakikalık bölüm, boss, hub, silah açma | Tamamlanmış sayılmaz; sıradaki adımlar aşağıda. |

Teknik yön bulma:

- Proje: `/Users/alicanozturk/Projeler/KitchenChaos`
- Mevcut bölüm: `Assets/Scenes/KitchenIntro_Blockout.unity`; bu sahne ve eski `Level01` korunur. Yeni aktif mekanik test sahnesi: `Assets/Scenes/KitchenMovement_Test.unity`.
- Yeni hareketler `PlayerMobility` ile açılır; bileşen yalnız test sahnesindeki oyuncuya eklendi, temel oyuncu prefabına eklenmedi. Paylaşılan kodlarda entegrasyon var; eski sahne yerleşimi değiştirilmedi.
- Test ayarları ve kontrol listesi: `Docs/MovementTest-2026-09-20.md`. Bu alan gerçek bölüm tasarımı veya 10 dakikalık demo değildir.
- Prefablar: `Assets/Prefabs/VerticalSlice`.
- Bezelye hâlâ uyumluluk için `Pepper_Prototype.prefab`, `PepperSeed_Prototype.prefab`, `PepperRangedAttack` ve `PepperSeedProjectile` adlarını kullanır. Bunları ayrı bir biber türü sanma; sırf ad için GUID/refaktör değişikliği yapma.
- Bezelye sunumu: `PeaRangedVisual.cs`, `PeaDeathVisual.cs`; sersemleme: `EnemyHitStun.cs`. Domatesin varsayılan sersemleme direnci sıfır; bezelye değişiklikleri domatese yayılmamalı.
- Kontrol düzeni: A zıplama, X spatula; RB dash ve aşağı eğilme test paketine eklendi, kullanıcı testi bekliyor. RT çatal ve Y özel hâlâ taslak; uygulanmadı.

## 8. Onaylanan uygulama sırası

1. Çalışan mevcut bölümü koru. Yeni hareketleri küçük/ayrı test alanında hazırla: yürüme/koşma, değişken zıplama, çift zıplama, yer/hava dash'i, eğilme. Şefin darbe tepkisini bu pakete dahil et; son geri bildirimle ilk dört hareket düzeltmesinden sonra darbe sunumu geliştirilebilir.
2. Kullanıcı hareket ve hasar testini yapar. Mesafe, süre, kontrol ve animasyon hissi ayarlanır.
3. Çatal fırlatmayı; yerde/havada yakın-uzak saldırı uyumunu ekle ve kullanıcıyla dengele.
4. Özel enerji ve spatula dönüşünü ekle, kullanıcıyla test et.
5. Bölüm tasarımına geçmeden kullanıcıyla hikâyenin temelini kararlaştır: şefin amacı, çatışmanın nedeni ve finaldeki sonuç. Henüz hikâye seçilmedi; örnek fikri onaylı sayma.
6. Hareket, savaş ve hikâyenin temeli netleşince domates/bezelye karşılaşmalarını ve platformları tasarla. Bilinen mekaniği tek başına öğret, sonra birleştir.
7. Yaklaşık 10 dakikalık başlangıç–gelişme–boss–bitiş akışını tamamla. İki düşman türü ve bir boss ilk hedef için yeterli; üçüncü düşman zorunlu değil.
8. Tutarlı çevre sanatı, ses, HUD ve okunabilirliği tamamla; süreyi oynanışla doğrula. Ayrıntılı anlatım içerikleri daha sonra eklenebilir.
9. Sonrasında küçük hub, ilerlemeyle açılan silahlar ve diğer bölümler.

Eski yol haritasındaki tost makinesi boss ve yedi alan sırası başlangıç taslağıdır. Yeni hareketlere göre tekrar değerlendirilmeden nihai yerleşim/denge kabul edilmez. Kesin boss saldırıları, bölüm sayısı ve yaklaşık dört saatlik içeriğin dağılımı açık konulardır.

Güncel ilerleme: hareket düzeltmelerinden sonra çömelme akıcılığı ve darbe sunumu revizyonu da uygulandı; sıradaki adım kullanıcı testi. Yeni darbe sunumu 0,42 sn; oynanış saldırı kesintisi mevcut 0,24 sn olarak kaldı, kontrol/hasar koruması süresi uzatılmadı. Ölüm öncelikli; toparlanma sırasında izin verilen yeni saldırı/dash sunumu devralabilir. Eğilirken yavaş ilerleme vardır; bu ilk pakette eğilerek spatula saldırısı kapalıdır. Onaylı eğilerek yatay uzak atış, çatal paketinde eklenecek.

## 9. Çalışma ve doğrulama kuralları

- Oynanış/Play Mode testlerini kullanıcı yapar. Asistan Play başlatmaz, karakteri kontrol etmez, otomatik test koşucusu veya build başlatmaz; kullanıcı açıkça farklı istemedikçe bu iş bölümü geçerlidir.
- Asistan kod/prefab/sahne değişikliği, statik referans kontrolü ve normal Unity içe aktarma/derleme mesajlarını inceleyebilir. Sonunda kısa test listesi verir; çalıştırılmamış şeyi test edilmiş diye sunmaz.
- Rutin kapsam içi Unity düzenlemeleri için tekrar tekrar onay sorulmaz. Ancak gerçek araç/dosya izinleri gerektiğinde istenir; güvenlik sınırları atlanmaz.
- Mevcut kullanıcı değişiklikleri korunur. Gereksiz yeniden yazım, paket güncelleme, otomatik commit/push yok.
- Görsel üretimi önceki onaylı karakterlere referansla asistan tarafından yapılır. Kullanıcı yerel görsel uygulamasını uzun UI döngüleriyle kontrol etme yöntemini token/zaman maliyeti nedeniyle bıraktı.
- Kısa güncellemeler, dar kapsamlı incelemeler ve küçük test paketleri tercih edilir. Yeni mekanikleri önce işlevsel doğrula; gereksiz çok sayıda final varlık üretme.

## 10. Kısa karar günlüğü

- 19–20 Eylül: Bezelye B, daha sık atış, sersemleme direnci, darbe ve ölüm sunumu kullanıcı testleriyle onaylandı.
- 20 Eylül: Hareket paketi + yakın/uzak/özel savaş paketi kabul edildi; uzak silah sos yerine çatal olarak seçildi.
- 20 Eylül: Şefin darbe animasyonu ilk hareket test paketine eklendi.
- 20 Eylül: Küçük hub + çoğunlukla lineer bölümler yönü; başlangıçta spatula/çatal, ilerledikçe yeni silahlar, hub'da bir yakın/bir uzak seçim sistemi kabul edildi.
- 20 Eylül: Bu kararların kalıcı not olması ve sonraki tüm bölüm tasarımlarında kullanılması istendi.
- 20 Eylül: Hikâyenin temeline bölüm tasarımından önce birlikte karar verilmesi onaylandı; ayrıntılı anlatım üretimi daha sonraya bırakıldı. Belirli bir hikâye henüz seçilmedi.
- 20 Eylül: İlk hareket ve şef darbe paketi ayrı KitchenMovement_Test sahnesine uygulandı. Dört yeni referanslı şef pozu üretildi. Play/build/otomatik oyun testi yapılmadı; kullanıcı testi bekleniyor.
- 20 Eylül, ilk test geri bildirimi: çoğu mekanik çalışıyor; koşma belirsiz, eğilerek yürüme eksik, ikinci sıçrama zayıf, darbe animasyonu yetersiz. Kullanıcı hava dash'ini zorunlu kıldı ve bölümlerin buna göre tasarlanmasını istedi. İlk dört düzeltme uygulandı, yeniden test bekleniyor; darbe sunumu sonraki iş. Önceki yerde-dash kısıtı geçersizdir.
- 20 Eylül, animasyon revizyonu: kullanıcı çömelerek yürüyüşün daha doğal/akıcı olmasını ve darbe animasyonunu istedi. Sekiz çömelme adım pozu ve ayakta/çömelmiş üçer darbe pozu Movement test sahnesine bağlandı. Eski görseller korundu. Kullanıcı testi bekleniyor. Çalışma sırasında kullanıcı Play'e girdi; kod yeniden yükleme sırasında InputReader/MeleeFeedback NullReference hataları görüldü. Kullanıcı Play'i durdurduktan sonra sahne kaydedildi; temiz yeni Play oturumu gerekli, hatalar tekrar ederse ayrı incelenecek. Asistan Play/build/oynanış testi başlatmadı.
- 20 Eylül, oyun duraklama hatası: konsol `TomatoLocomotionVisual.LateUpdate:114 IndexOutOfRangeException` bildirdi. Hata anında Play açık/duraklamıştı; yürüyüş dizisi 6 eleman ve `_frameClock` 6 olarak okundu. Bu kayıt önceki InputReader/MeleeFeedback yeniden yükleme hatalarından farklıdır. Domatesin kare saati sonlu değer kontrolü, üst uçta sıfırlama ve 0..Length-1 indeks sınırıyla düzeltildi. Şefin çömelme/koşu/aksiyon kare seçimlerine de sınır koruması eklendi. Kullanıcı Play'i durdurduktan sonra uygulandı; oynanış tekrar testi bekleniyor. Animasyon görselleri, hız ve savaş dengesi değiştirilmedi.
- 20 Eylül, çömelmiş ölüm: kullanıcı ölüm anında şefin önce doğrulduğunu bildirdi. `ChefLocomotionVisual` artık Died sırasında IsCrouching durumuna göre altı karelik ayrı ölüm dizisini bir kez seçer; zemin genişliği hesabı da seçilen diziyi kullanır. SuspendForDeath zaten çömelmiş collider durumunu korur; respawn ayakta hâli geri getirir. `KitchenMovement_Test` sahnesine `Assets/Art/Characters/ChefA/Movement/ChefA_CrouchDeath_01_v01.png`–`06_v01.png` bağlandı ve kaydedildi. Ayakta ölüm dosyaları ve sağlık/respawn zamanlamaları değiştirilmedi. Yerleşik görsel üretimi, mevcut crouch referansı; prompt: `/Users/alicanozturk/Documents/Codex/2026-09-16/https-www-youtube-com-watch-v/work/chef-crouch-death-prompt.txt`. Ayırma/ankraj ve uygulama kayıtları: çalışma alanında `work/prepare-crouch-death.cjs`, `outputs/chef-crouch-death/import.json`, `work/apply-crouch-death.cs`. Kullanıcı testi bekleniyor: çömelmiş sabit/yürüyen ölüm, ölümden sonra aşağı tuşunu bırakma, ayakta ölüm, checkpoint dönüşü ve çukur ölümü. Asistan Play/build/otomatik test başlatmadı.
