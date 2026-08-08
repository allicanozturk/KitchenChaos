# Claude Prompt

Sprint10'u uygula.

Önce aşağıdakileri oku:

1. `.claude/CLAUDE.md`
2. `.claude/project/*`
3. `.claude/sprints/Sprint10/*`
4. Mevcut PlayerVisual, PlayerAttack, PlayerJump, EnemyPatrol ve Animator altyapısını

Amaç:

Mevcut placeholder kutuları, temel insan benzeri Player ve okunabilir Enemy görselleriyle değiştirecek görsel prototip sürecini hazırla.

Kurallar:

- Sprint10 kapsamı dışına çıkma.
- Final art üretme veya final sanat yönü belirleme.
- Mevcut gameplay kodlarını gereksiz yere değiştirme.
- PlayerVisual içindeki mevcut Animator parametrelerini kullan:
  - Speed
  - VerticalVelocity
  - IsGrounded
  - Attack
- Root motion kullanma.
- Scene, prefab, Animator Controller veya animation YAML dosyalarını elle düzenleme.
- Unity Editor kurulumu geliştirici tarafından yapılacak.
- Geçici assetlerin ileride kolayca değiştirilebilir olmasını sağla.
- Gereksiz manager, service, interface veya event bus oluşturma.
- Singleton, GameManager, GameObject.Find veya FindObjectOfType kullanma.
- Kod gerekmiyorsa gereksiz script oluşturma.

Bu sprint ağırlıklı olarak Unity Editor ve asset kurulumu içerir.

Belirsizlik varsa önce sor.

İş sonunda yalnızca şunları raporla:

- Kod değişikliği gerekip gerekmediği
- Gerekli geçici Player sprite türü ve animasyon kareleri
- Gerekli geçici Enemy sprite türü
- Sprite import ayarları
- Animator state ve transition kurulumu
- Collider ayarlama adımları
- Sorting Layer kurulumu
- TESTS.md'ye göre manuel test adımları
- Önerilen conventional commit mesajı
