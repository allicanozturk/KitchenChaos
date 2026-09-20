# Chef A — kol/spatula aşağı vuruş eşlemesi (2026-09-18)

Kullanıcı mevcut hareketi aşağıdan yukarı vuruş gibi algıladığını ve spatulanın bağımsız savrulduğunu bildirdi.

## Neden

ChefLocomotionVisual her saldırı fazı başında gövde/el pozunu anında seçiyordu. PlayerMeleeFeedback ise aynı faz boyunca ayrıca -40→135,135→-55,-55→-40 açı geçişleri yapıyordu. Dolayısıyla el yukarı hazırlanmış veya aşağı inmişken spatula o sabit yumrukta ikinci bir dönüş yapıyordu. Poz sırası zaten yüksek hazırlık→alçak vuruş→toparlanmaydı; yanlış algıyı artıran bu bağımsız dönüş kaldırıldı.

## Değişiklik

Yalnız iki görsel bileşen değiştirildi:
- ChefLocomotionVisual, gösterilen saldırı sprite/el noktasıyla birlikte sağa dönük silah açısını yayınlıyor: hazırlık135°, vuruş-35°, toparlanma-40°.
- PlayerMeleeFeedback bu eşlenmiş açıyı LateUpdate'te kullanıyor. Şefin tam saldırı paketi bulunmayan eski prefablar eski görsel akışını koruyor.
- Koşu, duruş, zıplama, nötr, ölüm/yeniden doğuş ve devre dışı kalmada önbellek geçersizleştiriliyor. Duraklatılmış karede görsel poz ve açı birlikte korunuyor.
- Açı yalnız bir kez mevcut sağ-sol hesabıyla aynalanıyor.
- ClearEffects/SetWeaponPose içine override eklenmedi; temizleme durumları silahı hâlâ güvenle nötr konuma döndürüyor.

Kolun aşağıdan yukarı ilk hareketi kısa hazırlıktır; bu vuruşun asıl darbesi yukarıdan aşağıdır. Üç pozlu mevcut animasyon kullanılıyor; yeni görsel veya ara kare üretilmedi, kesintisiz kemik animasyonuna geçilmedi.

## Kontroller

Play Mode kullanıcı testinden açık kaldığı için düzenleme öncesi durduruldu; tekrar başlatılmadı. Oyun testi, test runner, sentetik giriş veya build yapılmadı.

Normal script içe aktarımı sonrası Console hata/uyarı girdisi:0. Bağımsız kaynak incelemesi sıfırlama, duraklama, sağ-sol ve eski prefab davranışlarında engelleyici sorun bulmadı.

Prefab verilerinden hesaplanan yerel yükseklikler:
- Hazırlık: yumruk2.295, spatula başı merkezi2.776.
- Vuruş: yumruk1.336, spatula başı merkezi0.946.
- Toparlanma: yumruk1.270, spatula başı merkezi0.833.

Yükseklikler sırayla azalıyor. Bunlar yalnız statik veri/geometri kontrolüdür, oynanış testi değildir.

PlayerAttack, PlayerJump, PlayerMovement, Level01, KitchenIntro_Blockout, Player_Prototype prefabı ve action atlası değiştirilmedi. Hasar, menzil, saldırı süreleri, silah boyu, zıplama, koşu ve nefes ayarları aynı.

## Kullanıcı kontrolü

Sağa/sola yerde ve havada saldır. Kısa hazırlık sonrası kol ve spatula birlikte aşağı vurmalı; el sabitken ayrı yukarı savurma olmamalı. Ölüm/yeniden doğuş sonrası silah eski duruşa dönmeli.
