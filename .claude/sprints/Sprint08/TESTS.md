# Sprint08 Tests

## Preparation
1. Open Bootstrap.
2. Add/configure Player attack.
3. Add/configure EnemyHealth.
4. Create Enemy layer if needed.
5. Assign Enemy layer and attack LayerMask.
6. Save scene.

## Attack
1. Press attack near Enemy.
2. Confirm health decreases.
3. Move out of range and attack.
4. Confirm no damage.

## Cooldown
1. Spam attack.
2. Confirm damage respects cooldown.

## Death
1. Enemy Max Health = 3.
2. Player Attack Damage = 1.
3. Hit three valid times.
4. Confirm Enemy disappears.
5. Confirm it no longer patrols or damages.

## Safety
1. If Enemy has multiple colliders, one attack should deal damage once.
2. Remove Enemy layer from LayerMask and confirm no damage.
3. Restore layer.

## Regression
1. Movement works.
2. Jump and jump assists work.
3. Camera follows.
4. Coin collection works.
5. Player health and respawn work.
6. Living Enemy contact damage works.
7. Console clean.
