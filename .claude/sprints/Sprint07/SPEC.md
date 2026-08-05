# Sprint 07 - First Enemy

## Business Goal

Introduce the first enemy into the game world.

The enemy should patrol between two points and damage the player on contact.

## Technical Goal

Create a reusable enemy system that can patrol and damage the player without affecting existing gameplay systems.

## Scope

- Enemy Patrol
- Patrol Points
- Contact Damage
- Inspector configurable patrol speed
- Inspector configurable damage amount

## Out of Scope

- Enemy AI
- Chasing Player
- Enemy Attack Animation
- Enemy Death
- Knockback
- Enemy Health
- Combat
- UI
- Audio

## User Story

As a player,
I can encounter an enemy that patrols back and forth.
Touching the enemy causes damage.

## Functional Requirements

- Enemy moves continuously between two patrol points.
- Enemy changes direction at patrol limits.
- Enemy damages the player on contact.
- Damage amount is configurable.
- Enemy never damages anything except the player.
- Existing Player Health system is reused.

## Technical Requirements

- Unity 6 LTS
- C#
- MonoBehaviour based
- No Singleton
- No GameManager
- No GameObject.Find
- No FindObjectOfType
- No manual Scene YAML editing
- Reuse PlayerHealth
- Reuse existing architecture

## Acceptance Criteria

- Enemy patrols correctly.
- Enemy reverses direction.
- Player loses one health on contact.
- Player respawns after health reaches zero.
- Camera still follows.
- Jump still works.
- Coins still work.
- Console contains no errors.

## Risks

- Patrol overshoots patrol points.
- Damage applied every physics frame unintentionally.
- Existing movement affected.

## Future Extensions

- Chase AI
- Enemy Attack
- Enemy Health
- Enemy Death
- Enemy Animation
- Multiple enemy types