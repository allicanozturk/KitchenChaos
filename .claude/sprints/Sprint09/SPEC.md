# Sprint 09 - Player Facing and Animation Foundation

## Business Goal
Improve player readability by making the character face the movement direction and preparing a clean animation state flow.

## Technical Goal
Add a focused PlayerVisual component that controls horizontal facing and Animator parameters without changing gameplay systems.

## Scope
- Player faces left or right according to horizontal movement
- SpriteRenderer flip is used for facing
- AttackOrigin mirrors to the active facing direction
- Animator integration for Idle, Run, Jump, Fall, and Attack
- Inspector references for SpriteRenderer, Animator, Rigidbody2D, and AttackOrigin
- Existing gameplay systems remain unchanged

## Out of Scope
- Final art
- Final animation clips
- Enemy animations
- Hit reactions
- Death animations
- Animation events
- Root motion
- Combo animations
- VFX
- Audio

## Functional Requirements
- Moving right faces right.
- Moving left faces left.
- Standing still preserves the last facing direction.
- AttackOrigin moves to the correct side when facing changes.
- Animator receives horizontal speed.
- Animator receives vertical velocity.
- Animator receives grounded state.
- Animator receives an attack trigger.
- Existing systems continue to work.

## Technical Requirements
- Unity 6 LTS
- Reuse PlayerInputReader and Rigidbody2D
- Use SpriteRenderer.flipX
- Do not invert Transform scale
- Do not use root motion
- Keep visual logic separate from gameplay logic
- No Singleton, GameManager, GameObject.Find, or FindObjectOfType
- Do not manually edit scene, prefab, Animator Controller, or animation YAML
- Unity Editor setup is performed manually

## Suggested Animator Parameters
- Speed: float
- VerticalVelocity: float
- IsGrounded: bool
- Attack: trigger

## Acceptance Criteria
- Player faces both directions correctly.
- Player keeps the last facing direction while idle.
- AttackOrigin mirrors correctly.
- Attacks work on both sides.
- Animator parameters update without errors.
- Existing gameplay systems still work.
- Console contains no new errors or warnings.

## Risks
- AttackOrigin mirrors incorrectly
- Sprite flips but attack direction does not
- Animator parameter names do not match
- Visual logic changes gameplay behaviour
- Transform scale inversion causes collider problems

## Future Extensions
- Final player sprites
- Final animation clips
- Attack animation events
- Hit reaction
- Death animation
- Enemy animations
