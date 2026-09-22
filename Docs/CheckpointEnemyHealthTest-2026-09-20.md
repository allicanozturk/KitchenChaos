# Checkpoint — yaşayan düşman canı

Neden: PlayerRespawn düşman canını yenilemiyordu. Domates ve bezelye ortak EnemyHealth kullandığından ikisi de etkileniyordu.

Uygulama: oyuncu Respawning dinleyicileri çalıştıktan (eski mermiler temizlendikten) sonra aynı sahnenin yaşayan etkin düşmanları tam cana döner. Can barı ve darbe sunumu anında temizlenir; sersemleme/direnç ve özel geri itiş sıfırlanır. Bezelyenin mevcut atış toparlanması korunur. İyileşme hasar/isabet olayı değildir, oyuncuya enerji vermez.

Kapsam: öldürülmüş düşmanlar yeniden doğmaz, düşman konumları/devriye yolları ve toplanmış nesneler sıfırlanmaz. Checkpoint'e yalnız dokunmak düşmanları iyileştirmez; yeniden doğuş gerekir.

Kullanıcı testi:

1. Bezelyeye 1–2 normal vuruş yap, öldürmeden şefi öldür. Checkpoint dönüşünde bezelye tam can göstermeli ve yeniden 3 normal vuruş istemeli.
2. Aynısını domateste yap; bar ve gerçek can uyuşmalı, devriye devam etmeli.
3. İki tür yaralıyken öl; ikisi de yenilenmeli. İkinci ölüm-dönüşte de aynı olmalı.
4. Eski bezelye mermisi geri dönüşte kalmamalı; bezelye normal hazırlıkla yeniden ateş etmeli.
5. Yeni checkpoint'e sadece dokunmanın düşman canını doldurmadığını ve iyileşmeden enerji kazanılmadığını kontrol et.

Statik doğrulama: Unity derlemesi başarılı; son konsolda hata/uyarı yok. Asistan Play, build veya otomatik test çalıştırmadı. Oynanış kullanıcı testi bekliyor.
