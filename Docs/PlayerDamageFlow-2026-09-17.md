# Oyuncu hasar ve ölüm akışı — 17 Eylül 2026

Durum: kullanıcı bu hasar/ölüm akışını oynayarak denedi ve “hepsi çalıştı” diyerek onayladı. Bu onay daha sonra eklenen spatula/yakın dövüş paketini kapsamaz. Asistan Play Mode veya test koşucusu başlatmadı. Düzenleme öncesinde kullanıcının açık bıraktığı Play Mode kapatıldı. Unity'nin normal script içe aktarımı ve Console kontrolü yapıldı; bağımsız build alınmadı.

## Bu adım

- Kabul edilen her normal vuruştan sonra oyuncu 0.75 saniye yeni normal hasar almaz. Başka bir düşman/hazard da bu korumaya uyar; ölümcül çukurlar uymaz.
- Prototip oyuncusunda 0.12 saniyelik kırmızı hasar tepkisi ve koruma sırasında yarı saydamlık dalgalanması bulunur. Kök fizik nesnesi sallanmaz veya ölçeklenmez.
- Can sıfır olunca hareket, zıplama, saldırı, platform taşıması ve fizik simülasyonu geçici olarak durur. Karakter 0.35 saniyede kaybolur; checkpoint'e tam canla döner ve 0.8 saniye korunur. Bu, nihai çizilmiş ölüm animasyonu değildir.
- Yeniden doğarken eski hız, zemin/platform referansı, zıplama tamponu, saldırı isteği ve animasyon durumu temizlenir. Ölürken basılı kalan zıplama/saldırı tuşu bırakılmadan yeni eylem başlatmaz; hareket tuşu basılıysa dönüş sonrası yürüyüş devam edebilir.
- Ölüm ve teleport karesindeki eski temaslar yeni canı azaltmaz, coin toplamaz veya checkpoint değiştirmez. Spawn üzerinde coin varsa sonraki fizik adımında tekrar denenir.
- Normal hasarda kontrol kilitlenmez. Mevcut sesler korunur; hasar sesi yalnız kabul edilen vuruşta çalar. Yeni bir ses dosyası eklenmedi.
- Skor, toplanmış coin'ler ve yok edilmiş düşmanlar mevcut davranışlarıyla korunur. Karşılaşmayı yeniden kurma bu adımın kapsamında değildir.

## Kullanıcının kısa kontrol listesi

`KitchenIntro_Blockout` sahnesinde:

1. Checkpoint'ten geç, düşmana saldırmadan temas et. Bir hasar anında bir can gitmeli, karakter kızarıp ardından kısa süre yarı saydamlaşmalı. Bu sırada hareket/zıplama çalışmalı.
2. Temasta kal. Koruma süresi içinde tekrar can gitmemeli; süre bittikten sonra düşmanın temas aralığına bağlı olarak yeniden hasar alabilmelisin.
3. Son canı da kaybet. Kısa kaybolma geçişinden sonra son checkpoint'te 3 canla görünmelisin. Karakter görünmez, hareketsiz veya saldırı animasyonunda takılı kalmamalı.
4. Ölüm sırasında zıplama ya da saldırıyı basılı tut. Dönüşte eski komut kendiliğinden tekrarlanmamalı; tuşu bırakıp yeniden basınca çalışmalı. Bunu iki ölüm/dönüş boyunca dene.
5. Coin topladıktan sonra öl. Skor korunmalı ve aynı coin tekrar oluşup ikinci kez puan vermemeli. Kamera, basamaklara gidiş/dönüş ve düşman sersemlemesi önceki onayladığın gibi kalmalı.

Ek kontrol: Bu prototipte ölümcül çukur yok. `Level01` sahnesindeki ölümcül hazard'ı koruma sürerken deneyebilirsin; dokunulmazlık ölümcül zeminde yürümeyi sağlamamalı. Alternatif olarak yalnız Play Mode'da PlayerHealth bileşen menüsündeki `Force Death (bypass protection)` aynı hasar-korumasını aşan ölüm yolunu çağırır; bu menü hazard çarpışmasını doğrulamaz.

## Kapsam

Ortak oyuncu, coin, checkpoint ve hazard scriptleri güncellendi; dolayısıyla eski sahne de yeni hasar/respawn kurallarını kullanır. Görsel `PlayerDamageFeedback` yalnız `Player_Prototype` prefabına bağlandı. `Level01.unity` sahne dosyası ve kabul edilen kamera/zıplama/düşman boyutları değiştirilmedi. Commit, push veya paket güncellemesi yapılmadı.
