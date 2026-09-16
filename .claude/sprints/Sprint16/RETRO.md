# Sprint16 Retrospective

## What went well

- PlayerAttack.Attacked zaten vardi; attack sesi icin gameplay koduna hic
  dokunmadan baglandi. Sprint9'da event'i eklemis olmak burada geri odedi.
- Diger bes sinyal tek satirlik event invoke ile eklendi. Hicbir karar,
  kosul veya siralama degismedi, yani regression yuzeyi cok dar kaldi.
- Unity tarafinin tamami MCP ile kuruldu; tek bir YAML satiri elle
  duzenlenmedi. AudioSource ve component ozellikleri MCP resource'lari ile
  tek tek dogrulandi.

## Problems encountered

- Projede hic AudioClip yoktu. Blocker olarak raporlandi.
- MCP generate_audio icin fal.ai anahtari editorde tanimli degil
  (configured: false), yani onaylanan AI uretimi kullanilamadi. Cozum olarak
  indirme yapmadan, Python stdlib ile sentezlenmis gecici WAV'lar uretildi.
- Coin ve Enemy pickup/olum aninda Destroy ediliyor, yani kendi uzerlerindeki
  bir AudioSource sesi keserdi. Coin icin ses player tarafina, enemy icin
  sahnede yasayan bir AudioSource'a tasindi.
- EnemyDeathAudio'ya AudioSource referansi int instanceID olarak verilince MCP
  konsola "Unexpected token type 'Integer'" uyarisi basti, ama referans dogru
  cozuldu (uc enemy'de de dogrulandi). Bir dahaki sefere obje referanslarini
  {"path": ...} / {"guid": ...} formatinda gecmek daha temiz.

## Temporary Player asset used

N/A - bu sprintte gorsel asset degismedi.

## Temporary Enemy asset used

N/A - bu sprintte gorsel asset degismedi.

## Sprite import values

N/A - sprite import edilmedi.

## Animator setup completed

N/A - Animator'e dokunulmadi. Attack sesi Animator event'i degil,
PlayerAttack.Attacked C# event'i uzerinden calisiyor.

## Collider adjustments

N/A - collider degistirilmedi.

## Files changed

Yeni:
- Assets/Scripts/Audio/PlayerAudio.cs
- Assets/Scripts/Audio/CheckpointAudio.cs
- Assets/Scripts/Audio/EnemyDeathAudio.cs
- Assets/Audio/Placeholder/*.wav (6 gecici clip)

Degisen (sadece event eklendi):
- Assets/Scripts/Player/PlayerJump.cs      -> Jumped
- Assets/Scripts/Player/PlayerHealth.cs    -> Damaged
- Assets/Scripts/Player/PlayerScore.cs     -> ScoreAdded
- Assets/Scripts/Enemy/EnemyHealth.cs      -> Died
- Assets/Scripts/Level/Checkpoint.cs       -> Activated
- Assets/Scenes/Level01.unity              -> AudioSource + audio component wiring

## Improvements for Sprint17

- Placeholder SFX'leri gercek sound design ile degistir. Kod degismeyecek,
  sadece Inspector'daki AudioClip alanlari.
- Volume/pitch varyasyonu ve mixer grubu hala kapsam disi; ses sayisi artmaya
  baslarsa once mixer, sonra varyasyon dusunulmeli.
- PlayerAudio dort clip tasiyor. Bes-alti'yi gecerse SFX tanimlarini bir
  ScriptableObject'e almak mantikli olur, ama simdi erken olurdu.

## Commit Hash

(commit atilmadi - onerilen mesaj raporda)

## Notes

- Mimari: sesi cikaran obje sesini asiyorsa kendi AudioSource'unu tasir
  (Player, Checkpoint). Asamiyorsa (Enemy olurken Destroy ediliyor) sahnedeki
  LevelAudio AudioSource'u Inspector'dan referansla kullanilir. Manager, static
  veya lookup yok.
- Butun ses component'leri tek yonlu: gameplay'i dinler, geri cagirmaz.
  Component'ler silinse oyun aynen calisir.
