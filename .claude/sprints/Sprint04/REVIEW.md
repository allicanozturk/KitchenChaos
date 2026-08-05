# Code Review Checklist

## Scope
- [ ] Only coyote time and jump buffering were added.
- [ ] Variable jump height was not added.
- [ ] Double jump was not added.
- [ ] Wall jump, dash, animation, audio, camera, and combat were not changed.
- [ ] No unnecessary abstractions were introduced.

## Architecture
- [ ] PlayerInputReader still owns input.
- [ ] PlayerMovement still owns horizontal movement only.
- [ ] PlayerJump still owns jump behaviour.
- [ ] Existing ground-check approach is reused.
- [ ] No manager, service, event bus, or unnecessary interface was added.
- [ ] No scene or prefab YAML was manually edited.

## Coyote Time
- [ ] Coyote time is Inspector configurable.
- [ ] Coyote time starts when the player leaves the ground.
- [ ] Jump succeeds inside the configured window.
- [ ] Jump fails after the window expires.
- [ ] Coyote time cannot be reused repeatedly in the same airborne state.

## Jump Buffer
- [ ] Jump buffer time is Inspector configurable.
- [ ] Input is stored only for the configured duration.
- [ ] Buffered jump triggers when valid ground contact occurs.
- [ ] Expired buffered input is cleared.
- [ ] Buffered input does not create a true double jump.

## Unity and Code Quality
- [ ] Input remains frame-safe.
- [ ] Physics changes remain in FixedUpdate().
- [ ] Serialized timing values use Min(0f).
- [ ] Naming is clear.
- [ ] Comments explain why, not what.
- [ ] No per-frame allocations were introduced.
- [ ] No compiler warnings or Console errors were introduced.

## Regression
- [ ] Horizontal movement still works.
- [ ] Normal jumping still works.
- [ ] Ground detection still works.
- [ ] Camera follow still works.
- [ ] Player still lands correctly.
