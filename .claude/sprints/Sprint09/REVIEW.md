# Sprint09 Review

## Scope
- [ ] Player facing implemented
- [ ] AttackOrigin mirroring implemented
- [ ] Animator parameter flow implemented
- [ ] Final art or clips not created
- [ ] Enemy animations not added
- [ ] Existing gameplay behaviour preserved

## Architecture
- [ ] Visual logic is isolated
- [ ] SpriteRenderer.flipX is used
- [ ] Transform scale is not inverted
- [ ] No root motion
- [ ] No Singleton or GameManager
- [ ] No Find methods
- [ ] No unnecessary abstraction
- [ ] No manual YAML editing

## Facing
- [ ] Moving right faces right
- [ ] Moving left faces left
- [ ] Idle preserves last direction
- [ ] AttackOrigin mirrors correctly
- [ ] Attack works on both sides

## Animator
- [ ] Speed parameter updates
- [ ] VerticalVelocity parameter updates
- [ ] IsGrounded parameter updates
- [ ] Attack trigger fires once per accepted attack
- [ ] Parameter names are consistent
- [ ] Missing references fail clearly

## Regression
- [ ] Movement works
- [ ] Jump and jump assists work
- [ ] Camera follows
- [ ] Coins work
- [ ] Player health/respawn work
- [ ] Enemy patrol/contact damage work
- [ ] Enemy health/death work
- [ ] Console clean
