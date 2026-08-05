# Manual Test Checklist

## Preparation
1. Open the Bootstrap scene.
2. Confirm Player and Ground exist.
3. Follow Claude's Unity Editor setup instructions.
4. Place at least three coin objects in the scene.
5. Give each coin a Collider2D with Is Trigger enabled.
6. Save the scene.

## Basic Collection
1. Press Play.
2. Move the Player into the first coin.
3. Confirm the coin disappears.
4. Confirm the score increases by the configured value.
5. Confirm Console contains no error.

## Multiple Coins
1. Collect the second coin.
2. Confirm score increases again.
3. Collect the third coin.
4. Confirm all coins work independently.

## Coin Value
1. Stop Play Mode.
2. Change one coin's value in the Inspector.
3. Press Play.
4. Collect that coin.
5. Confirm score increases by the new value.

## Single Collection
1. Collect a coin.
2. Confirm it cannot increase score more than once.
3. Confirm no duplicate collection occurs during the same trigger contact.

## Regression
1. Move left and right.
2. Jump normally.
3. Test coyote time.
4. Test jump buffer.
5. Confirm Cinemachine still follows the Player.
6. Confirm Console contains no errors or new warnings.
