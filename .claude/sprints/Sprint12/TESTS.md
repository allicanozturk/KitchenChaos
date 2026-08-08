# Sprint12 Tests

## Preparation

1. Open Bootstrap scene.
2. Follow Claude's Unity Editor setup instructions.
3. Create one horizontal MovingPlatform.
4. Create Point A and Point B.
5. Configure Platform Speed = 2.
6. Make sure Player can stand on the platform.
7. Save the scene.

## Horizontal Movement

1. Press Play.
2. Confirm platform moves from A to B.
3. Confirm it reverses at B.
4. Confirm it returns to A.
5. Confirm motion repeats without overshoot.

## Player Carry

1. Stand still on the moving platform.
2. Confirm Player travels with it.
3. Confirm Player does not visibly slide off.
4. Confirm there is no obvious jitter.

## Movement While Riding

1. Stand on the platform.
2. Move left and right.
3. Confirm Player movement still responds normally.
4. Release input.
5. Confirm Player continues being carried.

## Jump From Platform

1. Ride the platform.
2. Jump.
3. Confirm jump behaviour matches normal ground jump.
4. Confirm coyote time and jump buffer remain functional.
5. Land again and confirm Ground detection works.

## Vertical Platform

1. Stop Play Mode.
2. Change Point B so the platform moves vertically.
3. Press Play.
4. Confirm platform moves vertically.
5. Ride it upward and downward.
6. Confirm Player remains stable.
7. Jump from it and confirm normal behaviour.

## Regression

1. Move on normal Ground.
2. Collect a coin.
3. Activate a checkpoint.
4. Take damage and respawn.
5. Defeat an enemy.
6. Confirm Player/Enemy animations work.
7. Confirm camera follow.
8. Confirm Console contains no new errors or warnings.
