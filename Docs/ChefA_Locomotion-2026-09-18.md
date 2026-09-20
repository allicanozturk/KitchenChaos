# Chef A — bekleme ve koşu denemesi

Durum: 18 Eylül 2026, prototipe bağlandı; oynanış ve animasyon ritmi kullanıcı onayı bekliyor. Son kalite animasyon seti değildir.

Güncelleme: Kullanıcı koşu/duruş oran farkı ve zor görülen nefes bildirdi. Aynı gün yeni, koşu oranlarına uygun idle çizimi bağlandı; nefes yüksekliği %2.4, genişliği %1.2, süresi 2.2 saniye oldu. Aşağıdaki ilk entegrasyon ölçüleri tarihsel kayıttır; güncel düzeltme `ChefA_ProportionFix-2026-09-18.md` içinde.

## Kapsam

- `ChefLocomotionVisual` yalnız `Player_Prototype.prefab` üzerinde etkin. Eski sahnelere otomatik olarak eklenmedi.
- Beklerken ayak pivotunu koruyan 2.4 saniyelik yumuşak nefes: görsel genişlik en fazla %0.35, yükseklik %0.8 artar. Fizik kökü/kapsül ölçeklenmez.
- Koşu: altı sprite; hız 5 iken saniyede 10 kare, ölçülen yatay hıza göre ilerleme. Hareket isteği, gerçek hız ve yerde olma birlikte aranır; duvara yaslanma veya hareketli platformda durma koşu başlatmamalı.
- Havada, saldırı sırasında, ölümde veya kontrol kilitliyken onaylı bekleme çizimi kullanılır. Bu pakette zıplama/saldırı/ölüm gövde animasyonu üretilmedi. Mevcut spatula savuruşu, hasar rengi ve ölüm solması devam eder.
- Jumped olayı ilk kalkışta zeminin birkaç fizik adımı hâlâ algılanmasına karşı görsel koşuyu durdurur. Respawn ve devre dışı bırakma kare/süre/ölçeği sıfırlar.
- `PlayerVisual` yön/flipX, `PlayerDamageFeedback` renk/alpha sahibi olarak kaldı. Eski Animator ve kapalı root SpriteRenderer korundu.
- `PlayerMeleeFeedback` opsiyonel görsel referansından her karenin avuç konumunu dünya koordinatında alır. Yön ve nefes bu noktaya bir kez uygulanır; ikinci kez aynalanmaz. Referans yoksa eski tutuş kullanılır. Silah boyu ve hasar alanı değişmez.

## Varlıklar ve hizalama

- Üretim 17 Eylül'de yerleşik `image_gen` ile yapıldı; bu devam adımında yeni görsel üretilmedi, mevcut PNG'ler düzenlenmedi. CLI/API anahtarı kullanılmadı.
- İlk üretim: `/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-e567eda9-d93b-407c-a08c-7c91b341d6a6.png`.
- Bacak fazı düzeltmesi: aynı klasörde `exec-8fa539af-5e21-4be9-a757-e97f73c35fd1.png`.
- Önceki dosya ve 25 otomatik parça içeren kesim ayarları `Assets/Art/Characters/ChefA/ChefA_Run_Atlas_v01.png` üzerinde korundu.
- Entegrasyon kopyası: `Assets/Art/Characters/ChefA/ChefA_Run_Atlas_v02.png`. Piksel içeriği v01 ile aynı; yalnız Unity içe aktarma/kesim ayarları farklı.
- PNG: 1536×1024 RGBA, gerçek alpha. SHA256: `6a6523ea948ad456047b70d208c10a6ad6928eb648ffbb7d70b737af2945fb96`.
- Altı eşit 512×512 hücre; üst sıra soldan sağa, ardından alt sıra. `ChefA_Run_01`…`ChefA_Run_06`.
- PPU 122; Full Rect, bilinear, sıkıştırmasız, mipmap kapalı, Clamp, alpha transparency açık. Otomatik fizik şekli kapalı. Sprite ID eşlemesi Sprite Editor data provider ile kaydedildi.
- Taban çizgisi hücre üstünden 504 piksel; tüm pivot Y değerleri alttan 8 piksel. Birkaç karede ayakların bu çizgiden hafif yükselmesi adım pozunun parçası.
- X pivotları: 311.5 / 269.5 / 248.5 / 304.5 / 265.5 / 259.5 piksel. Yüzün atlas içindeki yatay kaymasını telafi eder; karakter boyu yaklaşık 4 dünya birimi kalır.
- Her karede ayrı avuç noktası vardır. Sprite yerel koordinatları sırasıyla: (0.412459,1.307131), (0.334918,1.204836), (0.432377,1.309426), (0.470082,1.340574), (0.359836,1.287787), (0.371311,1.272213).
- Bekleme noktası önce onaylanan (0.3525671,1.37715398). Sap parçaları yine şefin arkasında; boyun/baş önde çizilir.

## Doğrulama ve sınırlar

- Kesintinin bıraktığı eksik `ChefLocomotionVisual` sınıfına bağlı CS0246 giderildi; yeni bileşen Unity'ye içe aktarıldı ve tür/prefab bağlantıları okunabildi. Oyun build'i alınmadı.
- Prefab kaydından önce Rigidbody, Collider, hareket, zıplama, saldırı bileşenleri ve kök/görsel/spatula pivot transformları serialized karşılaştırıldı; değiştirilmedikleri doğrulandı.
- Hareket 5, zıplama 11, gravity 3, kapsül (.8,2), kök ölçek (1,2,1), kamera ortho 8; saldırı yarıçapı .75 ve .1/.12/.18 zamanları korundu.
- Altı dolu sprite referansı, altı avuç noktası, melee bağlantısı ve onaylı idle görseli doğrulandı. `git diff --check` temiz.
- Yalnız Edit Mode'da idle ve run 1/2 sağ, run 6 sol statik pozları görüntülendi; geçici sprite/yön/pivot ve Scene View tercihleri geri yüklendi. Aktif sahne temiz, Play Mode kapalı bırakıldı. Kare döngüsü veya oyun otomatik oynatılmadı.
- Statik görüntüler: `Captures/CharacterReview/ChefA_Locomotion_Idle_2026-09-18.png`, `ChefA_Run1_Right_Static_2026-09-18.png`, `ChefA_Run2_Right_Static_2026-09-18.png`, `ChefA_Run6_Left_Static_2026-09-18.png`.
- Statik görünüm geri yüklemesinde 2D Scene View'a rotation atanmasına ilişkin editör uyarısı oluştu; oyun scripti hatası değildir. Console temizlenmedi. Gelecek incelemelerde 2D Scene View rotation atlanmalı.
- İlk koşu çizimlerinde idle çizimine göre yüz/giysi oranlarında küçük farklar var; geçiş ve adım ritmi oynanışta değerlendirilmeli. Bu bir yöntem/okunabilirlik denemesidir; nihai temizlenmiş animasyon seti olduğu iddia edilmez.
- Önceden bilinen görsel–kapsül genişliği farkı sürer; bu görevde çarpışma sınırı değiştirilmedi.
- `Level01.unity` SHA256 hâlâ `acf17dc1bf299db961ac932d8a1e1bff78edda525651f58501d8cc1d2eb589f2`. Orijinal idle PNG değişmedi.

## Kullanıcının kontrol listesi

1. KitchenIntro_Blockout sahnesinde 3–4 saniye bekle: hafif nefes görünmeli; ayaklar ve spatula ayrı ayrı kaymamalı.
2. Sağa/sola koş ve dur: adımlar oynayıp durunca bekleme pozuna dönmeli; el–spatula bağı iki yönde korunmalı. Yüz/boyut değişimi rahatsız ediyor mu, adım ritmi çok hızlı/yavaş mı?
3. Duvara dayan ve hareketli platformda dur: yerinde koşma olmamalı.
4. Koşarken zıpla ve saldır: hareket/menzil önceki gibi kalmalı. Bu pakette havada/saldırıda bekleme gövde pozuna dönüş normal; koşu havada devam etmemeli.
5. Hasar al, öl ve checkpoint'e dön: renk/solma sürmeli; eski karakter görünmemeli, koşu veya büyütülmüş nefes ölçeği takılı kalmamalı.

## Üretim istemleri (17 Eylül, aynen)

### İlk atlas

Use case: identity-preserve.
Asset type: transparent 2D side-scrolling game RUN ANIMATION SPRITE SHEET, a single production atlas with six consecutive frames.
Input image 1 is the approved Chef A character identity and style reference/edit target. Preserve this exact elderly, very pot-bellied friendly human chef: same large folded cream chef hat, same round face and nose, fluffy white mustache, white side hair and brows, petrol-blue neckerchief, short cream rolled sleeves, cream apron with orange sauce and beige flour stains, loose charcoal trousers and broad dark kitchen shoes. Keep the same hand-drawn ink contour and softly painted shading. Do not redesign, slim, rejuvenate, change the face, change palette, or vary outfit between frames.
Create SIX sequential poses of a grounded brisk jog toward the RIGHT, in an EXACT regular 3-column by 2-row grid, order left-to-right top row then left-to-right bottom row. Canvas landscape 3:2, ideally 3072x2048. Each of the six equal SQUARE cells contains exactly one complete full-body chef with generous transparent margin, no overlap with other cells. Same camera, character scale, head proportions, light and consistent body silhouette in every cell. Side view matching the reference; no camera turns.
Animation: frame1 near leg forward heel contact/far leg back; frame2 weight down, knees flexed; frame3 near leg passing under body/far knee lifting; frame4 reversed contact far leg forward/near leg back; frame5 opposite down; frame6 opposite passing pose ready to loop to1. Distinct credible foot progression, modest grounded stride and weight suitable for a heavy older chef, not sprinting or leaping. Two legs, two feet only. Near arm and EMPTY closed gripping fist stay LOW at the front side of the belly, about 36 percent of body height from the soles, with a small natural counter-swing. Fist is clearly drawn in all six frames so a separate game weapon can be attached; do not hide fist behind apron. Far arm mostly occluded.
Keep consistent head/hat scale and position within each cell, with only tiny vertical gait bob; same baseline near 94% of cell height. Full hat and shoes must be inside each cell. The body and face must not morph between frames.
BACKGROUND: actual transparent RGBA, zero alpha outside chef silhouettes and between legs. No black/white/color background, no halo, no glow, no floor or cast shadow, no checkerboard painted into pixels. No grid lines, frame numbers, labels, text, watermark, objects, weapon, or spatula. Exactly six characters, not five, seven, or eight. This is a practical sprite atlas, not a concept-art presentation.

### Bacak fazı düzeltmesi

Use case: identity-preserve.
Image 1 is the EDIT TARGET, the six-frame Chef A running sprite atlas. Keep all six heads, faces, hats, torsos, aprons, arm/fist poses, shading, cell placement and character identity unchanged. Keep the top row completely unchanged. Make ONLY the following animation correction to LOWER BODY of the BOTTOM THREE CELLS: these must be the OPPOSITE-LEG phase of the top row, not duplicates. In bottom-left cell, the near leg (coming from the LEFT/lower foreground side of his hip in this right-facing profile) must extend FORWARD to the RIGHT with its foreground shoe visibly planted ahead; the far leg swings BACK to the LEFT behind it. In bottom-middle the near forward leg takes weight with knee flexed, far leg passing behind. In bottom-right the far leg passes forward with raised knee while near leg trails but remains visible. Clear alternating left/right leg cycle, anatomically connected thighs, no extra legs/feet. Leave the top-row running poses and every upper-body part intact. All six equal 512-by-512 cells in the existing 1536-by-1024 3-column 2-row atlas remain the same dimensions; no crop, no rescale, no re-layout. Genuine transparent RGBA background, zero alpha outside silhouettes, no halos, no floor, no shadows, no grid, no text, no weapon. Preserve the exact chef and scale.
