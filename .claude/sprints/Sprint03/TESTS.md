# Manual Test Checklist

## Preparation
1. Open the Bootstrap scene.
2. Confirm Player and Ground exist.
3. Confirm Cinemachine is installed.
4. Save the scene.

## Horizontal Follow
1. Press Play.
2. Click the Game view.
3. Move right.
4. Confirm the camera follows.
5. Move left.
6. Confirm the camera follows.

## Vertical Follow
1. Press Space.
2. Confirm the camera follows upward.
3. Confirm it follows while falling.
4. Confirm it settles after landing.

## Quality Checks
1. Move continuously left and right.
2. Jump while moving.
3. Confirm no visible jitter.
4. Confirm the camera is not excessively delayed.
5. Confirm there is no harsh snapping.

## Regression Checks
1. Player still moves.
2. Player still jumps.
3. No airborne second jump.
4. Player lands correctly.
5. Console has no errors or warnings.

## Inspector Tuning
1. Change damping slightly.
2. Test again.
3. Confirm response changes.
4. Restore approved values before committing.
