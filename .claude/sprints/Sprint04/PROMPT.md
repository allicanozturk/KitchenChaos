# Claude Prompt

Sprint04'ü uygula.

Önce aşağıdakileri oku:

1. `.claude/CLAUDE.md`
2. `.claude/project/*`
3. `.claude/sprints/Sprint04/*`
4. Mevcut `PlayerInputReader`, `PlayerMovement` ve `PlayerJump` implementasyonları

Amaç:

Mevcut zıplama sistemine yalnızca coyote time ve jump buffer ekle.

Kurallar:

- Sprint04 kapsamı dışına çıkma.
- `PlayerMovement` sorumluluğunu değiştirme.
- Input sahipliği `PlayerInputReader` içinde kalmalı.
- Mevcut ground-check yaklaşımını koru.
- Gerçek double jump ekleme.
- Variable jump, wall jump, dash, animation, audio, camera veya combat ekleme.
- Gereksiz sınıf, interface, manager, service veya event bus oluşturma.
- Mevcut çalışan hareket, zıplama ve kamera davranışlarını bozma.
- Inspector üzerinden ayarlanabilen başlangıç değerleri kullan:
    - Coyote Time: 0.12
    - Jump Buffer Time: 0.15
- Input süresi ile fizik adımlarını güvenli şekilde koordine et.
- Sahne veya prefab YAML dosyalarını elle düzenleme.
- Yalnızca gerçekten gerekli dosyaları değiştir.

Uygulamadan önce mevcut `PlayerJump` mantığını analiz et.

Belirsizlik varsa kod yazmadan önce sor.

İş sonunda yalnızca şunları raporla:

- Değiştirilen veya oluşturulan dosyalar
- Coyote time mantığının kısa açıklaması
- Jump buffer mantığının kısa açıklaması
- Mevcut double-jump engelinin nasıl korunduğu
- Unity Inspector'da yapılacak ayarlar
- TESTS.md'ye göre beklenen sonuçlar
- Önerilen conventional commit mesajı
