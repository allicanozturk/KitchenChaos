# Sprint 02 - Jump Foundation

## Business Goal

Allow the player to jump in a responsive and reliable way.

## Technical Goal

Implement a reusable jump system using Rigidbody2D while keeping
movement and jump responsibilities separated.

## Scope

-   Jump input
-   Ground detection
-   Single jump only
-   Configurable jump force
-   Clean architecture

## Out of Scope

-   Double Jump
-   Coyote Time
-   Jump Buffer
-   Variable Jump Height
-   Wall Jump
-   Dash
-   Animation
-   Audio

## User Story

As a player, I can jump over obstacles and land safely.

## Functional Requirements

-   Space triggers a jump.
-   Player can only jump while grounded.
-   Jump force is configurable from the Inspector.
-   Existing movement continues to work.

## Technical Requirements

-   Unity 6 LTS
-   Rigidbody2D
-   New Input System
-   Physics in FixedUpdate()
-   Cache references in Awake()
-   No direct Transform movement

## Acceptance Criteria

-   Player jumps correctly.
-   No double jump.
-   Existing movement is not broken.
-   No compiler warnings or errors.

## Risks

-   Incorrect ground detection.
-   Mixing Update and FixedUpdate.
-   Applying jump force multiple times.

## Future Extensions

Double Jump, Coyote Time, Jump Buffer, Variable Jump Height.
