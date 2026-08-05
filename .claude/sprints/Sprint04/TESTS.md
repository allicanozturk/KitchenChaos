# Manual Test Checklist

## Preparation
1. Open the Bootstrap scene.
2. Confirm the Player, Ground, GroundCheck, and Cinemachine Camera are present.
3. Select Player and confirm the new timing fields are visible.
4. Use the starting values:
    - Coyote Time: 0.12
    - Jump Buffer Time: 0.15
5. Save the scene.

## Normal Jump Regression
1. Press Play.
2. Click the Game view.
3. Press Space while grounded.
4. Confirm the player jumps normally.
5. Confirm the player lands normally.
6. Confirm no airborne double jump is possible.

## Coyote Time Test
1. Move toward the edge of the Ground platform.
2. Walk off without jumping.
3. Press Space immediately after leaving the edge.
4. Confirm the player jumps.
5. Repeat, but wait noticeably longer before pressing Space.
6. Confirm the jump no longer occurs after the coyote-time window.

## Jump Buffer Test
1. Jump into the air.
2. Shortly before landing, press Space.
3. Confirm the player jumps again immediately after touching the ground.
4. Repeat, but press Space much earlier while still high in the air.
5. Confirm the expired input does not trigger a jump after landing.

## Movement and Camera Regression
1. Move left and right.
2. Jump while moving.
3. Confirm horizontal air movement still works.
4. Confirm Cinemachine continues following the player.
5. Confirm no visible camera regression.

## Stress Test
1. Repeatedly press Space while airborne.
2. Confirm no true double jump occurs.
3. Walk off edges and press Space at different delays.
4. Press Space at different times before landing.
5. Confirm behaviour remains predictable.

## Inspector Tuning Test
1. Set Coyote Time to 0.
2. Confirm edge forgiveness disappears.
3. Restore Coyote Time to 0.12.
4. Set Jump Buffer Time to 0.
5. Confirm pre-landing input is no longer stored.
6. Restore Jump Buffer Time to 0.15.

## Console Check
1. Open the Console.
2. Confirm there are no errors.
3. Confirm there are no new warnings.
