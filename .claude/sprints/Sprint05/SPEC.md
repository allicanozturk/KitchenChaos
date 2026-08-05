# Sprint 05 - Collectible System

## Business Goal
Add the first simple gameplay objective by allowing the player to collect coins placed in the level.

## Technical Goal
Implement a small, reusable collectible system that detects the Player, increases a score value, and removes the collected object from the scene.

## Scope
- Coin collectible
- Trigger-based collection
- Score tracking
- Inspector-configurable coin value
- Coin disappears after collection
- Existing movement, jump, jump assistance, and camera systems remain unchanged

## Out of Scope
- Score UI
- Coin animation
- Coin sound
- Save system
- Inventory
- Multiple collectible types
- Object pooling
- Particle effects
- Achievements

## User Story
As a player, I can touch a coin to collect it and increase my score.

## Functional Requirements
- Coin uses a Collider2D configured as Trigger.
- Coin detects the Player when the Player enters the trigger.
- Coin increases the current score by its configured value.
- Coin can only be collected once.
- Coin is removed from the scene after collection.
- Multiple coins can be collected independently.

## Technical Requirements
- Unity 6 LTS
- C#
- Use focused MonoBehaviour components
- Use `[SerializeField, Min(1)]` for the coin value
- Do not use GameObject.Find
- Do not use FindObjectOfType
- Do not use Singleton
- Do not create GameManager
- Do not create UI in this sprint
- Do not manually edit scene or prefab YAML
- Keep the implementation minimal

## Acceptance Criteria
- Player can collect a coin by touching it.
- Score increases by the coin value.
- Coin disappears after collection.
- The same coin cannot be collected twice.
- At least three coins can be placed and collected independently.
- Existing gameplay systems still work.
- Console contains no new errors or warnings.

## Risks
- Trigger is not enabled on the coin collider.
- Player identification is unreliable.
- Collection happens more than once before destruction.
- Score ownership becomes unnecessarily complex.

## Future Extensions
- Score UI
- Audio feedback
- Visual effects
- Animated collectibles
- Different collectible types
- Save integration
