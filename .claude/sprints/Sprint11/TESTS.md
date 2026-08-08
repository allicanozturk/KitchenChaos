# Sprint11 Tests

## Preparation
1. Open Bootstrap.
2. Create two checkpoint GameObjects.
3. Add Collider2D with Is Trigger enabled.
4. Place checkpoints at different positions.
5. Save scene.

## Original Spawn
1. Play.
2. Do not touch a checkpoint.
3. Die.
4. Confirm respawn at original start.

## Checkpoint A
1. Touch A.
2. Move away.
3. Die.
4. Confirm respawn at A.
5. Confirm full health and zero velocity.

## Checkpoint B
1. Touch B.
2. Move away.
3. Die.
4. Confirm respawn at B, not A.

## Re-entry
1. Touch B repeatedly.
2. Die.
3. Confirm stable respawn.

## Regression
1. Movement and jump work.
2. Coyote time/jump buffer work.
3. Camera follows.
4. Coin collection works.
5. Enemy contact damage works.
6. Enemy can be defeated.
7. Animations work.
8. Console clean.
