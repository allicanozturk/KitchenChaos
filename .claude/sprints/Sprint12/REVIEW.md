# Sprint12 Review

## Scope

- [ ] Moving platform implemented
- [ ] Two-point movement implemented
- [ ] Player carry implemented
- [ ] Horizontal/vertical paths supported
- [ ] No unrelated platform types added
- [ ] Existing gameplay behaviour preserved

## Architecture

- [ ] Platform movement has focused responsibility
- [ ] Rigidbody2D is used
- [ ] Physics movement occurs in FixedUpdate
- [ ] Transform.Translate is not used for physics movement
- [ ] No Singleton/GameManager/Find methods
- [ ] No unnecessary abstraction
- [ ] No manual YAML editing

## Platform Movement

- [ ] Speed is configurable
- [ ] Point A and Point B are configurable
- [ ] Platform reverses correctly
- [ ] No endpoint overshoot
- [ ] Horizontal movement works
- [ ] Vertical movement works
- [ ] Diagonal movement remains valid

## Player Carry

- [ ] Player stays on platform while standing still
- [ ] Player can move while riding
- [ ] Player can jump normally
- [ ] Player does not visibly jitter
- [ ] Player velocity is not incorrectly overwritten
- [ ] Ground detection works on platform

## Regression

- [ ] Normal ground movement works
- [ ] Jump/coyote/buffer work
- [ ] Camera follows
- [ ] Coins work
- [ ] Health/respawn/checkpoints work
- [ ] Enemy/combat work
- [ ] Animations work
- [ ] Console clean
