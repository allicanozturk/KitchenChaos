# Claude Prompt

Sprint12'yi uygula.

Önce aşağıdakileri oku:

1. `.claude/CLAUDE.md`
2. `.claude/project/*`
3. `.claude/sprints/Sprint12/*`
4. Mevcut PlayerMovement, PlayerJump, PlayerRespawn ve EnemyPatrol kodları

Amaç:

İki nokta arasında hareket eden ve Player üzerinde durduğunda Player'ı düzgün şekilde taşıyan reusable bir Moving Platform sistemi oluştur.

Kurallar:

- Sprint12 kapsamı dışına çıkma.
- Platform movement fizik güvenli olsun.
- Rigidbody2D kullan.
- Platform iki authored Transform noktası arasında gidip gelsin.
- Speed Inspector'dan ayarlanabilsin.
- Horizontal, vertical ve diagonal rotaları desteklesin.
- Player platform üzerinde dururken platformla birlikte taşınsın.
- Player input ile platform üzerinde hareket etmeye devam edebilsin.
- Player platformdan normal şekilde zıplayabilsin.
- PlayerMovement ve PlayerJump kodlarını gereksiz yere değiştirme.
- Player'ı platforma parent etme, eğer fiziksel olarak güvenli daha temiz bir çözüm varsa onu tercih et.
- Transform.Translate veya doğrudan Transform pozisyon yazımıyla fizik hareketi yapma.
- Singleton, GameManager, GameObject.Find, FindObjectOfType kullanma.
- Gereksiz interface, manager, service veya event bus oluşturma.
- Scene/prefab YAML dosyalarını elle düzenleme.
- Mevcut checkpoint, health, combat, camera, collectible, enemy ve animation sistemlerini bozma.
- Unity Editor kurulumu geliştirici tarafından yapılacak.

Player'ı taşıma yaklaşımı konusunda birden fazla mantıklı seçenek varsa implementasyondan önce sor.

İş sonunda yalnızca şunları raporla:

- Oluşturulan veya değiştirilen dosyalar
- Platform movement mimarisi
- Player carry mimarisi
- Neden bu yaklaşımın seçildiği
- Unity Editor kurulum adımları
- TESTS.md'ye göre manuel test adımları
- Önerilen conventional commit mesajı
