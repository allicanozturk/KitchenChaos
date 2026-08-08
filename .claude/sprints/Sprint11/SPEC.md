# Sprint 11 - Checkpoint System

## Business Goal
Allow the player to activate checkpoints so death no longer always returns them to the original level start.

## Technical Goal
Extend the existing respawn flow so PlayerRespawn owns a mutable respawn position that can be updated by checkpoint objects without changing PlayerHealth or other gameplay systems.

## Scope
- Checkpoint GameObject
- Trigger-based checkpoint activation
- PlayerRespawn exposes a safe API for updating the respawn position
- Active checkpoint becomes the new respawn location
- Player respawns at the latest activated checkpoint
- Existing systems continue to work

## Out of Scope
- Save/Load
- Persistent checkpoints between sessions
- Checkpoint UI
- Checkpoint animation/audio
- Level selection
- Scene transitions
- Autosave

## Functional Requirements
- Original scene spawn is used before any checkpoint.
- Touching a checkpoint updates the respawn position.
- A second checkpoint replaces the previous one.
- Only the Player can activate checkpoints.
- PlayerHealth still owns death.
- PlayerRespawn still owns teleport/respawn position.

## Technical Requirements
- Unity 6 LTS
- Reuse PlayerRespawn
- Trigger-based detection
- No Singleton, GameManager, GameObject.Find or FindObjectOfType
- No manual scene/prefab YAML editing
- Unity Editor setup is manual

## Acceptance Criteria
- Original spawn works before checkpoint activation.
- Checkpoint activation changes respawn position.
- Second checkpoint replaces first.
- Respawn resets velocity and restores health.
- Existing gameplay works.
- Console stays clean.

## Future Extensions
- Visual activation state
- Audio
- Persistent save
- Level progression
