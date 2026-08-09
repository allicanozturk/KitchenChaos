# Sprint13 Review

## Scope
- [ ] Damage hazard implemented
- [ ] Instant-kill mode implemented
- [ ] Spikes supported
- [ ] Lava/death zone supported
- [ ] Existing gameplay preserved

## Architecture
- [ ] Hazard is reusable
- [ ] PlayerHealth reused
- [ ] Player detection component-based
- [ ] No Singleton/GameManager/Find
- [ ] No unnecessary abstraction
- [ ] No manual YAML editing

## Damage Behaviour
- [ ] Configurable damage works
- [ ] Instant kill works through PlayerHealth
- [ ] Non-player objects ignored
- [ ] No accidental per-physics-frame damage
- [ ] Re-entry after respawn works

## Regression
- [ ] Latest checkpoint remains active
- [ ] Health restore works
- [ ] Movement/jump/camera/coins/enemy/combat/moving platforms/animations work
- [ ] Console clean
