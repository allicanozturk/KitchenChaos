# Şef ölüm animasyonu entegrasyonu — 2026-09-19

## Durum ve kapsam

Kullanıcının onayladığı altı ölüm pozu Player_Prototype prefabına bağlandı. Yalnızca şefin ölüm sunumu ve bu prefabın ölüm bekleme süresi değişti. Domatesin ölüm animasyonu henüz eklenmedi; önce şefin kullanıcı oynanış kontrolü alınacak.

Unity Play Mode, oynanış/otomatik input, test runner veya build çalıştırılmadı. Normal script import/derlemesi, referans kontrolleri, kod incelemesi ve Edit Mode statik poz önizlemesi yapıldı.

## Kullanılan görsel

Onaylı kaynak:
`/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-b17c5f76-51d5-4a26-83d1-6e8b0a0d1e43.png`

Projede kullanılan birebir kopya:
`/Users/alicanozturk/Projeler/KitchenChaos/Assets/Art/Characters/ChefA/ChefA_Death_Grumpy_Atlas_v01.png`

Önceki önizleme aşamasında imagegen becerisi ve yerleşik image_gen aracıyla üretildi; CLI/API kullanılmadı. Bu entegrasyonda yeni çizim üretilmedi veya PNG pikseli değiştirilmedi. Kırpma, kare ayırma ve pivotlar Unity TextureImporter metadata'sında tanımlandı.

Referanslar:
- `/Users/alicanozturk/Projeler/KitchenChaos/Assets/Art/Characters/ChefA/ChefA_Idle_Grumpy_v03.png`
- `/Users/alicanozturk/Projeler/KitchenChaos/Assets/Art/Characters/ChefA/ChefA_Actions_Grumpy_Atlas_v02.png`

## Sprite import

Kaynak1536×1024 RGBA. Altı kare için tek PPU130 kullanıldı; hiçbir poza ayrı ölçek verilmedi. Ayakta ilk pozun yaklaşık520px yüksekliği, mevcut4birim yüksek şefe eşlendi. Yatan pozlar aynı ölçekte daha alçak/geniş olur.

| Kare | Unity rect x,y,w,h | Rect içi pivot x,y |
|---|---|---|
| Recoil |83,479,346,529|169,4|
| Buckle |592,481,316,472|156,4|
| Kneel |1069,486,409,397|125,4|
| Brace |26,59,486,335|151,4|
| Collapsed |523,61,496,225|157,4|
| Settled |1026,67,501,208|164,3|

Koordinatlar Unity alt-sol orijinindedir. Sabit512grid kullanılmadı: üst sıradaki ayaklar bu sınırı geçiyordu. Çizimi değiştirmeden her tam siluete ayrı dikdörtgen verildi.

Bilinear, Clamp, mipmap kapalı, Uncompressed, FullRect, alphaIsTransparency açık, fallbackphysics kapalı, maxTextureSize2048.

## Kod ve sorumluluklar

- ChefLocomotionVisual: isteğe bağlı altı death sprite; ölüm dalı yürüyüş/zıplama/saldırıdan önce gelir. Tek zaman kaynağı PlayerHealth.DeathProgress. İlk0.7 normalize süreye yayılan kare seçimi, ardından son pozu tutma. Altıncı kare yaklaşık0.61s'de başlar.
- PlayerDamageFeedback: yeni isteğe bağlı fade başlangıcı ve tint gücü. Varsayılanlar eski prefabların önceki davranışını korur. Şef prefabında fadeStart0.76, tintStrength0.15.
- PlayerMeleeFeedback: şef prefabında hideWeaponOnDeath açık. Ölümde yalnızca bileşenin kendi spatula parçaları gizlenir; respawn, ölüm iptali veya componentdisable sonrası başlangıç görünürlüğü geri gelir.
- PlayerHealth ve PlayerRespawn kodu değişmedi. Ölüm beklemesi yalnızca Player_Prototype prefabında0.35s'den1.05s'ye çıktı. Bu, gerçek kontrol kilidi/respawn bekleme süresini de artırır. Kaybolma yaklaşık0.798s'de başlayıp1.05s'de tamamlanır.
- Hasar, giriş engelleme, rigidbody kapatma ve checkpoint yetkisi eski sistemde kalır. Mevcut can3, hasar koruması0.75s, respawn koruması0.8s değişmedi.

## Zemin / hava politikası

PlayerRespawn, Died olayından önce PlayerJump zemin bilgisini temizler. Şef görseli sağken son fizik adımındaki zemini saklar ve ölümde mevcut geometriyi doğrular.

Tam çökme/devrilme için:
- Etkin, solid, kenar yuvarlaması olmayan, eksen hizalı BoxCollider2D.
- Destek Rigidbody'si yok veya Static.
- Ayak pivotu ile zemin üstü arasındaki fark en çok0.08birim.
- Aynalanmış tüm ölüm karelerinin yatay alanı zeminin içinde ve kenarlardan0.08birim uzakta.
- Zemin desteği ölüm boyunca etkin/yerinde kalmalı; kaybolursa bu ölümün kalanında recoil'a dönülür, yeniden tam düşüşe geçilmez.

Havada, dar kenarda, hareketli platformda veya destek şekli belirsizken ilk darbe/recoil pozu ve fade kullanılır. Fizik kökü taşınmaz; ceset fiziği veya yeni raycast sistemi eklenmedi. Bu konservatif bloklama, dar kenarda/havada yatık gövde görüntüsünü önler. İleride özel havada ölüm ve farklı zemin türleri ayrı geliştirilebilir.

## Korunanlar ve statik doğrulama

Prefab mutasyonunda diğer118bileşenin serileştirilmiş durumu aynı kaldı. Prefab farkı yalnızca ölüm sprite referansları, deathDelay, deathfade/tint, hideWeaponOnDeath alanlarıdır.

Level01, KitchenIntro_Blockout, PatrolEncounter_Prototype; PlayerHealth, PlayerRespawn, PlayerJump, PlayerAttack, PlayerMovement dosyaları aynı kaldı. Eski duruş/yürüyüş/zıplama/saldırı kareleri, hareket ve nefes değerleri değiştirilmedi.

Edit Mode'da1.ve6.kare görüntüleri alındı, ardından sprite/flip/weaponenabled değerleri ve yalnızca geçici previewoverride'ları geri yüklendi. SceneView önceki konumuna getirildi; sahne temiz bırakıldı.

- `/Users/alicanozturk/Projeler/KitchenChaos/Captures/CharacterReview/chef_death_frame1_static_v01.png`
- `/Users/alicanozturk/Projeler/KitchenChaos/Captures/CharacterReview/chef_death_frame6_static_v01.png`

## Kullanıcı oynanış kontrolü

1. Domatesin yanında geniş düz zeminde canı bitir: sendeleme, diz çökme, devrilme ve son poz okunmalı; yaklaşık1s sonra checkpoint'e dönüş.
2. Sola ve sağa bakarken ayrı kontrol et; pozlar bakılan tarafa devrilmeli ve ayak zemini korumalı.
3. Ölümde yürüme/zıplama/saldırı yapılamamalı; spatula görünmemeli. Respawn'da şef, spatula, can ve kontroller normale dönmeli.
4. Havada/tehlikeli zeminde ölme: tam yatış yerine kısa darbe pozu+kaybolma beklenir; checkpoint döngüsü takılmamalı.
5. Yeniden doğduktan sonra yürüyüş, zıplama, nefes ve spatula saldırısının önceki gibi olduğunu kontrol et.

## Onaylı görselin üretim istemi

```text
Use case: identity-preserve.
Asset type: first approval preview of a six-frame death / defeat sprite atlas for the existing elderly chef in a 2D side-scrolling kitchen action platformer.
Input image 1: PRIMARY, approved chef identity, costume, proportions and painted art style.
Input image 2: supporting reference of the same chef's existing animation sprites, showing rendering style and side-view consistency; do not copy its jumping or punching poses.
Primary request: depict this EXACT elderly pot-bellied chef losing strength, sinking to his knees and toppling forward onto his side. A short readable non-graphic cartoon defeat, not a redesign or a new character.
Layout: one1536x1024 transparent RGBA atlas, exactly3columns by2rows, six equal512x512 cells, read top-left to top-right, then bottom-left to bottom-right. No grid lines, text, numbers or labels. Each cell contains exactly one complete pose of the SAME chef. The chef initially faces RIGHT; later his head falls toward the RIGHT side of the cell, feet toward the LEFT. Same side-on view throughout, no camera turn.
Identity invariants: same elderly age, stocky round belly and short sturdy legs, big rounded nose, thick white moustache, bushy white eyebrows, side tufts of white hair, large soft off-white chef hat, rolled off-white sleeves, dark teal-blue neckerchief, stained ivory apron with the SAME orange food stains and gray-beige wear, charcoal trousers and brown work shoes. Hat stays on his head in all frames. Preserve head size, belly volume and limb lengths across poses; do not shrink a collapsed pose to fill an imagined square. Match the reference's clean dark contours and soft warm painted shading, not a 3D render.
Scale and grounding: standing-equivalent height about450pixels in EVERY cell. All six poses share the same physical scale. Imaginary floor baseline y488 within each cell. Entire body, hat, shoes and hands remain inside the cell with at least16px transparent safety margin. Collapsed characters become lower and wider naturally. Bend the knees slightly in lying poses to fit without shrinking. No contact shadow or drawn ground.
Six distinct sequential keyframes:
1 Impact recoil: still standing, weight over planted shoes, chest and head recoil slightly toward LEFT, knees soften, eyes squeeze shut and brows tense, mouth a small pained frown. Not jumping, not a punch.
2 Knees buckle: hips drop substantially, both knees bend deeply, torso tilts slightly forward, arms drop rather than strike; same face.
3 Kneeling collapse: knees reach the floor, shoes folded behind to LEFT, pot belly and head droop forward toward RIGHT; exhausted, eyes closed.
4 Topple: falls from the knees toward RIGHT, head and torso at a diagonal about45degrees from vertical, one elbow low preparing to touch down. Clear intermediate between kneeling and lying, not a crawling pose.
5 Landing on his side: torso almost horizontal on the floor, head at RIGHT, shoes at LEFT, knees slightly bent, head and hat close to ground. Body fully supported, not propped up on his arms.
6 Settled defeat: lying fully on his side, eyes closed, belly and arms relaxed, one arm loosely in front of apron. Almost same body position as frame5 but fully at rest; unmistakably defeated rather than crouching, praying, sleeping upright or getting back up.
Hands and weapon: draw ONLY the chef body and its normal hands. The spatula is a separate sprite already controlled by the game, so DO NOT paint any spatula, knife, utensil or loose prop into this atlas. No duplicate hand or floating accessory.
Background: genuine transparent RGBA with zero alpha outside clean narrow antialiased silhouettes, including gaps between limbs. No colored haze, gradient, glow, floor, shadow, scene or checkerboard. No blood, gore, injuries, bruises, skulls, X-shaped eyes, stars, birds, ghost, lettering, effects or extra characters. Output ONLY the six-frame sprite atlas.
```

