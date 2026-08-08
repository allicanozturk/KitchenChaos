# Sprint11 Review

## Scope
- [ ] Checkpoint activation implemented
- [ ] Respawn position update implemented
- [ ] Save/Load not added
- [ ] UI/audio/animation/autosave not added

## Architecture
- [ ] PlayerRespawn owns respawn position
- [ ] PlayerHealth owns death decision
- [ ] Checkpoint is focused
- [ ] No Singleton/GameManager/Find
- [ ] No unnecessary abstraction
- [ ] No manual YAML editing

## Behaviour
- [ ] Original spawn works before activation
- [ ] First checkpoint updates spawn
- [ ] Second checkpoint replaces first
- [ ] Non-player objects cannot activate
- [ ] Re-entering same checkpoint is safe

## Regression
- [ ] Velocity reset still works
- [ ] Health restore still works
- [ ] Movement/jump/camera/coins/enemy/combat/animations work
- [ ] Console clean
