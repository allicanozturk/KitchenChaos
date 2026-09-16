# Sprint 16 - Basic Sound Effects

## Business Goal
Add basic sound feedback to the core gameplay so actions feel more responsive.

## Technical Goal
Add a minimal audio presentation layer for existing gameplay events without moving gameplay responsibility into audio code.

## Scope
- Jump sound
- Coin pickup sound
- Player attack sound
- Player damage sound
- Enemy death sound
- Checkpoint activation sound
- AudioSource setup via Unity MCP
- Inspector AudioClip references

## Out of Scope
- Music
- Audio mixer
- Volume settings
- Persistent AudioManager
- Footsteps
- Ambient loops
- Dynamic music
- Voice
- Final production SFX
- Pooling

## Requirements
- One sound per accepted gameplay event
- Missing clips must not break gameplay
- Existing gameplay systems stay unchanged
- No Singleton/GameManager/Find methods
- No manual YAML editing
- Unity setup via MCP where possible

## Acceptance Criteria
- All six requested sounds can play
- No duplicate triggers
- Existing gameplay still works
- Console clean
