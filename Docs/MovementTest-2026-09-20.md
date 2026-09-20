# İlk hareket ve şef darbe paketi — 20 Eylül 2026

## Son animasyon revizyonu — önce bunu test et

- Çömelerek yürüyüş: dört eski kare yerine sekiz yeni ara poz; tam çömelme hızında yaklaşık 14,5 kare/sn. Döngü 1,35 birim ilerlemeye bağlı; daha az analog itişte adımlar da yavaşlar. Karakter ve collider boyutu değiştirilmedi. Sağ/sol yürüme, durma, yeniden başlama, alçak tünel ve spatula tutuşunu kontrol et.
- Darbe: ayakta/havada kullanılan üç poz ve çömelmiş üç ayrı poz; acı ifadesi → geri çekilme → toparlanma. Sunum 0,42 sn; görselde sürekli, az miktarda geri çekilme var, fizik köküne/çarpışmaya geri itme uygulanmıyor. Saldırı kesintisi eski 0,24 sn; can, ses, hasar koruması süresi ve hareket kontrolü korunuyor. Darbe pozları okunabilsin diye toparlanma sırasında alfa sabit; ardından kalan korunma için yanıp sönme sürüyor. Ölüm her zaman öncelikli.
- Ayakta, havada ve çömelirken domates/bezelyeden hasar al. Hareket kilitlenmesi olmamalı; ölüm/respawn sonrasında görsel konum normale dönmeli. Spatula her darbe pozunda eli takip etmeli.
- Kod değişirken Play sırasında InputReader.OnEnable/OnDisable ve MeleeFeedback.ClearEffects NullReference kayıtları görüldü; yeniden yüklemeyle ilişkili olduğu düşünülüyor, kesin kök neden/play doğrulaması yapılmadı. Kullanıcı Play'i durdurduktan sonra yeni bağlantılar kaydedildi. Testi yeni Play oturumunda yap; aynı hatalar tekrarlanırsa bildir.
- V2 varlıkları: `Assets/Art/Characters/ChefA/Movement/ChefA_CrouchWalk_01_v02.png`–`08_v02.png` ve `ChefA_HurtSequence_01_v02.png`–`06_v02.png`. Yerleşik görsel üretim aracı; mevcut crouch/idle referansları kullanıldı. Önceki dosyalar silinmedi.
- Prompt seti: `/Users/alicanozturk/Documents/Codex/2026-09-16/https-www-youtube-com-watch-v/work/chef-animation-v2-prompts.txt`. Ayırma/ankraj: `work/prepare-chef-animation-v2.cjs`, `outputs/chef-animation-v2/import.json`; Unity uygulama: `work/apply-chef-animation-v2.cs`. Aşağıdaki ilk revizyon notları tarihsel ayrıntıdır; darbe erteleme ve dört kare bilgisi bu bölümle geçersizdir.

## Önceki hareket revizyonu

Durum: ilk kullanıcı testi alındı; aşağıdaki hareket revizyonu uygulandı ve yeniden kullanıcı testi bekliyor. Aktif sahne `Assets/Scenes/KitchenMovement_Test.unity`. Asistan Play, build veya otomatik oynanış testi başlatmadı. İlk pakette çoğu mekanik çalıştı; kullanıcı koşuyu belirsiz, ikinci sıçramayı zayıf, eğilerek yürüme ve darbe animasyonunu eksik buldu. Hava dash'i yeni zorunlu özellik. Darbe sunumu bu revizyonda ertelendi; mevcut poz onaylı animasyon sayılmaz.

Eski `KitchenIntro_Blockout` ve `Level01` yerleşimleri korundu. Yeni sahne eski bölümün kopyasından oluşturuldu; eski ortam/oynanış/yönlendirme kökleri yalnız kopyada kapatıldı. Yeni PlayerMobility yalnız test sahnesi oyuncusunda; temel prefabda yok. Paylaşılan scriptlerde entegrasyon değişiklikleri vardır.

## Kontroller

| İşlem | Xbox | Klavye |
| --- | --- | --- |
| Hareket | Sol analog; az itiş yürüme, tam itiş koşma | A/D veya sağ/sol |
| Zıplama | A; kısa bas/basılı tut | Space; kısa bas/basılı tut |
| İkinci zıplama | Havada yeniden A | Havada yeniden Space |
| Yer/hava dash'i | RB | Sol Shift |
| Eğilme | Sol analog veya yön pedi aşağı | S, aşağı ok veya C |
| Spatula | X | Önceden kullanılan saldırı tuşu |

## Soldan sağa test

1. Başlangıçta analog hız farkını ve tam itişte koşu ritmini kontrol et. İlk zıplamada kısa/uzun basışı karşılaştır. İkinci basış kısa olsa da tam güçlü bir ek sıçrama olmalı; üçüncü olmamalı. Yere inince yenilenmeli.
2. Yüksek platforma çift zıplamayla çık. Kenarlara yandan temas edince havada takılma olmamalı.
3. Dash duvarına doğru RB/Shift kullan: duvarın içinden geçmemeli. İlk veya ikinci zıplamadan sonra havada dash olmalı; aynı havalanışta ikinci dash olmamalı. Yere inince yenilenmeli. Dash sırasında saldırı başlamamalı. Tuşu tutmak kendiliğinden tekrar dash üretmemeli. Havada duvara dash sonrası yerçekimi normale dönmeli; düşüş kilitlenmemeli.
4. Alçak tünelde eğilerek ilerle: ayaklar adım atmalı, durunca sabit eğilmiş poza dönmeli; sağ/sol tutuşu kontrol et. Tavan altındayken eğilme tuşunu bırakınca veya zıplamaya basınca tavanın içine büyümemeli; açık alana çıkınca doğrulabilmeli.
5. Yükseltilmiş bezelyenin atışını ayakta ve eğilerek karşılaştır. Yüksek atış eğilmiş şefin üstünden geçmeli. Dash'in yalnız başlangıcında düşman hasarından kaçınma mümkün; bütün dash dokunulmaz değil.
6. Hareketli platforma bin, üstünde yürü ve çift zıpla. Taşınma/iniş davranışında bozulma olmamalı.
7. Domatesten ayakta, havada ve eğilmişken hasar al. Can/ses/darbe pozu birlikte tepki vermeli. Geçici yanıp sönme bitmeli, karakter kilitlenmemeli. Spatula elden kopuk görünmemeli.
8. Kırmızı tehlikeden dash ile geçmeyi dene: dash koruması tehlikeyi etkisizleştirmemeli. Önceki darbeden kalan normal hasar koruması ayrı işlemeye devam eder.
9. Çukura düş, sonra düşman hasarıyla da öl. Ölüm sunumu öncelikli olmalı; checkpoint dönüşünde boy, collider ve kontroller normal hâle gelmeli.

## İlk ayarlar ve sınırlar

- Koşma hızı test sahnesinde 7 birim/sn (önceki 5). Mevcut altı koşu karesinin ritmi hıza bağlı; tam hızda yaklaşık 14 kare/sn. Analog küçük itiş yürüme; ayrı sprint tuşu yok.
- İkinci sıçrama hızı ilk sıçramanın %100'ü (11 birim/sn). Tuş bırakma yalnız ilk sıçramayı kısaltır; ikinci sıçrama kısa basışta da tam yay verir.
- Dash: hız 13 birim/sn, süre 0,18 sn, başlangıçlar arası en az 0,65 sn; ilk 0,11 sn düşman hasarı koruması. Havalanış başına bir hava dash'i; gerçek zemin temasıyla yenilenir. Yerde başlayan dash kenardan çıkınca devam eder ve hava hakkını tüketir. Dikey hız/yerçekimi dash boyunca sıfır; bitince yerçekimi geri gelir, önceki düşüş devam eder veya yükselme yerine düşüş başlar. Diken/lav/çukur koruması vermez.
- Eğilmiş collider yüksekliği ayaktakinin %68'i: bu sahnede 4 → 2,72 birim. Ayak tabanı korunur. Eğilerek ilerleme normal hızın %35'i.
- Darbe pozu 0,24 sn; saldırıyı keser, hareket/zıplamayı bütünüyle kilitlemez. Ölüm animasyonu önceliklidir.
- İlk pakette eğilerek spatula saldırısı kapalı. Eğilerek uzak atış sonraki çatal paketinin parçası.
- Kamera orthographic size 8. Bu alan mekanik denemesi; nihai bölüm, hikâye, boss veya 10 dakikalık içerik değil.
- Çatal ve özel dönüş henüz uygulanmadı. Bezelye/domatesin onaylı savaş istatistikleri değiştirilmedi.

## Dosyalar ve görsel üretim kaydı

- Ana yeni bileşen: `Assets/Scripts/Player/PlayerMobility.cs`.
- Entegrasyon: PlayerInputReader, PlayerMovement, PlayerJump, PlayerAttack, PlayerHealth, PlayerRespawn, ChefLocomotionVisual, HazardDamage.
- Dört yeni poz: `Assets/Art/Characters/ChefA/Movement/` altında Hurt, Crouch, Dash, Crouch_Hurt PNG'leri. Referans: `ChefA_Idle_Grumpy_v03.png`. Yerleşik görsel üretim aracıyla üretildi; kareler mekanik olarak ayrılıp aynı piksel/birim ölçeği ve ayak tabanı esas alınarak içe aktarıldı. Tutuş noktaları kullanıcı görsel testi bekliyor.
- Üretim promptu: `/Users/alicanozturk/Documents/Codex/2026-09-16/https-www-youtube-com-watch-v/work/chef_movement_prompt.txt`.
- Ham sayfa: `/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-bc728388-6cf0-4bc4-82f5-261de0e0024b.png`.
- Kare ayırma/ankraj kaydı: çalışma alanında `work/prepare-chef-movement.cjs` ve `outputs/chef-movement/sprite-import.json`.
- Revizyon eğilerek yürüme: aynı proje klasöründe `ChefA_CrouchWalk_01_v01.png`–`ChefA_CrouchWalk_04_v01.png`. Mevcut eğilme pozu referans alınarak yerleşik görsel üretim aracıyla üretildi; alfa korundu, dört ayrı kare olarak içe aktarıldı. Prompt: `/Users/alicanozturk/Documents/Codex/2026-09-16/https-www-youtube-com-watch-v/work/chef-crouch-walk-prompt.txt`. Mekanik ayırma/ankraj: `work/prepare-crouch-walk.cjs`, `outputs/chef-crouch-walk/sprite-import.json`. Ham sayfa: `/Users/alicanozturk/.codex/generated_images/01a0a98c-bd1c-7ee1-90c7-de98c262a4b9/exec-44ae80ff-664d-45fc-92f2-2e30c62e8522.png`.

Sonraki işlem: dört hareket düzeltmesini kullanıcıya test ettir, sonra eksik darbe animasyonunu ele al. Hareket/darbe paketi onaylandıktan sonra çatal saldırısına geç. Kullanıcı test yapmadan çalıştığı onaylandı diye kaydetme.
