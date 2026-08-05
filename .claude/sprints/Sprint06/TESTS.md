# Sprint06 Tests

## Preparation

1. Open Bootstrap scene.
2. Select Player.
3. Add PlayerHealth component.
4. Set Max Health = 3.
5. Save scene.

## Damage Test

1. Call TakeDamage(1).
2. Health becomes 2.

3. Call TakeDamage(1).
4. Health becomes 1.

5. Call TakeDamage(1).
6. Player dies.

## Respawn Test

1. Player returns to spawn.
2. Health restored.
3. Player can move.
4. Player can jump.
5. Camera still follows.

## Regression

1. Collect coins.
2. Jump.
3. Test coyote time.
4. Test jump buffer.
5. Console has no errors.