# Sprint 13 - Hazards

## Business Goal
Add environmental hazards so the level can punish mistakes and create traversal challenges.

## Technical Goal
Implement reusable hazard components that damage or instantly kill the Player by reusing the existing PlayerHealth system.

## Scope
- Reusable damage hazard
- Spike hazard setup
- Lava/death-zone setup
- Inspector-configurable damage
- Optional instant-kill mode
- Trigger-based detection
- Existing PlayerHealth/respawn flow reused
- Existing gameplay systems preserved

## Out of Scope
- Hazard animation/audio/VFX
- Knockback
- Invulnerability frames
- Moving hazards
- Timed traps
- Projectile traps
- Save/Load
- UI changes

## Functional Requirements
- Hazard only affects objects with PlayerHealth.
- Damage amount is configurable.
- Instant-kill mode is supported.
- Damage is not accidentally applied every physics frame.
- Death/respawn continues through PlayerHealth.
- Latest checkpoint remains the respawn point.
- Non-player objects are ignored.
- Re-entering hazards after respawn works.

## Technical Requirements
- Unity 6 LTS
- Trigger collider required
- Component-based Player detection
- Reuse PlayerHealth
- No Singleton/GameManager/Find methods
- No manual YAML editing
- Unity Editor setup is manual

## Suggested Defaults
- Damage Hazard Damage: 1
- Instant Kill: false
- Lava/Death Zone Instant Kill: true

## Acceptance Criteria
- Spikes reduce health.
- Lava/death zone can kill instantly.
- Existing respawn flow works.
- Latest checkpoint remains active.
- Existing gameplay remains functional.
- Console stays clean.
