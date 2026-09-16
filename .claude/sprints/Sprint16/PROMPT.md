# Claude Prompt

Sprint16'yı gerçekleştir.

Önce:
- .claude/CLAUDE.md
- .claude/project/*
- .claude/sprints/Sprint16/*
- PlayerJump, PlayerAttack, PlayerHealth, Coin, EnemyHealth, Checkpoint kodlarını
- Unity MCP üzerinden Level01 sahnesini ve projedeki mevcut audio assetlerini

incele.

İlk olarak projede kullanılabilir AudioClip var mı kontrol et.
Eğer yoksa bunu blocker olarak raporla ve implementasyona başlamadan önce bana sor.
Kendin final ses asseti üretme veya internetten indirme.

İstenen sesler:
1. Jump
2. Coin pickup
3. Player attack
4. Player damage
5. Enemy death
6. Checkpoint activation

Kurallar:
- Audio presentation olarak kalsın.
- Background music ekleme.
- AudioManager/Singleton/global service oluşturma.
- Mevcut gameplay kurallarını bozma.
- Mevcut event/callback varsa kullan.
- Yoksa minimum lokal entegrasyon yap.
- Büyük event mimarisi oluşturma.
- Missing clip gameplay'i bozmasın.
- Aynı olayda çift ses tetiklenmesin.
- Scene/prefab YAML elle düzenleme.
- Unity Editor işlemlerini mümkün olduğunca MCP ile yap.
- Level01 üzerinde kurulum yap; Bootstrap'ı gereksiz değiştirme.
- AudioSource Play On Awake kapalı, Spatial Blend 0 olsun.

İş sonunda raporla:
- oluşturulan/değiştirilen dosyalar
- her sesin hangi sinyalden tetiklendiği
- AudioSource mimarisi
- Unity MCP değişiklikleri
- AudioClip atamaları
- manuel test adımları
- conventional commit mesajı
