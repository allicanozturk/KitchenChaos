# Checkpoint sonrası tam tekrar

Sahne: KitchenMovement_Test. Önceki `CheckpointEnemyHealthTest` yalnız yaşayan can yenileme davranışını anlatır; bu sahnede artık aşağıdaki kural geçerlidir.

1. CP_Fork sonrasında iki domates öldür, bir diğerini yarala; öl. Checkpoint'e dönüşte öldürülenler geri gelmeli, yaralı tam can olmalı. Hepsi başlangıç konum/devriye rotasında olmalı.
2. Bezelyeyi öldürüp öl; bezelye geri gelmeli ve normal hazırlıkla ateş etmeli. Yaralı bezelye için de tekrarla; eski mermi/ölüm parçaları kalmamalı.
3. Aynı denemeyi iki–üç kez tekrarla. Düşman sayısı artmamalı, can barı ve saldırılar normal kalmalı.
4. CP_Crouch'tan sonra bezelyeyi öldürüp CP_Hurt'a ulaş, sonra öl. CP_Hurt sonrasındaki domates yenilenmeli; artık önceki bölümde kalan bezelye yeniden doğmamalı.
5. CP_Crouch sonrası hareketli platform ilerledikten sonra öl. Başlangıç konum/yönüne dönmeli; eski platform kopyası kalmamalı.
6. Yeni checkpoint'e sadece dokunmak dünyayı anında sıfırlamamalı. Aynı checkpoint'e gidip gelmek denemenin sayaç kaydını değiştirmemeli.
7. Oyuncu checkpoint'te tam can/boş özel enerjiyle dönmeli; yakın/uzak/özel saldırı ve dash çalışmalı.

Toplanabilir altyapısı: checkpoint sonrası Coin kökleri kayıtlıysa geri kurulur ve PlayerScore checkpoint değerine döner. Mevcut test sahnesinde Coin bulunmuyor; bu davranış sonraki coinli kurulumda ayrıca test edilmeli. Yeni nesneler `_entries` ve uygun `Section` kaydına eklenmeden otomatik olarak reset kapsamına girmez.

Teknik: 5 checkpoint, 14 sıfırlama kökü. Pasif başlangıç kopyaları oyun başında, gameplay Awake öncesinde alınır; prefab sahne override'ları ve kök içi referanslar korunur. Fizik kökleri eski kopya Destroy edilmeden önce devre dışı bırakılır. Düşman ölümü/coin toplanması mevcut Destroy davranışını korur; yeniden doğuş canlı nesneyi canlandırmak değil başlangıç kopyasını kurmaktır. Normal eski sahneler değiştirilmedi. Asistan Play/build/otomatik test çalıştırmadı.

Statik doğrulama: Unity derlemesi başarılı; konsol hata/uyarı yok. 18 domates devriye referansı kendi reset kökünde; platformun 2 harici Route_A/Route_B referansı sıfırlanmayan sabit sahne işaretlerine bağlı (hareket eden üst bileşen yok). Reset kökleri arasında kırılacak rota referansı yok. Sahne kaydedildi. Runtime yeniden kurma/oynanış henüz kullanıcı tarafından denenmedi.
