# Sprint13 Tests

## Preparation
1. Open Bootstrap.
2. Create SpikeHazard with trigger Collider2D.
3. Set Damage = 1, Instant Kill = false.
4. Create LavaHazard/DeathZone with trigger Collider2D.
5. Set Instant Kill = true.
6. Save scene.

## Spike Damage
1. Touch spikes once.
2. Confirm health decreases by 1.
3. Move away and re-enter.
4. Confirm damage applies again according to the chosen contact rule.

## Spike Death
1. Keep re-entering until death.
2. Confirm respawn at latest checkpoint.
3. Confirm health restores.

## Instant Kill
1. Enter lava/death zone at full health.
2. Confirm immediate death via PlayerHealth.
3. Confirm respawn at latest checkpoint.

## Re-entry
1. Return after respawn.
2. Confirm hazard works again.

## Regression
1. Movement/jump/coyote/buffer.
2. Camera.
3. Coin.
4. Checkpoint.
5. Enemy contact damage.
6. Enemy defeat.
7. Horizontal/vertical moving platforms.
8. Animations.
9. Console clean.
