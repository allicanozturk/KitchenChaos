# Sprint 03 - Camera Follow

## Business Goal
Improve the gameplay experience by making the camera follow the player smoothly.

## Technical Goal
Configure a clean camera-follow setup using Cinemachine without adding unrelated camera effects.

## Scope
- Cinemachine camera follows the Player.
- Smooth horizontal and vertical tracking.
- Camera movement remains stable while the Player moves and jumps.
- Follow behaviour can be tuned from the Unity Inspector.
- Existing movement and jump systems continue to work.

## Out of Scope
- Camera shake
- Look-ahead
- Camera bounds or confiners
- Boss camera
- Cutscenes
- Dynamic zoom
- Camera transitions

## User Story
As a player, I can move and jump while the camera follows me smoothly so that the Player remains visible.

## Functional Requirements
- The camera follows the Player during horizontal movement.
- The camera follows the Player during jumping and falling.
- The camera does not jitter during normal gameplay.
- The Player remains visible in the Game view.
- Existing movement and jump behaviour remains unchanged.

## Technical Requirements
- Unity 6 LTS
- Cinemachine package
- Use Unity Editor configuration for scene objects and references.
- Do not manually edit scene or prefab YAML.
- Do not add camera logic to PlayerMovement or PlayerJump.
- Avoid custom camera scripts unless Cinemachine configuration alone is insufficient.
- Keep the implementation limited to Sprint03.

## Acceptance Criteria
- Player movement still works.
- Player jumping still works.
- Camera follows the Player horizontally.
- Camera follows the Player vertically.
- Camera motion is smooth.
- No visible jitter during movement or jumping.
- No Console errors or warnings introduced by Sprint03.

## Risks
- Incorrect Cinemachine target assignment.
- Multiple active cameras rendering simultaneously.
- Excessive damping causing delayed camera response.
- Very low damping causing harsh camera motion.
- Scene-specific setup not saved before committing.

## Future Extensions
- Camera look-ahead
- Camera confiner
- Camera shake
- Boss arena camera
- Dynamic zoom
- Cutscene cameras
