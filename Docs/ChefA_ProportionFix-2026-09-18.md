# Chef A — duruş/koşu oranı ve nefes düzeltmesi

Kullanıcı geri bildirimi: koşarken daha kısa/geniş, durunca daha ince/uzun görünme; nefesin fark edilmemesi. Düzeltmeler bağlandı; tekrar oynanış kontrolü kullanıcıda.

Son ince ayar: kullanıcı yeni duruş/koşu görünümünde başka sorun olmadığını bildirdi; nefesin fazla büyütüp küçülttüğünü belirtti. İsteğiyle yalnız nefes genliği üçte bir azaltıldı: genişlik %1.2 → %0.8, yükseklik %2.4 → %1.6 (ilk yüksekliğin iki katı). 2.2 saniyelik süre ve .05 saniyelik ölçek yumuşatma korundu. Görseller, tutuş, kamera ve fizik değişmedi. Açık Play Mode düzenleme için durduruldu; asistan oynanış testi başlatmadı. Kullanıcının yalnız 4–5 saniye bekleyip nefes şiddetini değerlendirmesi yeterli.

## Yapılanlar

- Sorun kamera değişimi değil, ilk idle ve run çizimlerinin baş/gövde oranlarının farklılığıydı. Run atlasının üst-orta karesi referans alınarak eşleşen tek duruş çizimi yerleşik `image_gen` ile üretildi. CLI/API anahtarı kullanılmadı.
- Yeni varlık: `Assets/Art/Characters/ChefA/ChefA_Idle_RunMatched_v02.png`. Orijinal idle ve koşu atlaslarının piksel içerikleri korundu; yeni PNG sonradan düzenlenmedi.
- Kaynak: `/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-92da9d12-1677-45ae-8d24-996f07b45705.png`.
- Boyut 1254×1254 RGBA; alpha 0–255. SHA256 `26c099ed96c9dfca1c95ebc8f720c117d97be5dd1c02271ad32978cd105caba8`.
- Unity Single / Full Rect / PPU298.5, pivot (648.125,35) piksel. Görünür şapka–taban yüksekliği yaklaşık 4 dünya birimi; burun ucu dünya X≈0.75, Y≈2.972. Run02 karşılığı X0.75, Y≈2.984: iki pozun yüz yüksekliği farkı yaklaşık .012 birim. Kafa/gövde silueti ayrıca statik görüntüde karşılaştırıldı.
- Yeni idle avuç noktası ((732.39574−648.125)/298.5, (1219−869.5684)/298.5). Hem bileşene hem prefabın spatula varsayılan tutuşuna bağlandı; devre dışı bırakma sonrası eski tutuşa kaymaz.
- Nefes 2.4s → 2.2s; genişlik %0.35 → %1.2; yükseklik %0.8 → %2.4. Kamera ortho8 ve1080px yükseklikte eski hareket başta≈2.16px, yeni hedef≈6.48px. Gerçek ekrandaki piksel karşılığı Game View yüksekliğine bağlıdır.
- Normal idle/run ölçek hedeflerine zaman bağımsız üstel yaklaşma eklendi (zaman sabiti .05s); nefes esnemesi koşuya başlarken aniden sıfırlanmaz. Ölüm/respawn/devre dışı bırakma temizliği anlık kalır. Tam gövde crossfade/çift siluet kullanılmadı.
- Fizik kökü, kapsül, hareket, zıplama, hasar menzili/zamanları ve kamera değişmedi. Prefab kaydından önce ilgili serialized veriler karşılaştırıldı.
- Açık kullanıcı Play Mode'u düzenleme için durduruldu; tekrar başlatılmadı. Build, oynanış otomasyonu ve test koşucusu yok.
- Yalnız Edit Mode'da idle, nefes tepe ölçeği ve Run02 statik görüntüleri karşılaştırıldı. Bu hareket ritminin oynanış testi değildir.
- Statik incelemelerden kalan yalnız görsele ait `m_Sprite`, `m_FlipX`, spatula local position/rotation ve görsel local scale override'ları prefab değerlerine döndürüldü. Böylece sahne yeni idle'ı miras alır. Daha önce temiz olan `KitchenIntro_Blockout` sahnesi kaydedildi; diğer override'lar korunur. Scene View rotation'a dokunulmadı.
- Son Console kontrolünde hata girdisi yok; sahne temiz ve Play Mode kapalı. `Level01.unity` ile orijinal idle SHA256 değerleri değişmedi.

## Kullanıcı kontrolü

1. Sağa/sola koşup birkaç kez dur: şefin baş/gövde oranı veya kamera uzaklığı değişiyormuş hissi azaldı mı?
2. 4–5 saniye bekle: nefes artık okunuyor mu; hareket doğal mı yoksa fazla esneme hissi mi veriyor?
3. Nefesin ortasında koşmaya başla; spatula avuçta kalmalı. Bir saldırı/ölüm/respawn sonrası da boyut/tutuş takılmamalı.

## Üretim istemi (aynen)

Use case: identity-preserve.
Asset: one production IDLE STANDING sprite for the EXACT same elderly chef in the input RUN ATLAS.
Input image is the edit/identity target, especially the MIDDLE CHARACTER IN THE TOP ROW. Isolate and reuse that character; create only ONE complete standing character, not another atlas.
CRITICAL: The current game's old idle was too narrow and long compared to this run atlas. The new idle MUST retain THIS ATLAS'S exact broad pot-bellied torso, large round head, cheek and nose, head-to-body ratio, short sturdy legs, apron width, hat silhouette, upper-body angles, round friendly face, neckerchief knot, stains, crisp ink line weight and soft shading. Do NOT reinterpret him into a taller, thinner man. Do NOT shrink the head. Do NOT lengthen the neck or legs. The head, hat, torso, near upper arm and EMPTY low gripping fist should look copied from the top-middle atlas frame.
Make only the leg/stance change necessary to stand comfortably: both dark shoes flat on the SAME horizontal baseline, side-on pointing RIGHT, feet staggered by a small amount so both shoes are readable, knees naturally relaxed rather than running/crouching. No exaggerated upright stretch. Keep the forearm gently bent forward with EMPTY closed fist low in front of apron, same arm proportions and approximate hand height as top-middle atlas frame. Far arm mostly hidden behind torso. Do not add a weapon.
Single character strict right-facing profile matching atlas camera; full hat and both shoes visible. Square 1024x1024 canvas, character about 950px tall with even small transparent vertical margins and generous side margins. Same white folded hat, fluffy white mustache/side hair/eyebrows, cream short rolled sleeves, petrol-blue scarf, orange/beige stained cream apron, charcoal trousers, dark broad kitchen shoes.
Genuine transparent RGBA: zero alpha outside silhouette and between legs. No backdrop, halo, gradient, drop shadow, floor, text, grid, label, frame number, props, weapon, second character. Preserve character identity and atlas proportions above all.
