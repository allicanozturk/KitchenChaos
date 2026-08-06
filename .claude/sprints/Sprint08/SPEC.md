# Sprint 08 - Basic Player Combat

## Business Goal
Allow the player to defeat the first enemy with a simple close-range attack.

## Technical Goal
Implement a minimal player attack and enemy health flow without breaking existing systems.

## Scope
- Basic attack input
- Short-range attack hitbox
- Configurable attack damage, radius and cooldown
- Enemy health
- Enemy death
- Enemy disappears at zero health

## Out of Scope
- Animation
- Combo
- Heavy attack
- Knockback
- Hit stop
- Camera shake
- Audio
- UI
- Projectiles
- Enemy drops
- Object pooling

## Functional Requirements
- Player can trigger a close-range attack.
- Only objects with EnemyHealth are damaged.
- Cooldown prevents per-frame attacks.
- Enemy dies once at zero health.
- Existing systems continue to work.

## Technical Requirements
- Reuse PlayerInputReader
- Use child Transform as attack origin
- Use Physics2D overlap query
- Use LayerMask
- No Singleton, GameManager, Find methods, or manual YAML edits

## Suggested Defaults
- Attack Damage: 1
- Attack Cooldown: 0.4
- Attack Radius: 0.75
- Enemy Max Health: 3

## Acceptance Criteria
- Attack input works.
- Enemy in range takes damage.
- Enemy out of range does not.
- Cooldown works.
- Enemy dies after expected hits.
- Dead enemy no longer patrols or damages Player.
- Console is clean.
