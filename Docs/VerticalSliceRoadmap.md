# KitchenChaos — ilk oynanabilir bölüm

> 20 Eylül 2026 notu: Bu dosya eski aşama ve uygulama geçmişini korur. Güncel tasarım kararları ve tamamlanma durumu için önce [ProjectDecisions.md](ProjectDecisions.md) okunmalıdır. Aşağıdaki biber, ilk uzak silah olarak bıçak, eksik animasyonlar ve eski bölüm öğretim sırası güncel plan değildir. Biber bezelyeyle değişti; ilk silahlar spatula/çatal; yeni hareket paketi ve şefin darbe tepkisi, ayrıntılı bölüm tasarımından önce gelir.

Karar tarihi: 16 Eylül 2026. Bu belge hedef ve kabul ölçütüdür; tamamlanmış özellik listesi değildir.

## Sabit kapsam

İlk kez oynayan biri için yaklaşık 10 dakikalık, başı ve sonu olan tek mutfak bölümü. Süre oynanış testleriyle ölçülecek; yürüyüşü uzatarak veya düşman canını şişirerek doldurulmayacak.

- Bir ana karakter.
- İki düşman türü: yakın temas/devriye domates ve atışını önceden belli eden menzilli biber. Üçüncü tür ilk kabulden sonraya bırakılır.
- Bir yakın dövüş silahı (tava/spatula) ve bir uzak saldırı (fırlatılan mutfak bıçağı). İlk prototipte uzak saldırı bekleme süreli, cephanesizdir.
- Bir tost makinesi boss: okunabilir iki saldırı, aralarında belirgin savunmasızlık penceresi. İki silahla da yenilebilir. Ek düşman dalgası ve ölümcül çukur yok.
- Checkpoint, ölüm/yeniden doğma, anlaşılır HUD, bölüm bitişi ve tekrar oynama.
- Tamamlanan örnek bölümde tutarlı mutfak sanatı, temel karakter/düşman animasyonları, vuruş/hasar geri bildirimi ve sesler bulunacak. İlk yerleşim denemesi geçici şekiller kullanır.

## Çalışma ilkeleri

Mevcut çalışan kod yeniden yazılmayacak. `Level01` ve kullanıcının Sprint16 ses değişiklikleri korunacak. Yeni yerleşim `Assets/Scenes/KitchenIntro_Blockout.unity` içinde yapılacak. Deneme prefabları `Assets/Prefabs/VerticalSlice` içinde tutulacak. Mevcut scriptler ortak olduğundan sonraki kod değişiklikleri hem eski hem yeni sahnede kontrol edilecek.

Build almak bu aşamanın şartı değil. Önce sahne/referans kontrolü ve Editor Play Mode denemesi yapılır. Paket güncelleme, geniş mimari dönüşüm, otomatik commit/push yok.

17 Eylül çalışma kararı: Play Mode ve oynanış testlerini kullanıcı yapar. Asistan otomatik oynanış, sanal klavye veya test koşucusu başlatmaz; değişiklikleri uygular, kaynak/bağlantıları inceler ve kullanıcıya kısa bir test listesi bırakır. Yeni C# dosyalarının Unity tarafından normal içe aktarılması ve derleme mesajlarının okunması bu süreçten ayrıdır; ayrıca oyun build'i alınmaz.

## Aşamalar ve çıkış ölçütleri

| Aşama | Yapılacak iş | Bir sonrakine geçiş koşulu |
| --- | --- | --- |
| 1 — Temel yerleşim | Mevcut oyuncu, kamera, HUD ve seslerle güvenli giriş, iki alçak basamak, checkpoint ve tek düşman; tekrar kullanılabilir prefablar | Başlangıç güvenli; rota okunur; basamaklar aşılır; düşmana iki yönden vurulur; ölüm checkpoint'e döndürür |
| 2 — Savaş hissi | Yön bilgisini görselden ayırma; küçük ortak hasar sözleşmesi; saldırının aktif vuruş zamanı; kısa hasar dokunulmazlığı; koordineli ölüm/respawn | Çift hasar ve çukur dokunulmazlığı hatası yok; saldırı, ses ve temas aynı olaya bağlı; ölüm sonrası geçici durum temiz |
| 3 — Temsilî kalite ve çeşitlilik | Bir karakter + bir düşman + tek mutfak köşesinde sanat denemesi; yakın silahı tamamla; uzak silah ve biber ekle | Stil ve okunabilirlik onaylı; iki silah farklı bir ihtiyacı karşılıyor; düşman atışı anlaşılır biçimde haber veriliyor |
| 4 — Bütün bölüm | Aşağıdaki yedi alan, checkpoint aralıkları, tost makinesi boss, çıkış akışı | Baştan sona kesintisiz oynanıyor; boss öncesi tekrar kısa; eksik ilerleme/softlock yok |
| 5 — Cila ve kabul | Son stile yakın çevre, karakter animasyonları, VFX, mutfak sesleri, miks, kamera ve zorluk ayarı | İlk kez oynayan kişilerle süre ve anlaşılabilirlik ölçülmüş; kritik hata yok; görsel/ses varlıklarının kaynak/lisansları kayıtlı |

Bu bölüm onaylanmadan dört saatlik oyunun bütün düşmanları, dünyaları ve silahları üretilmez. Pooling, genel servis mimarisi ve kapsamlı kayıt sistemi gerçek ihtiyaç oldukça eklenir.

## Bölüm akışı

1. Güvenli giriş: hareket, zıplama, iki alçak basamak ve ödül yönlendirmesi. Tehlike yok.
2. Hazırlık tezgâhı: geniş düz zeminde yalnız domates; yakın dövüşü öğrenme. Kör inişte düşman yok.
3. Bulaşık hattı: önce altında güvenli zemin bulunan hareketli platform; sonra öğrenilen hareketin kısa uygulaması.
4. Baharat rafı: uzak silah güvenle alınır; tek biber ve siperli karşılaşma. Çıkışta checkpoint.
5. Servis hattı: bilinen düşman ve platform fikirleri sırayla birleştirilir; yeni kurallar aynı anda yüklenmez.
6. Fırın kapısı: iyileşme, checkpoint, boss'a kısa dönüş.
7. Boss: düz arena; alçak ekmek atışını zıplayarak, işaretli kırıntı düşüşünü yer değiştirerek aşma. Kapak açılınca saldırma; zafer çıkışı.

## İlk yerleşim denemesi

Hedef: keşif ve ilk karşılaşma dahil yaklaşık 30–60 saniyelik tasarım denemesi; henüz ölçülmüş süre değil. On dakikalık bölümün kendisi değil.

| Bölge | Yaklaşık X aralığı | Amaç |
| --- | --- | --- |
| Giriş | 0–7 | Güvenli hareket, kontrol hatırlatması, iki ödül |
| Basamaklar | 8–18 | Birbirini izleyen iki düşük sıçrama; altta kesintisiz güvenli zemin |
| Nefes / checkpoint | 20–28 | Güvenli iniş ve kısa yeniden deneme mesafesi |
| İlk karşılaşma | 31–38 | Tek devriye düşmanı, düz zemin, saldırı denemesi |
| Çıkış işareti | 42–49 | Ödül ve prototipin sonunu açıkça belirtme; henüz bölüm geçişi yok |

Koordinatlar başlangıç yerleşimidir. Mevcut hareket hızı, gerçek sıçrama yayı, karakter boyutu ve kamera üzerinden oynanarak ayarlanır. Mevcut animasyon/seslerin tekrar kullanılması bunların son kalite olduğu anlamına gelmez.

## Yeniden doğma kararı

Hedef davranış: açılmış silahları koru, aktif karşılaşmayı tutarlı biçimde sıfırla, toplanan ödüllerden sınırsız puan üretme. Karşılaşma sıfırlama ve silah durumunun korunması 2–4. aşamada uygulanıp test edilecek; yok edilen düşmanlar şimdilik geri gelmez.

17 Eylül ilerlemesi: kamera, basamak dönüşü, düşman boyutu, isabet sersemlemesi ve oyuncunun hasar/ölüm/respawn akışı kullanıcı tarafından onaylandı. Oyuncuda 0.75 saniye hasar koruması, 0.35 saniye ölüm geçişi ve checkpoint dönüşünde 0.8 saniye koruma bulunur; geçici hareket, zıplama ve saldırı durumu temizlenir. Skor ve toplanmış coin'ler korunur. Ayrıntılar `PlayerDamageFlow-2026-09-17.md` içinde.

Yakın dövüş paketi kullanıcı tarafından onaylandı: yön/erişim görselden ayrıldı; hazırlık–aktif vuruş–toparlanma akışı, geçici spatula, isabete bağlı efekt/ses bağlandı. Spatulanın uzunluğu kullanıcı isteğiyle şimdilik korunuyor. Ayrıntılar `MeleePrototype-2026-09-17.md` içinde. Ortak hasar arayüzü henüz eklenmedi; menzilli silah/boss entegrasyonunda gerçek ihtiyaçla birlikte ele alınacak.

## Görsel ve ses üretimi

Önce stil/palet, karakter ölçeği ve kamera çözünürlüğü kararlaştırılır. Konsept PNG'ler oynanabilir sprite veya animasyonun otomatik olarak hazır olduğu anlamına gelmez. Tek karakterin idle/run/jump/fall/attack/hurt/death seti ve tek düşman örneğiyle üretim yöntemi denenir; gerekirse kare temizliği veya parçalı animasyon için ek araç kullanılır. Mevcut placeholder sesler korunur. Son seslerde kayıt/üretim kaynağı ve kullanım lisansı tutulur.

Karakter kararı: insan, yaşlı, göbekli, beyaz bıyıklı, sıcak yüzlü usta şef; lekeli krem önlük ve petrol mavisi fular. Üçlü konseptte kullanıcı A seçeneğini seçti. İlk sağa bakan tek duruş `Player_Prototype/Chef_Visual` üzerine bağlandı; hareket/çarpışma ayarları değişmedi. Kullanıcı genel görünümü ve son tutuş düzeltmesini onayladı. Kaynak, üretim istemi ve kısıtlar `ChefA_StaticFit-2026-09-17.md` içinde.

18 Eylül: hafif bekleme nefesi, altı karelik koşu döngüsü ve kareye bağlı spatula tutuşu prototipe eklendi. Kaynak/serialized referans ve statik Edit Mode görünümü incelendi; oynanış testi kullanıcıya bırakıldı. Zıplama, saldırı ve ölüm gövde animasyonları henüz yok; bu hallerde onaylı bekleme çizimi kullanılır. Son kalite seti değildir; ritim ve idle/run çizim geçişi onaydan sonra iyileştirilecek. Ayrıntılar ve test listesi `ChefA_Locomotion-2026-09-18.md` içinde.

## İş bölümü

Asistan: C# değişiklikleri, prefab/sahne kurulumu, ilk platform ve düşman yerleşimi, araçlarla yapılabilen varlık üretimi ve teknik doğrulama. Kullanıcı: kısa oynanış testleri, hareket/savaş hissi ve görsel tarz onayı. Kullanıcının her platformu elle yerleştirmesi veya baştan Unity uzmanı olması gerekmiyor; kararları küçük oynanabilir adımlarla birlikte veririz.
