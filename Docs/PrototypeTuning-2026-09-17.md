# Prototip ayarı — 17 Eylül 2026

Kullanıcının oynanış geri bildirimine göre düzenlendi. Bu sürümün oynanış testi kullanıcıya bırakıldı; asistan Play Mode, otomatik giriş veya test koşucusu çalıştırmadı. Önceki `PrototypeChecks.md`, ilk yerleşimin tarihsel kontrol kaydıdır ve bu değişikliklerin test sonucu değildir.

## Değişiklikler

- `KitchenIntro_Blockout`: Main Camera ve Cinemachine ortografik büyüklük 6.5 → 8. Daha geniş görüş; fizik ölçüleri aynı.
- `PatrolEncounter_Prototype`: düşman ölçeği (2.5, 2.5) → (1.6, 2.5). Yüksekliği/zemin hizası aynı; görsel ve çarpışma kutusu birlikte daraldı.
- Büyük basamağın sağına `Step_03_Return` eklendi: merkez (18.5, -1.875), boyut (3, 1.25). Zeminden ara basamağa, oradan büyük basamağa iki adet 1.25 birimlik yükseliş var. Aradaki yatay açıklık 0.5 birim. Zıplama ayarları (hız 11, gravityScale 3) değiştirilmedi.
- Yeni, isteğe bağlı `EnemyHitStun`: başarılı hasardan sonra 0.45 saniye devriye ve temas hasarı durur. Yalnızca prototip düşman prefabına eklendi.
- `EnemyHealth.Damaged` olayı hasar alındığı anda senkron çalışır. `EnemyPatrol` ve `EnemyContactDamage` ortak sersemleme durumunu kontrol eder. Görsel titreme/can çubuğu ayrı `EnemyHitFeedback` bileşeninde kaldı.

## Savaş kuralı

Boşa saldırmak oyuncuya dokunulmazlık sağlamaz. İsabet alan düşmanın zarar verme yeteneği kısa süre durdurulur; diğer düşmanlar ve hazard'lar bu mekanizmadan etkilenmez. Sersemleme bitince temas devam ediyorsa düşman yeniden hasar verebilir. Yeni isabet süreyi son vuruştan itibaren yeniler; süreler üst üste eklenmez.

Mevcut saldırı aralığı 0.4, sersemleme 0.45 saniye olduğu için zamanında art arda isabet ettirmek bu ilk düşmanı durdurabilir. Bu, başlangıç karşılaşması için bilinçli kolaylaştırıcı ayardır. Daha önce alınmış hasar geri verilmez.

Kinematik gövdenin aynı fizik adımında önceden sıraya koyduğu hareket de isabet anında mevcut konuma `MovePosition` çağrısıyla iptal edilir. Hurtbox/collider kapatılmaz; sonraki vuruşlar düşmanı bulabilir. Düşman ölünce mevcut ölüm ve ses akışı devam eder.

## Kullanıcının deneyecekleri

1. Kamera: karakter, düşman ve platformlar daha rahat okunuyor mu? HUD aynı büyüklükte kalmalı.
2. Gidiş-dönüş: büyük basamaktan sağa in, sonra yeni ara basamak üzerinden sola geri çık. Köşeye takılmadan her iki yönde ilerleyebiliyor musun?
3. İsabet: düşmana temas etmeden biraz önce vur. Can çubuğu azalırken düşman kısa süre durmalı; o aralıkta aynı düşmanın teması canını azaltmamalı.
4. Süre sonu: bir kez vurduktan sonra saldırıyı bırak ve düşmanın temasında kal. Sersemleme bitince yeniden hasar verebilmeli. Düşmana isabet etmeyen saldırı aynı korumayı sağlamamalı.
5. Ölüm: art arda üç başarılı vuruşla düşman kaldırılmalı; oyuncunun checkpoint akışı bozulmamalı.

## Kapsam ve kayıt

`Level01.unity` dosyasına dokunulmadı. Ortak üç düşman scriptine küçük eklemeler yapıldı; EnemyHitStun olmayan etkin düşmanlar mevcut davranışlarını sürdürür. Oyuncu/hazard/ses scriptleri değiştirilmedi. Commit/push veya paket güncellemesi yapılmadı.

Kaynak incelemesi ve Unity'nin script içe aktarımı sırasında Console kontrolü yapıldı. Kullanıcı bu ayarları oynayarak denedi ve “evet hepsi çalıştı” diyerek onayladı. Bu onay, sonraki oyuncu hasar/ölüm değişikliklerini kapsamaz.
