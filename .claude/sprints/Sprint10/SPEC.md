# Sprint 10 - Basic Visual Prototype

## Business Goal

Replace the current primitive box placeholders with readable temporary visuals so the game begins to feel like an actual platform game.

## Technical Goal

Import and configure simple placeholder character art for the Player and Enemy, connect basic animation clips to the existing Animator foundation, and align colliders without changing gameplay behaviour.

## Scope

- Temporary human-like Player sprite set
- Temporary Enemy sprite set
- Player animations:
    - Idle
    - Run
    - Jump
    - Fall
    - Attack
- Basic Enemy idle or walk animation if available
- Sprite import settings
- Pixels Per Unit and pivot configuration
- Player Animator states and transitions
- Collider alignment for the new sprites
- Sorting Layer setup
- Existing gameplay systems remain unchanged

## Out of Scope

- Final production art
- Final character design
- Detailed VFX
- Lighting polish
- Final UI
- Audio
- Enemy combat animations
- Hit reactions
- Death animations
- Root motion
- Skeletal animation

## User Stories

- As a player, I can visually distinguish the Player from the Enemy.
- As a player, I can understand whether the character is idle, running, jumping, falling, or attacking.
- As a developer, I can test gameplay with readable character visuals instead of rectangles.

## Functional Requirements

- Player sprite displays correctly in the scene.
- Enemy sprite displays correctly in the scene.
- Player faces left and right using the existing flip logic.
- Idle animation plays while standing.
- Run animation plays while moving on the ground.
- Jump animation plays while rising.
- Fall animation plays while descending.
- Attack animation plays when attacking.
- Existing AttackOrigin continues to mirror correctly.
- Colliders remain aligned with the visible sprites.
- Existing gameplay behaviour remains unchanged.

## Technical Requirements

- Unity 6 LTS
- Use the existing PlayerVisual and Animator parameter flow
- Use SpriteRenderer
- Use Animator Controller states and transitions
- Do not use root motion
- Do not modify movement, jump, combat, health, camera, collectible, or enemy logic unless a genuine compatibility issue is found
- Do not manually edit scene, prefab, Animator Controller, or animation YAML
- Unity Editor configuration is performed manually
- Temporary assets must be easy to replace later

## Suggested Sorting Layers

- Background
- Ground
- Characters
- Collectibles
- Foreground

## Acceptance Criteria

- Player and Enemy are no longer represented by plain rectangles.
- Player animations transition correctly between Idle, Run, Jump, Fall, and Attack.
- Facing left and right still works.
- Attacks still work in both directions.
- Player collider matches the temporary character sprite.
- Enemy collider matches the temporary enemy sprite.
- Coins, health, respawn, patrol, contact damage, and combat still work.
- Console contains no new errors or warnings.

## Risks

- Sprite pivot causes visible sliding
- Pixels Per Unit makes sprites too large or too small
- Collider no longer matches the visible body
- Animator transitions interrupt attack incorrectly
- Temporary art becomes too tightly coupled to gameplay
- Root motion accidentally moves the Player

## Future Extensions

- Final Player art
- Final Enemy art
- Hit reactions
- Death animations
- Visual effects
- Lighting polish
- Environment art pass
