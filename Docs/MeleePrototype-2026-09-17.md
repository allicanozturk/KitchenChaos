# Yakın dövüş prototipi — 17 Eylül 2026

Durum: kullanıcı tüm yakın dövüş testlerinin başarılı olduğunu bildirdi. Spatulanın biraz uzun olduğu not edildi; kullanıcının isteğiyle küçültme sonraya bırakıldı. Bu onay, daha sonra eklenen Chef A görsel yerleştirmesini kapsamaz. Asistan Play Mode, test koşucusu veya otomatik giriş başlatmadı; kaynak, içe aktarma ve Console/serialized referans kontrolü yaptı.

## Saldırı kuralı

- Süreler: 0.10 saniye hazırlık, 0.12 saniye aktif vuruş, 0.18 saniye toparlanma. Toplam yaklaşık 0.4 saniye; faz sınırları fizik adımlarında uygulanır.
- Hasar yalnız aktif fazdaki fizik sorgularında uygulanır. Bir düşmanın birden çok collider'ı olsa veya aktif faz boyunca menzilde kalsa bile aynı savuruşta bir kez hasar alır. Hasar 1, yarıçap 0.75, yerel saldırı merkezi (±0.65, -0.35) olarak korundu.
- Hazırlık/toparlanma hasar vermez. Boşa sallamak oyuncuyu korumaz; hazırlık sırasında düşman hâlâ vurabilir. Gerçek isabetten sonra mevcut 0.45 saniyelik düşman sersemlemesi devreye girer.
- Hareket/zıplama kilitlenmez. Bakış ve saldırı yönü bütün savuruş boyunca sabittir; ters yöne yürümek aynı saldırıyı arkaya taşımaz. Sonraki saldırıda yeni yöne dönülebilir.
- Basılı tutma otomatik saldırı değildir. Meşgul fazlarda basılan tuşlar sonraki saldırı için sıraya alınmaz.
- Ölüm, respawn veya Attack bileşenini kapatma fazı ve tek-vuruş hafızasını temizler. Gecikmeli coroutine/Animation Event hasarı yoktur.

## Geçici sunum

- `Player_Prototype` altında kare sprite parçalarından oluşturulmuş ahşap saplı, üç yarıklı metal spatula bulunur. Yapay zekâ ile üretilmiş bitmiş sprite seti değil, değiştirilebilir kod tabanlı prototip geometrisidir.
- Silah hazırlıkta kalkar, aktif aralıkta savrulur, sonra bekleme pozuna döner. Sarı yay yalnız aktif aralığı gösterir. Hasar kuralları ile bu animasyon aynı faz saatini okur.
- Oyuncu kökü (1,2,1) ölçeğini korur. Silahın (1,0.5,1) üst nesnesi deformasyonu önler; yalnız görsel pivot döner. Yeni collider veya Rigidbody eklenmedi.
- Eski karakterin yumruk animasyonu yalnız bu prototip prefabında tetiklenmez; onun ayrı zamanlaması ile spatula savuruşunun çakışması önlendi. Idle/run/jump/fall, hasar tepkisi ve mevcut animasyon dosyaları korunur. Nihai karakter saldırı animasyonu sanat örneği aşamasında hazırlanacak.
- Gerçek can azalmasında kısa sarı/beyaz kıvılcım görünür. Dört küçük efekt yuvası oyuncuda tutulur; kıvılcım dünyadaki isabet noktasında kalır ve öldürücü vuruşta düşmanla birlikte yok olmaz. Ölüm/respawn eski efektleri temizler.
- Savurma sesi aktif faz başında, isabet sesi yalnız kabul edilen vuruşta çalar. Çok hedefli bir savuruşta üst üste yüksek ses oluşmaması için tek isabet sesi kullanılır; her hedefin görsel isabeti ayrıdır.

## Ses kaynağı

Yeni `Assets/Audio/Placeholder/SFX_SpatulaImpact_Placeholder.wav`: 0.15 saniye, mono, 44.1 kHz PCM16; en yüksek örnek genliği 0.60. Kısa bas darbesi, sönümlenen metalik sinüsler ve sabit tohumlu gürültüyle bu proje için prosedürel üretildi. Üçüncü taraf kayıt veya indirilen örnek içermiyor; geçici efekt.

Tekrar üretim aracı: `Tools/Audio/generate_spatula_impact.py`. Çıktı yolu argümanı alır ve mevcut dosyanın üzerine yazmayı reddeder. Mevcut savurma, zıplama, oyuncu hasarı, coin ve düşman ölüm sesleri değiştirilmedi.

## Kullanıcının kısa kontrol listesi

`KitchenIntro_Blockout` sahnesinde mevcut saldırı tuşunu kullan:

1. Güvenli alanda sağa ve sola birer kez sallan. Spatula görünmeli; kaldırma/savurma/toparlanma ayırt edilmeli. Boş vuruşta savurma sesi olmalı ama temas kıvılcımı/metalik isabet sesi olmamalı.
2. Düşmanın temasına girmeden menzilin ucundan bir kez vur. Savurma sırasında can çubuğu yalnız bir kademe azalsın; kıvılcım ve isabet sesi aynı olayda olsun. Sağdan ve soldan dene.
3. Başka saldırı yapmadan menzilde kal: aynı savuruş canı tekrar azaltmamalı. Üç ayrı isabet düşmanı öldürmeli; son isabette de kıvılcım/ses görünmeli/duyulmalı.
4. Sallanırken ters yöne yürü. Hareket devam etsin ama silah ve bakış o saldırı bitmeden taraf değiştirmesin. Sonraki saldırının yeni yöne döndüğünü kontrol et. Havada saldırıyı da dene.
5. Saldırı sırasında öl ve checkpoint'e dön. Spatula/efekt takılı kalmasın; eski saldırı kendiliğinden devam etmesin. Yeni basışla saldırı çalışsın. Önceki kamera, basamak, hasar koruması ve checkpoint davranışları korunmalı.

İlk geri bildirimde özellikle silahın büyüklüğünü, isabetin görsel menzille uyumunu, saldırının gecikmeli/ağır hissettirip hissettirmediğini ve isabet sesinin seviyesini belirt.

## Kapsam

Ortak `PlayerAttack`, `PlayerVisual`, `PlayerAudio` scriptleri güncellendi; eski sahneler de yeni saldırı fazları/yön kurallarını kullanır. Spatula/efekt ve isabet sesinin bağlantısı yalnız prototip prefabındadır. `Level01.unity`, mevcut animasyon dosyaları, düşman sersemlemesi ve oyuncunun onaylanmış fizik/sağlık ayarlarına dokunulmadı. Build, commit, push, paket güncellemesi yapılmadı.
