# Claude Prompt

Sprint13'ü uygula.

Önce oku:
- .claude/CLAUDE.md
- .claude/project/*
- .claude/sprints/Sprint13/*
- Mevcut PlayerHealth, PlayerRespawn, Checkpoint ve EnemyContactDamage kodları

Amaç:
Spike ve lava/death-zone gibi çevresel hazard'ların mevcut PlayerHealth sistemini kullanarak Player'a hasar vermesini sağla.

Kurallar:
- Sprint13 kapsamı dışına çıkma.
- Hazard reusable ve ayrı bir component olsun.
- Trigger tabanlı çalışsın.
- Player tespiti component tabanlı olsun; tag/name kullanma.
- Damage Inspector'dan ayarlanabilsin.
- Instant-kill mode desteklensin.
- Instant kill PlayerHealth akışını bypass etmesin.
- PlayerHealth/PlayerRespawn mimarisini gereksiz değiştirme.
- Checkpoint respawn akışını bozma.
- Non-player objeleri yok say.
- Knockback, invulnerability, animation, audio, VFX, moving/timed trap ekleme.
- Singleton, GameManager, GameObject.Find, FindObjectOfType kullanma.
- Gereksiz abstraction oluşturma.
- Scene/prefab YAML dosyalarını elle düzenleme.
- Unity Editor kurulumu geliştirici tarafından yapılacak.

Temas hasarının tekrar davranışı için birden fazla mantıklı seçenek varsa implementasyondan önce sor.

İş sonunda raporla:
- Oluşturulan/değiştirilen dosyalar
- Hazard damage mimarisi
- Instant-kill akışı
- Player'ın nasıl tanındığı
- Unity Editor kurulum adımları
- Manuel test adımları
- Conventional commit mesajı
