# Sprint09 Tests

## Preparation
1. Open Bootstrap scene.
2. Follow Claude's Unity Editor setup instructions.
3. Add PlayerVisual to Player.
4. Assign SpriteRenderer, Animator, Rigidbody2D, and AttackOrigin.
5. Create Animator parameters with exact names:
    - Speed
    - VerticalVelocity
    - IsGrounded
    - Attack
6. Save the scene.

## Facing Test
1. Press Play.
2. Move right.
3. Confirm Player faces right.
4. Move left.
5. Confirm Player faces left.
6. Release input.
7. Confirm Player preserves the last facing direction.

## Attack Direction Test
1. Face right and attack an Enemy on the right.
2. Confirm the attack hits.
3. Face left and attack an Enemy on the left.
4. Confirm the attack hits.
5. Confirm AttackOrigin changes sides correctly.

## Animator Parameter Test
1. Open Animator parameter view during Play Mode.
2. Move horizontally and confirm Speed changes.
3. Jump and confirm VerticalVelocity becomes positive, then negative.
4. Confirm IsGrounded is false in air and true after landing.
5. Attack and confirm the Attack trigger fires.

## Regression
1. Test movement.
2. Test jump, coyote time, and jump buffer.
3. Confirm camera follow.
4. Collect a coin.
5. Take enemy contact damage.
6. Respawn.
7. Defeat an enemy.
8. Confirm Console contains no new errors or warnings.
