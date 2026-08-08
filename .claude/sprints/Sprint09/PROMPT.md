# Claude Prompt

Sprint09'u uygula.

Önce aşağıdakileri oku:

1. `.claude/CLAUDE.md`
2. `.claude/project/*`
3. `.claude/sprints/Sprint09/*`
4. Mevcut PlayerInputReader, PlayerMovement, PlayerJump ve PlayerAttack kodları

Amaç:

Player'ın hareket yönüne bakmasını, AttackOrigin'in doğru tarafa geçmesini ve temel Animator parametrelerinin güncellenmesini sağlayan minimal bir görsel katman oluştur.

Kurallar:

- Sprint09 kapsamı dışına çıkma.
- Görsel sorumluluk ayrı bir PlayerVisual component'inde olsun.
- Sprite yönü için SpriteRenderer.flipX kullan.
- Transform scale ile yön değiştirme.
- Player durduğunda son baktığı yön korunsun.
- AttackOrigin sağ ve sol yön arasında aynalansın.
- Animator parametreleri:
    - Speed
    - VerticalVelocity
    - IsGrounded
    - Attack
- Final sprite veya animation clip üretme.
- Animator Controller veya scene/prefab YAML dosyalarını elle düzenleme.
- Root motion kullanma.
- Mevcut sistemleri bozma.
- Gereksiz interface, manager, service veya event bus oluşturma.
- Singleton, GameManager, GameObject.Find veya FindObjectOfType kullanma.
- Unity Editor kurulumu geliştirici tarafından yapılacak.

Grounded bilgisini mevcut mimaride güvenli şekilde paylaşmak gerekiyorsa en küçük değişikliği yap.
Attack animasyon trigger'ı için PlayerAttack ile en küçük ve temiz entegrasyonu kullan.

Belirsizlik varsa kod yazmadan önce sor.

İş sonunda yalnızca şunları raporla:

- Oluşturulan veya değiştirilen dosyalar
- Facing mimarisi
- AttackOrigin aynalama mantığı
- Animator parametre akışı
- Mevcut sınıflarda yapılan küçük değişiklikler
- Unity Editor kurulum adımları
- TESTS.md'ye göre test adımları
- Önerilen conventional commit mesajı
