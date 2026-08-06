# Sprint08 Review

## Scope
- [ ] Basic attack implemented
- [ ] Enemy health/death implemented
- [ ] No animation, combo, knockback, UI, audio, projectile or pooling
- [ ] Existing systems preserved

## Architecture
- [ ] Input ownership remains in PlayerInputReader
- [ ] Player attack is focused
- [ ] EnemyHealth is focused
- [ ] No Singleton/GameManager/Find
- [ ] No unnecessary abstraction
- [ ] No manual YAML edits

## Attack
- [ ] Origin, radius, damage and cooldown configurable
- [ ] LayerMask filters targets
- [ ] Same enemy not hit multiple times by one attack
- [ ] Unrelated objects not damaged

## Enemy Health
- [ ] Max health configurable
- [ ] Health never below zero
- [ ] Death once at zero
- [ ] Enemy removed after death
- [ ] Dead enemy no longer patrols or damages

## Regression
- [ ] Movement
- [ ] Jump
- [ ] Camera
- [ ] Coins
- [ ] Player health/respawn
- [ ] Enemy patrol/contact damage before death
- [ ] Console clean
