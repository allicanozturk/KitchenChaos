# Code and Setup Review Checklist

## Scope
- [ ] Sprint03 scope was respected.
- [ ] No camera shake was added.
- [ ] No look-ahead was added.
- [ ] No camera bounds or confiner was added.
- [ ] No unrelated gameplay system was changed.

## Architecture
- [ ] PlayerMovement remains responsible only for horizontal movement.
- [ ] PlayerJump remains responsible only for jumping.
- [ ] Camera responsibility is not placed on Player scripts.
- [ ] No unnecessary custom camera script was created.
- [ ] No scene or prefab YAML was manually edited.

## Unity Setup
- [ ] Cinemachine camera exists in the scene.
- [ ] Player is assigned as the follow target.
- [ ] Only the intended Main Camera renders the Game view.
- [ ] Camera damping values are sensible.
- [ ] Scene changes are saved.

## Regression
- [ ] Horizontal movement still works.
- [ ] Jumping still works.
- [ ] No double jump was introduced.
- [ ] Ground detection still works.
- [ ] Console contains no new errors or warnings.
