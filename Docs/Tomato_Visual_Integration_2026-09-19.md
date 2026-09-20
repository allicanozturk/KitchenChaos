# Domates düşmanı: duruş ve yürüyüş entegrasyonu — 2026-09-19

## Tamamlanan kapsam

Kullanıcı “tasarım güzel” diyerek duruş tasarımını onayladı. Onaylı duruş görseli ve yeni altı karelik yürüyüş atlası, mevcut PatrolEncounter_Prototype prefabındaki düşmana bağlandı. Kaynak PNG'ler değiştirilmedi; Unity import ayarlarıyla kareler ayrıldı, boyut ve hizalar ayarlandı.

Yeni bileşen: `Assets/Scripts/Enemy/TomatoLocomotionVisual.cs`.
Sadece SpriteRenderer.sprite ve flipX yönetir. Rigidbody2D'nin tamamlanmış fizik adımları arasındaki yer değiştirmeyi okuyarak yürüyüş/yön seçer; fizik adımları arasındaki render karelerinde ölçümü korur. Stun, kapalı devriye veya kapalı simülasyonda duruşa geçer; durduğunda son yönünü korur. Oyun duraklatılınca mevcut kare donar. Başlangıç yönü sola, nominal hız 1.2 birim/s, yürüyüş 8 kare/s. Geçersiz görsel bağlantıları etkinleşirken doğrulanır.

EnemyHitFeedback, görsel transform/tint/shake/squash üzerindeki sahipliğini korur. Eski Visual Animator yalnızca bu prefabda devre dışı bırakıldı; controller ve eski sprite/animasyon dosyaları silinmedi.

## Dosyalar ve üretim yöntemi

Yerleşik image_gen aracı ve imagegen becerisi kullanıldı; CLI veya ayrı API kullanılmadı. Görseller üzerinde Python ile değişiklik yapılmadı; salt okunur alfa/boyut/hiza ölçümleri yapıldı.

Tarz referansı:
`/Users/alicanozturk/Projeler/KitchenChaos/Assets/Art/Characters/ChefA/ChefA_Idle_Grumpy_v03.png`

Onaylı duruş kaynağı:
`/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-b498c549-9f34-4d9f-88b7-a85b07f65e9b.png`

Duruşun projede kullanılan kopyası:
`/Users/alicanozturk/Projeler/KitchenChaos/Assets/Art/Characters/Tomato/Tomato_Idle_v01.png`

Yürüyüşün girdi referansı: onaylı duruş kaynağı.
Yürüyüş çıktısı:
`/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-30ab8aa7-bf33-45a4-8365-154ed7b64c7e.png`

Yürüyüşün projede kullanılan kopyası:
`/Users/alicanozturk/Projeler/KitchenChaos/Assets/Art/Characters/Tomato/Tomato_Walk_Atlas_v01.png`

## Ölçek ve import

- Duruş: 1254×1254 RGBA, PPU 423.333333, piksel pivotu (640,147), görünür yükseklik yaklaşık 2.4 birim.
- Yürüyüş: 1536×1024 RGBA, 3 sütun×2 satır; 512×512 hücreler, PPU 200. Kare sırası üst sıra soldan sağa, ardından alt sıra soldan sağa.
- Karelerin hücre içi alt-sol orijinli pivotları: (288,22), (264,17), (248,23), (288,35), (264,30), (247,39).
- Yürüyüş PPU'su gövde/yüz ölçülerini duruşa eşler; kareye göre değişen bacak pozları zorla esnetilmedi. Bir yürüyüş döngüsünde yaklaşık 0.07 birim doğal baş salınımı kalır.
- Bilinear, mipmap kapalı, Clamp, Uncompressed, alphaIsTransparency açık, FullRect, otomatik fizik şekli kapalı.
- Düşman kökü aynı: scale (1.6,2.5,1); collider dünya ölçüsü 1.6×2.5. Görsel çocuk scale (0.625,0.4,1) ile dünya ölçeği (1,1,1) oldu. Görselin localPosition değeri (0,-0.5,0); ayak pivotu dünya Y=-2.5 olan zeminde.
- Sprite sorting order 2 ve mevcut Sprite-Lit materyali korundu. Can barının kardeş hiyerarşisi korunarak görsel yön değişiminden etkilenmesi engellendi.

## Korunanlar / henüz yapılmayanlar

Düşman canı 3, devriye hızı 1.2, temas hasarı 1, temas aralığı 1 saniye, stun 0.45 saniye değişmedi. Prefab mutasyonunda 19 mevcut bileşenin serileştirilmiş hali karşılaştırıldı ve korundu. Yalnızca görsel child transform, renderer sprite/flip, Animator.enabled değişti; yeni sunum bileşeni eklendi.

Şef prefabı ve onaylı şef animasyonlarına dokunulmadı. Level01 ve KitchenIntro_Blockout sahne dosyaları değiştirilmedi; blockout yeni görünümü prefabdan devralır.

Yeni domates darbe/ölüm sprite animasyonları bu adımın kapsamında değil. Mevcut hit tint/shake/squash/can barı ve hemen kaybolma şeklindeki ölüm davranışı korunuyor. Yeni düşman saldırısı eklenmedi.

## Doğrulama sınırı ve kullanıcı testi

Normal Unity script import/derlemesi tamamlandı, konsolda hata/uyarı yoktu. Referanslar ve prefab farkı statik olarak incelendi. Play Mode, otomatik oynanış, test runner veya build çalıştırılmadı.

Edit Mode statik görüntüleri:
- `/Users/alicanozturk/Projeler/KitchenChaos/Captures/CharacterReview/tomato_idle_integration_v01.png`
- `/Users/alicanozturk/Projeler/KitchenChaos/Captures/CharacterReview/tomato_walk_frame02_static_v01.png`

İkinci görüntü için geçici sprite/flip önizlemesi yapıldı; sonra yalnızca bu iki özellik geri yüklendi, geçici override bırakılmadı. Scene View eski konumuna döndürüldü ve sahne temiz kaldı.

Kullanıcının kontrol etmesi gerekenler:
1. Devriye sırasında altı karelik adımlar ve uçlarda doğru yöne dönüş.
2. Ayakların zeminde kalması; duruş/yürüyüş geçişinde belirgin büyüme, esneme veya yatay zıplama olmaması.
3. Spatula isabetinde yürüyüşün durması, mevcut titreme/can azalmasının görünmesi ve stun sırasında temas hasarı vermemesi.
4. Stun sonrasında yürümeye devam etmesi, üçüncü isabette önceki gibi ölmesi.

## Duruş üretim istemi

```text
Use case: stylized-concept.
Asset type: first idle character-design preview for the basic walking tomato enemy in an existing kitchen-themed 2D side-scrolling action platformer.
Input image 1: ART-STYLE REFERENCE ONLY, the game's approved elderly chef. Do NOT draw the chef or make a tomato version of his clothes. Borrow the clean dark contour lines, warm hand-painted cartoon shading, restrained highlights, readable simple forms and finished 2D sprite look.
Primary request: ONE original anthropomorphic ripe red tomato enemy, complete full body, standing in an alert idle pose, facing RIGHT in a near-side / slight three-quarter view appropriate for side-scrolling gameplay. Both feet planted; one clear forward-facing eye and the far eye only partly visible if appropriate. Its gaze and body orientation must clearly show rightward travel when later animated.
Design: a compact rounded tomato forms both head and torso, a little taller than wide rather than flattened. Rich warm tomato red skin with very subtle natural lobes, a small green five-point calyx and short stem on top. Determined angry eyebrows, clear ivory eye whites with dark pupils, small firm frowning mouth; cheeky grumpy vegetable adversary, not horror and not a friendly collectible. Two short sturdy red arms with small rounded clenched hands tucked near the body; two short dark leafy-green stalk legs and simple broad leaf-like feet. Keep appendages compact, with no long arms, oversized fists or wide battle stance. Total silhouette is upright and compact, approximately 0.7-0.8 times as wide as tall including arms; tomato body stays dominant. No human nose, human skin, hair, beard, mustache, hat, apron, clothing, accessories, boxing gloves or weapon. Not a chef, not a boss.
Composition: one character only, centered on a square canvas, full stem and both feet visible, comfortable transparent margins on every side. Consistent orthographic-like side-game perspective, not a top-down view or dramatic camera. Readable expression and leaf silhouette at small game size. Clean forms suitable for later walk/hit/death sprite frames.
Background: genuine transparent RGBA with zero alpha outside the character, narrow clean antialiased edges. NO scenery, kitchen environment, floor, cast shadow, glow, haze, gradients, backdrop, drawn checkerboard, labels, text, watermark, logo, comparison sheet, second pose or extra character.
Style constraints: match the reference's polished softly shaded cartoon illustration, not photorealism, pixel art, 3D plastic render, flat vector sticker or sketch. Output just the single tomato idle design.
```

## Yürüyüş üretim istemi

```text
Use case: identity-preserve.
Asset type: 6-frame walking sprite atlas for the approved tomato enemy in a 2D side-scrolling platformer.
Input image 1 is the APPROVED CHARACTER IDENTITY AND STYLE reference. Create a walk-cycle atlas of this EXACT tomato, not a redesign.
Output layout: exactly1536x1024 pixels, 3 columns and2 rows of equal512x512 cells, six full-body sprites facing RIGHT. Frame order top-left,top-middle,top-right,bottom-left,bottom-middle,bottom-right. No labels or drawn grid.
Physical invariants: keep the exact round ripe-red tomato torso shape/width/height, dark outlines, lobes/highlights, small red arms and rounded fists, leafy dark-green eyebrows, large ivory eyes looking RIGHT, small frown, leaf crown and bent stem shape, green stalk legs and leaf feet. Same polished warm hand-painted cartoon shading. The body and face should look like the SAME drawing through all six frames with only subtle 2-4pixel vertical walk bob, no body squash, stretch, scale drift, facial redesign or changing gaze. Keep both fists near the sides with very restrained opposing arm swing; no raised attack arms.
Geometry: in EVERY512square cell, center the tomato body at x256, top of stem abouty28, standing foot baseliney488. Overall standing height about460pixels, compact width about340pixels. Preserve the source's body proportions; do not widen or flatten the tomato. Keep at least16pixels of transparent safety margin. Never crop a stem, foot or hand.
Walk cycle: a slow grumpy compact patrol walk, six genuinely different alternating foot poses.
1 Near foot forward planted, far foot back touching ground.
2 Weight over near foot; far foot lifts and passes under body.
3 Far foot swings forward while near heel starts rising.
4 Far foot forward planted, near foot back touching ground, opposite contact toframe1.
5 Weight over far foot; near foot lifts and passes under body.
6 Near foot swings forward toward frame1 while far heel starts rising.
Small readable strides and low foot lifts, NO running, jumping, crossed twisted legs or extra limbs. A pair of short leaf-like feet should remain legible in each cell. Feet stay near the same ground baseline; no floating character.
Background: genuine transparent RGBA, zero alpha outside actual silhouettes, clean narrow antialiased edges; no painted checkerboard, gray/brown haze, backdrop, floor shadows or glow. No weapon, clothing, hat, text, labels, logos, extra props, second character design or extra frames. Output ONLY the six-frame walking atlas.
```

