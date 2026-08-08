# Sprint10 Tests

## Preparation

1. Open the Bootstrap scene.
2. Import the temporary Player sprite sheet.
3. Import the temporary Enemy sprite sheet.
4. Apply the approved sprite import settings.
5. Create or assign Player animation clips.
6. Configure Animator states and transitions.
7. Assign Enemy visuals.
8. Adjust colliders.
9. Save the scene.

## Player Visual Test

1. Press Play.
2. Stand still.
3. Confirm Idle animation plays.
4. Move left and right.
5. Confirm Run animation plays.
6. Confirm the Player faces both directions correctly.
7. Confirm there is no visible sliding caused by a bad pivot.

## Jump and Fall Test

1. Jump.
2. Confirm Jump animation plays while rising.
3. Confirm Fall animation plays while descending.
4. Confirm Idle or Run resumes after landing.

## Attack Test

1. Attack while facing right.
2. Confirm Attack animation plays.
3. Attack while facing left.
4. Confirm Attack animation plays and the hit direction is correct.
5. Confirm cooldown behaviour remains unchanged.

## Collider Test

1. Stand on Ground.
2. Confirm the visible feet align with the platform.
3. Move near platform edges.
4. Confirm the collider does not visibly float or sink.
5. Touch Enemy.
6. Confirm contact damage still works.
7. Attack Enemy.
8. Confirm the hit registers at the expected visual range.

## Enemy Visual Test

1. Confirm Enemy is visually different from Player.
2. Confirm Enemy patrol animation or static visual moves with the patrol.
3. Confirm Enemy disappears after health reaches zero.

## Regression

1. Test movement.
2. Test jump, coyote time, and jump buffer.
3. Confirm camera follow.
4. Collect a coin.
5. Lose health and respawn.
6. Defeat an Enemy.
7. Confirm Console contains no new errors or warnings.
