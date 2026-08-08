# Claude Prompt

Sprint11'i uygula.

Önce oku:
- .claude/CLAUDE.md
- .claude/project/*
- .claude/sprints/Sprint11/*
- Mevcut PlayerHealth ve PlayerRespawn kodları

Amaç:
Player checkpoint'e dokunduğunda respawn noktasını güncelle.

Kurallar:
- Sprint11 kapsamı dışına çıkma.
- PlayerHealth ölüm akışını değiştirme.
- Respawn konumunun sahibi PlayerRespawn olarak kalsın.
- PlayerRespawn için yalnızca minimum gerekli API'yi ekle.
- Checkpoint ayrı ve odaklı component olsun.
- Trigger tabanlı çalışsın.
- Player tespiti component tabanlı olsun.
- Save/Load, UI, animation, audio, scene transition veya autosave ekleme.
- Singleton, GameManager, Find metotları kullanma.
- Gereksiz abstraction oluşturma.
- Scene/prefab YAML dosyalarını elle düzenleme.
- Mevcut gameplay sistemlerini bozma.
- Unity Editor kurulumu geliştirici tarafından yapılacak.
- Checkpoint world position respawn pozisyonu olsun.

Belirsizlik varsa önce sor.

İş sonunda raporla:
- Oluşturulan/değiştirilen dosyalar
- PlayerRespawn değişikliği
- Checkpoint aktivasyon akışı
- Player'ın nasıl tanındığı
- Unity Editor kurulum adımları
- Manuel test adımları
- Conventional commit mesajı
