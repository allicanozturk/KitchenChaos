# Sprint 06 - Health System

## Business Goal

Introduce a basic player health system that supports taking damage, dying and respawning.

## Technical Goal

Implement a reusable Player Health component without affecting existing gameplay systems.

## Scope

- Player Health
- Maximum Health
- Current Health
- TakeDamage()
- Death
- Simple Respawn
- Inspector configurable values

## Out of Scope

- Enemy
- Combat
- Knockback
- Invulnerability Frames
- Checkpoints
- Save System
- UI
- Audio
- Animation

## User Story

As a player, I have health.
When my health reaches zero, I die and respawn.

## Functional Requirements

- Player has Max Health.
- Player has Current Health.
- Damage reduces Current Health.
- Health never drops below zero.
- When Health reaches zero the Player dies.
- Death immediately respawns the Player.
- Health is fully restored after respawn.

## Technical Requirements

- Unity 6 LTS
- C#
- Use MonoBehaviour
- Inspector configurable Max Health
- Do not create GameManager
- Do not use Singleton
- Do not use GameObject.Find
- Do not use FindObjectOfType
- Do not modify existing movement system
- Do not modify jump system
- Do not modify camera
- Do not edit scene YAML manually

## Acceptance Criteria

- Player receives damage.
- Health decreases correctly.
- Health never becomes negative.
- Death occurs at zero health.
- Respawn restores full health.
- Existing movement still works.
- Existing jump still works.
- Existing collectibles still work.
- Console has no errors.

## Risks

- Respawn occurring multiple times.
- Health becoming negative.
- Existing systems accidentally modified.

## Future Extensions

- Enemy damage
- Checkpoints
- Invulnerability
- Health UI
- Health pickups