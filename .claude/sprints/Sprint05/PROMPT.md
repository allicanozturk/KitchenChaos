# Claude Prompt

Sprint05'i uygula.

Önce aşağıdakileri oku:

1. `.claude/CLAUDE.md`
2. `.claude/project/*`
3. `.claude/sprints/Sprint05/*`
4. Mevcut Player kodları

Amaç:

Player'ın sahnedeki coin nesnelerini toplayabildiği basit bir collectible sistemi oluştur.

Kurallar:

- Sprint05 kapsamı dışına çıkma.
- Coin toplandığında score artsın ve coin sahneden kaldırılsın.
- Coin değeri Inspector üzerinden ayarlanabilir olsun.
- Aynı coin yalnızca bir kez toplanabilsin.
- Singleton kullanma.
- GameManager oluşturma.
- UI, animasyon, ses, save, inventory, object pool veya particle sistemi ekleme.
- GameObject.Find ve FindObjectOfType kullanma.
- Sahne veya prefab YAML dosyalarını elle düzenleme.
- Mevcut movement, jump ve camera kodlarını değiştirme.
- Gereksiz interface, event bus, service veya abstraction oluşturma.
- Unity Editor kurulumunu geliştirici yapacak; yalnızca C# dosyalarını oluştur veya değiştir.

Uygulamadan önce score sahipliğinin en basit ve temiz biçimini belirle.

Belirsizlik varsa kod yazmadan önce sor.

İş sonunda yalnızca şunları raporla:

- Oluşturulan veya değiştirilen dosyalar
- Score değerinin nerede tutulduğu
- Coin'in Player'ı nasıl tanıdığı
- Aynı coin'in iki kez toplanmasının nasıl engellendiği
- Unity Editor'da yapılacak kurulum adımları
- TESTS.md'ye göre test adımları
- Önerilen conventional commit mesajı
