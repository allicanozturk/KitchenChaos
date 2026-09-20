# KitchenIntro_Blockout — ilk kontrol kaydı

16 Eylül 2026 / Unity 6000.5.6f1 / Editor Play Mode. Build veya Unity Test Runner çalıştırılmadı. Kalıcı C# script değişikliği yapılmadı.

## Oluşturulanlar

- `Assets/Scenes/KitchenIntro_Blockout.unity`: güvenli zemin, iki basamak, checkpoint, bir devriye düşmanı, yedi ödül ve pasif bitiş işareti.
- `Assets/Prefabs/VerticalSlice`: Player_Prototype, Counter_Block, Coin_Prototype, Checkpoint_Prototype, PatrolEncounter_Prototype.
- Mevcut animasyonlar ve Sprint16 placeholder sesleri yeniden kullanıldı. Yeni mutfak karakteri, tava/bıçak görselleri, menzilli saldırı veya boss henüz yapılmadı.
- Küçük düşmana erişebilmek için yalnızca yeni oyuncu prefabının AttackOrigin konumu ayarlandı. Hareket fiziği ve eski Level01 değiştirilmedi.
- Devriye karşılaşması prefabı kendi devriye noktalarını ve düşmanın ölümünden sonra yaşayan ses kaynağını içerir; dış sahne referansı gerekmez.

## Teknik sonuç

Sahne doğrulaması: 0 eksik script, 0 bozuk prefab. KitchenChaos bileşenlerinin ve beş prefabın serileştirilmiş oyun referanslarında boş bağlantı bulunmadı.

Kontrollü Play Mode denemesi, geçici sanal klavye ile mevcut Input System bağlarına giriş gönderdi. Başlangıç konumu sıfırlandı; test süresince oyuncunun input asset'i yalnızca test cihazını dinledi. Sonunda cihaz kaldırıldı ve önceki giriş filtresi geri yüklendi. Bu bir otomatik kısa kontroldür, insan oynanış değerlendirmesinin yerine geçmez.

| Kontrol | Sonuç | Gözlem |
| --- | --- | --- |
| Sağa hareket | Geçti | Oyuncu X=4.33 konumuna ilerledi |
| Ödül toplama | Geçti | Puan 2 oldu; yedi ödülün tamamı ayrı ayrı test edilmedi |
| Birinci basamağa zıplama | Geçti | X=10.30, Y=0.50; yerde; gözlenen tepe Y=4.51 |
| İkinci basamağa zıplama | Geçti | X=16.30, Y=2.00; yerde |
| Checkpoint ve ölüm | Geçti | Checkpoint'e temasın ardından ölümcül hasar; X=26'ya, 3 canla dönüş |
| Sağa saldırı | Geçti | Düşman canı 3 → 2 |
| Sola saldırı | Geçti | Düşman canı 2 → 1 |
| Düşmanın ölümü | Geçti | Son vuruş sonrası düşman kaldırıldı |

Checkpoint ve saldırı kontrollerini ayırmak için oyuncu test sırasında ilgili alanlara taşındı; saldırı testinde devriye geçici olarak durduruldu. Kesintisiz baştan sona yürüyüş, hareketli düşmanla savaş hissi ve bütün ödüllerin toplanması bu otomatik kontrolün kapsamı değildir. Bu geçici durumlar sahneye kaydedilmedi.

İlk test sırasında kullanıcı da karakteri kontrol ettiği için ilk koşu kabul kanıtı sayılmadı. İzole son koşudaki sekiz kontrol geçti. Son koşunun ardından okunan Console hata/uyarı listesi boştu. Daha önceki görüntü alma sırasında iki memoryless depth uyarısı görüldü; bu aşamada render/paket ayarları değiştirilmedi. Ses dosyalarının bağlantıları korundu; ses kalitesi işitsel olarak onaylanmadı.

## Kullanıcı denemesi

`KitchenIntro_Blockout` sahnesinde Play'e bas, Game görünümüne tıkla. A/D veya oklar: yürü; Space: zıpla; Enter veya sol tık: yakın saldırı. Menzilli silah henüz yok.

1. İki basamağı geç ve ödülleri topla. Kamera ve yazılar rahat okunuyor mu?
2. Turkuaz checkpoint'e temas et. Düşmana yenilirsen bu alana dönüyor musun?
3. Düşmana sağından ve solundan yaklaşarak vur. Saldırı mesafesi anlaşılır mı?
4. Sağdaki bitiş işaretine ulaş. Bu işaret henüz sahne geçişi veya zafer ekranı çalıştırmaz.

İlk geri bildirimde üç şey yeterli: zıplama fazla yüksek/yavaş mı, kamera fazla uzak mı, vurmak kolay anlaşılıyor mu? Süre ve eğlence henüz kabul edilmedi.

## Koruma kontrolü

Önceden var olan staged değişikliklere dokunulmadı; index'e yeni dosya eklenmedi, commit/push yapılmadı. `git diff --name-only` kontrolünde tracked dosyalara ek unstaged değişiklik yoktu.

İşlem öncesi ve sonrası Level01 SHA-256:
`acf17dc1bf299db961ac932d8a1e1bff78edda525651f58501d8cc1d2eb589f2`

İşlem öncesi ve sonrası staged binary diff SHA-256:
`4567172b8153f50ffb875e77559f915ff249d29c6723d7cf2e22616ce0b24943`

Sıradaki iş: insan oynanış geri bildirimi, ardından yol haritasının ikinci aşamasındaki savaş/yeniden doğma iyileştirmeleri. Geniş kapsamlı yeniden yazım yok.
