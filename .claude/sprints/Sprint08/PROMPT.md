# Claude Prompt

Sprint08'i uygula.

Önce:
- .claude/CLAUDE.md
- .claude/project/*
- .claude/sprints/Sprint08/*
- Mevcut PlayerInputReader, PlayerMovement, PlayerJump, PlayerHealth, EnemyPatrol ve EnemyContactDamage kodlarını

oku.

Kurallar:
- Sprint08 kapsamı dışına çıkma.
- Attack inputu PlayerInputReader içinde kalsın.
- Player attack ayrı component olsun.
- Attack origin child Transform olsun.
- Physics2D overlap query ve LayerMask kullan.
- Damage, radius ve cooldown Inspector'dan ayarlanabilsin.
- EnemyHealth ayrı component olsun.
- Enemy sıfır health'te yalnızca bir kez ölsün ve sahneden kaldırılsın.
- Mevcut sistemleri bozma.
- Animation, combo, knockback, hit stop, audio, UI, projectile, drop veya pooling ekleme.
- Singleton, GameManager, GameObject.Find, FindObjectOfType kullanma.
- Gereksiz abstraction oluşturma.
- Scene/prefab YAML dosyalarını elle düzenleme.
- Unity Editor kurulumu geliştirici tarafından yapılacak.

Belirsizlik varsa önce sor.

İş sonunda raporla:
- Oluşturulan/değiştirilen dosyalar
- Attack input mimarisi
- Player attack mimarisi
- Enemy health/death mimarisi
- Aynı saldırının aynı enemy'yi iki kez vurmasının nasıl engellendiği
- Unity kurulum adımları
- Test adımları
- Commit mesajı
