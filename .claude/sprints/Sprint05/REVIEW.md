# Code Review Checklist

## Scope
- [ ] Coin collection was implemented.
- [ ] Score tracking was implemented.
- [ ] Coin disappears after collection.
- [ ] UI was not added.
- [ ] Animation, audio, save, inventory, pooling, and particles were not added.
- [ ] Existing Player and camera systems were not changed.

## Architecture
- [ ] No Singleton was introduced.
- [ ] No GameManager was introduced.
- [ ] No GameObject.Find was used.
- [ ] No FindObjectOfType was used.
- [ ] No unnecessary interface, service, manager, or event bus was added.
- [ ] Components have focused responsibilities.
- [ ] No scene or prefab YAML was manually edited.

## Collectible Behaviour
- [ ] Coin uses trigger-based collection.
- [ ] Coin value is Inspector configurable.
- [ ] Coin identifies the Player reliably.
- [ ] Coin can only be collected once.
- [ ] Score increases by the configured value.
- [ ] Multiple coins work independently.

## Code Quality
- [ ] Serialized coin value uses Min(1).
- [ ] Naming is clear.
- [ ] Comments explain why, not what.
- [ ] No per-frame allocations were introduced.
- [ ] No compiler warnings or Console errors were introduced.

## Regression
- [ ] Horizontal movement still works.
- [ ] Jumping still works.
- [ ] Coyote time still works.
- [ ] Jump buffer still works.
- [ ] Camera follow still works.
