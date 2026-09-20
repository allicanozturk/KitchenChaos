# Chef A — zıplama ve saldırı pozları (2026-09-18)

## Teslim edilen kapsam

- Zıplama: yükselme, tepe noktası ve düşüş için üç ayrı gövde pozu.
- Saldırı: hazırlık, vuruş ve toparlanma için üç ayrı kol/gövde pozu.
- Spatula her karenin yumruk noktasını izler; silah görsele çizilmedi.
- Eski duruş/koşu görselleri, onaylı hafif huysuz yüz, nefes ayarları korundu.
- Bu sürüm üçer pozlu kare animasyonudur; sürekli kemik animasyonu değildir. Aktif vuruş kısa bir aşağı savurma olarak çizildi.
- Havadaki saldırıda saldırı pozları önceliklidir; ayrı hava-saldırısı bacak katmanı bu kapsamda üretilmedi.

## Dosyalar ve yöntem

Yerleşik image_gen ve imagegen becerisi kullanıldı. CLI/API veya kodla piksel düzenlemesi yapılmadı. PNG'nin üretilen alfa kanalı korundu; Python yalnız ölçüm/doğrulama için kullanıldı.

Final atlas: `Assets/Art/Characters/ChefA/ChefA_Actions_Grumpy_Atlas_v02.png`.
SHA256: `c145f5ed2a3498ed3b6b7e7414b238e1394aa0f0540829251da09bc90791941f`.

Kaynak referanslar:
- `Assets/Art/Characters/ChefA/ChefA_Idle_Grumpy_v03.png`
- `Assets/Art/Characters/ChefA/ChefA_Run_Grumpy_Atlas_v03.png`

Üretim sırası:
1. İlk poz atlası: `/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-9422372f-8d5f-407f-a93f-eb9b72d2fe2b.png`.
2. Vuruş kolu daha kısa menzilli pozla düzeltildi: `/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-a5358b41-ebe8-4dbd-b45a-7d1c9825c715.png`. Bu ara görselde yarı saydam arka plan sızıntısı ölçüldü; oyuna bağlanmadı.
3. Alfa temizliği sonrası seçilen final: `/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-c5ac1b41-0950-40cd-a8a4-7e169d04d94b.png`.

Reddedilen ara PNG proje klasöründen çalışma klasöründeki `work/ChefA_Actions_RejectedAlpha.png` konumuna taşındı; silinmedi. Eski oyun varlıkları değiştirilmedi.

## İçe aktarma ve el bağlantıları

1536×1024 RGBA; 3×2 düzen, altı 512×512 hücre. PPU122, FullRect, bilinear, mipmap yok, sıkıştırma yok, clamp, max2048. Otomatik fizik şekli yok.

Aşağıdaki piksel koordinatları her hücrenin sol üst köşesine göredir. Baseline, sprite pivotunun Y eksenindeki referansıdır; zıplamada ayakları yukarı toplamak gövdeyi büyüterek telafi edilmez.

| Poz | X pivot | Baseline Y | Yumruk merkezi |
| --- | ---: | ---: | --- |
| Rise | 338.5 | 502 | 385,291 |
| Apex | 281.5 | 502 | 320,309 |
| Fall | 253.5 | 502 | 320,326 |
| Windup | 296.5 | 490 | 338,210 |
| Strike | 284.5 | 490 | 326,327 |
| Recovery | 249.5 | 491 | 307,336 |

Sprite pivotu: (pivotX,512-baselineY).
El yereli: ((fistX-pivotX)/122,(baselineY-fistY)/122).
Sağ-sol ayna yalnız mevcut SpriteRenderer flipX/flipY üzerinden bir kez uygulanır.

## Kod ve prefab değişiklikleri

`ChefLocomotionVisual.cs`:
- İsteğe bağlı _jumpFrames/_jumpGripPoints ve _attackFrames/_attackGripPoints dizileri eklendi. Eksik paket çalışan duruş/koşuyu devre dışı bırakmaz.
- Öncelik: ölüm/kontrol engeli/fizik kapalı → nötr; aktif saldırı → saldırı pozu; havada → zıplama pozu; yerde → mevcut koşu/duruş.
- Saldırı görseli doğrudan PlayerAttack.Phase ile seçilir; yeni bir hasar saati veya animasyon olayı yok.
- Apex aralığı ±1.2 birim/s. Kenardan düşüş kabul edilmiş zıplama ile karıştırılmaz.
- Doğuş/yeniden doğuş sonrasında taze zemin örneği beklenir. Zıplama bileşeni kapatılırsa eski zemin bilgisi kullanılmaz.
- Her sprite ve ilgili el bağlantısı birlikte LateUpdate sırası40'da değiştirilir; silah sunumu80'de izler.

`PlayerMeleeFeedback.cs`:
- Önceki sabit hazırlık açısı varsayılan80 derecelik serialize alana çevrildi.
- Yalnız Player_Prototype prefabında135 derece seçildi; yeni yükselmiş yumruk pozunda spatula yüzü kapatmadan yana/yukarı hazırlanıyor.
- Silah uzunluğu ve parçaları, vuruş hasarı/menzili/süreleri değişmedi.

`Player_Prototype.prefab` farkı yalnız yeni altı sprite/el dizilerini ve _raisedAngle135'i içeriyor.
Duruş/koşu referansları, eski el noktaları, dönüşümler ve gameplay bileşenleri değişmedi.

## Doğrulama

- Normal Unity script içe aktarımı sonrası Console: hata/uyarı yok.
- Runtime/Play Mode, build, test runner veya sentetik giriş çalıştırılmadı.
- Edit Mode'da statik Windup, Strike ve sola dönük Apex pozları incelendi; şeffaflık ve el–spatula bağlantısı kontrol edildi.
- Hazırlık açısı135 final önizlemede yüzü kapatmıyor.
- Önizlemelerden sonra sprite/flip/weapon dönüşümleri eski duruşa döndürüldü; geçici görsel override'lar temizlendi.
- Aktif sahne temiz, yeni dizileri prefabdan override olmadan miras alıyor.
- Nefes .008/.016, döngü2.2s, geçiş.05s değişmedi.
- PlayerAttack, PlayerJump, PlayerMovement, Level01 ve KitchenIntro_Blockout dosyalarının önce/sonra SHA256 değerleri aynı.

Statik yakalamalar: `Captures/CharacterReview/ChefA_Action_Windup_Final_EditMode.png`, `ChefA_Action_Strike_EditMode.png`, `ChefA_Action_Apex_Left_EditMode.png`.

## Kullanıcının yapacağı oynanış kontrolü

1. Yerinde ve sağa/sola koşarken zıpla: yükselme → tepe → düşüş, ardından koşu/duruş geçişini kontrol et.
2. Zıplamadan platform kenarından düş: doğrudan düşüş pozu görünmeli.
3. Yerde ve havada vur: kol hazırlanmalı, inmeli ve toparlanmalı; spatula elden kopmamalı.
4. Düşmana vur: önceki hasar, menzil ve bekleme süreleri aynı kalmalı.
5. Öl/checkpointten doğ: duruş/koşu/nefes normale dönmeli, havada veya saldırı pozunda takılmamalı.

## İlk üretim istemi

Use case: identity-preserve.
Asset type: production 2D platform game character action sprite atlas.
Input images: Image 1 is the approved elderly Chef A idle CHARACTER IDENTITY, proportions and facial expression reference. Image 2 is the approved run atlas STYLE, scale and six-cell LAYOUT reference. Make an additional action atlas, do NOT edit or duplicate the running poses.
Output layout: exactly 1536x1024 transparent RGBA canvas, a regular 3-column by 2-row grid of six 512x512 cells, no lines or labels. One full-body chef facing RIGHT in each cell. Use identical physical head, torso and belly size throughout, matching the existing run frames. Fixed body anchor in all cells: pelvis center roughly x256, hat top y16, imaginary standing feet baseline y504. Do not enlarge tucked-leg poses to fill the cell. Keep full hat/hands/shoes inside each cell with safe margins.
Top row is airborne poses, ordered left to right:
1 RISE: chef springing up, both knees noticeably bent and boots tucked slightly back under his belly. Near arm elbow bent, closed gripping fist carried just ahead of belly, not idle standing legs.
2 APEX: chef suspended at jump peak, both knees more tucked, boots gathered upward below body, apron hem floating a little. Keep head and torso the same size and position as rise, no squash/stretch.
3 FALL: chef descending, legs extend downward with bent knees ready to land, feet separated slightly, near closed gripping fist steadies in front of belly. Not a running stride.
Bottom row is a grounded spatula swing WITHOUT drawing the weapon, ordered left to right:
4 WINDUP: feet planted at baseline, near elbow visibly bent, near forearm lifted up to chest height, CLOSED gripping fist near x280 y235, ready to drive a short downward chopping strike. Keep face visible, torso same as idle and full round belly.
5 STRIKE: feet planted in a balanced stance, near shoulder and elbow visibly drive the forearm forward and DOWN; CLOSED gripping fist near x345 y345 at lower-belly height, extends modestly forward, not a long straight punch. Arm visibly different from idle and windup. Keep arm anatomically connected, no extra hands.
6 RECOVERY: same grounded stance, near forearm finishes down and slightly forward, CLOSED gripping fist near x325 y380, elbow relaxed but still a clearly different position from idle, ready to return to idle.
STRICT CHARACTER INVARIANTS: preserve the approved grumpy-yet-lovable expression (lowered white brows, narrowed eye looking right, firm closed mouth), same elderly human identity, round bulbous nose, voluminous white mustache, ear/white hair, tall tilted cream chef hat, pot belly, cream rolled-sleeve shirt, stained cream apron with orange and tan stains, petrol-blue scarf, dark gray pants and brown-gray shoes. Same clean dark linework, softly shaded painted cartoon art and warm colors as references. Only limbs and the small natural clothing folds needed for action change; don't make him thinner, younger, shorter or wider, no camera zoom between frames.
Background: genuine transparent zero-alpha outside silhouettes, no backdrop, gradient, ground, floor shadow, glow, halo, checkerboard or scenery. NO spatula, weapon, tool, prop, motion lines, blur, effects, text, labels, cell borders or extra characters. Exactly six separate clean cutout full-body poses.

## Kısa vuruş düzeltme istemi

Use case: precise-object-edit.
Image 1 is the EDIT TARGET: six 512x512 action frames in a 1536x1024 atlas. Make ONE targeted correction to frame 5 only (BOTTOM MIDDLE cell, x512..1023 y512..1023).
Shorten the forward reach of the near attacking arm by bending its elbow downward, without shortening its anatomy. Move this frame's closed gripping fist from its current approximate global center (909,830) to global center (865,850) (cell-local x353 y338). The forearm should slope forward and down close to the front of the belly, with the fist in front of the apron, near lower-belly height. It is a compact downward chopping strike, not an extended punch. Keep a clear change from the bent-up windup and relaxed recovery poses. Maintain same full head, torso, belly, feet, costume, stern expression and style.
STRICTLY preserve the other FIVE frames unchanged and preserve all body parts of frame 5 except the near upper arm/forearm/closed fist and the apron pixels newly revealed behind the moving arm. No moving, scaling, cropping or reframing any chef. Same 1536x1024 atlas layout and true RGBA transparency/alpha silhouette. No new object, weapon, spatula, effects, background, text or label. Output the entire corrected six-frame atlas.

## Alfa temizleme istemi

Use case: background-extraction.
Image 1 is the EDIT TARGET. Preserve the exact six chef drawings, all poses, closed fists, grumpy expressions, colors, size and pixel placement. This is only an ALPHA/BACKGROUND cleanup, not a redraw.
The existing six-frame atlas has unwanted smoky brown/gray background haze with partial transparency around the sprites. REMOVE ALL THAT HAZE ENTIRELY. Make a clean transparent sprite atlas: alpha exactly ZERO everywhere outside the actual chef silhouettes, and normal fully opaque artwork inside the chefs with only a narrow anti-aliased edge. Remove all floor shadows, soft glows, all gray/brown clouds in the empty gaps between arms/legs and between sprites. No replacement background, checkerboard, white or black fill; actual RGBA transparency.
Keep exact 1536x1024 canvas, same six512x512 cells in 3columns2rows, no reframing, scaling, cropping, shifting or body/arm/head changes. The only opaque subjects should be the six complete chefs themselves. Output just the cleaned atlas with genuine transparent background.
