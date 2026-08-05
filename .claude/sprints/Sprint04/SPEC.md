# Sprint 04 - Responsive Jump Assistance

## Business Goal
Make jumping feel more forgiving and responsive without changing the core movement style.

## Technical Goal
Add coyote time and jump buffering to the existing jump system while preserving the separation between input, movement, and jumping responsibilities.

## Scope
- Coyote time
- Jump buffer
- Inspector-configurable timing values
- Existing horizontal movement remains unchanged
- Existing single-jump rule remains unchanged
- Existing ground detection continues to be used

## Out of Scope
- Variable jump height
- Double jump
- Wall jump
- Dash
- Animation
- Audio
- Camera changes
- Combat

## User Stories
- As a player, I can still jump for a very short time after leaving a platform edge.
- As a player, I can press jump shortly before landing and jump automatically on contact.

## Functional Requirements
- A jump pressed shortly after leaving the ground succeeds within the configured coyote-time window.
- A jump pressed shortly before landing is stored within the configured jump-buffer window.
- Buffered jump input expires if the player does not land before the configured window ends.
- Airborne jump presses outside the buffer window do not cause a jump after landing.
- The player still cannot perform a true double jump.
- Existing left/right movement and Cinemachine camera follow continue to work.

## Technical Requirements
- Unity 6 LTS
- Rigidbody2D
- Unity New Input System
- Reuse PlayerInputReader
- Reuse the current ground-check approach
- Timing values must use `[SerializeField, Min(0f)]`
- Physics changes must remain in `FixedUpdate()`
- Input capture must remain frame-safe
- Do not add unnecessary managers, services, interfaces, or event buses
- Do not edit scene or prefab YAML manually

## Suggested Default Values
- Coyote Time: 0.12 seconds
- Jump Buffer Time: 0.15 seconds

## Acceptance Criteria
- Player can jump immediately after stepping off a ledge within the coyote-time window.
- Player cannot jump after the coyote-time window expires.
- Player can press jump shortly before landing and jump on contact.
- Expired buffered input does not trigger a later jump.
- No true double jump is possible.
- Existing movement, jumping, ground detection, and camera follow still work.
- Console contains no new errors or warnings.

## Risks
- Accidentally introducing unlimited or double jumping
- Keeping buffered input alive too long
- Mixing frame time and fixed time incorrectly
- Breaking the current takeoff-protection logic
- Making the jump feel automatic or unpredictable

## Future Extensions
- Variable jump height
- Apex gravity tuning
- Fall gravity multiplier
- Double jump
- Wall jump
