# Sprint 01 - Player Movement Foundation

## Business Goal

Create the first playable interaction by allowing the player to move
left and right.

## Technical Goal

Implement a clean, extensible movement component using Rigidbody2D and
the Unity New Input System.

## Scope

-   Horizontal movement
-   Rigidbody2D movement
-   Configurable movement speed
-   Clean architecture

## Out of Scope

-   Jump
-   Dash
-   Combat
-   Animation
-   Sound

## User Story

As a player, I can move left and right using the keyboard.

## Functional Requirements

-   A/D and Left/Right Arrow move the player.
-   Movement is smooth.
-   Speed is configurable in the Inspector.

## Technical Requirements

-   Unity 6 LTS
-   Rigidbody2D
-   New Input System
-   Cache components in Awake()
-   Physics in FixedUpdate()

## Acceptance Criteria

-   Player moves correctly.
-   No compiler warnings or errors.
-   No unnecessary allocations.

## Risks

-   Mixing Update and FixedUpdate.
-   Direct Transform movement.

## Future Extensions

Jump, sprint, dash, knockback, slopes.
