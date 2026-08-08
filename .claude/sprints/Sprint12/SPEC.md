# Sprint 12 - Moving Platforms

## Business Goal

Add moving platforms so the first playable level can include timing-based traversal and vertical/horizontal platforming challenges.

## Technical Goal

Implement a reusable moving platform component that travels between two authored points and carries the Player smoothly while standing on it.

## Scope

- Moving platform GameObject
- Horizontal or vertical movement between two points
- Inspector-configurable movement speed
- Reusable patrol-point style setup
- Player is carried with the platform while standing on it
- Existing movement, jump, camera, health, checkpoint, enemy, combat, collectibles, and animation systems remain unchanged

## Out of Scope

- Rotating platforms
- Falling platforms
- One-way platforms
- Crumbling platforms
- Platform activation switches
- Complex spline movement
- Platform acceleration curves
- Conveyor belts
- Moving enemies on platforms
- Final environment art

## User Stories

- As a player, I can stand on a moving platform without sliding off due to the platform's motion.
- As a player, I can jump from a moving platform and continue normal movement.
- As a level designer, I can configure a platform path using two points.

## Functional Requirements

- Platform moves continuously between Point A and Point B.
- Platform reverses direction when reaching either endpoint.
- Platform speed is configurable in the Inspector.
- Platform path can be horizontal, vertical, or diagonal.
- Player remains stable while standing on the moving platform.
- Player can jump off normally.
- Player movement input still works while on the platform.
- Platform movement does not change Player gravity, jump force, or movement code unnecessarily.

## Technical Requirements

- Unity 6 LTS
- Use Rigidbody2D
- Platform movement occurs in FixedUpdate
- Use Rigidbody2D.MovePosition or an equivalent physics-safe approach
- Reuse two Transform endpoints
- Do not use Transform.Translate for physics movement
- Do not use Singleton
- Do not create GameManager
- Do not use GameObject.Find or FindObjectOfType
- Do not manually edit scene or prefab YAML
- Keep responsibilities focused
- Unity Editor scene setup is performed manually

## Suggested Defaults

- Platform Speed: 2

## Acceptance Criteria

- Platform moves from A to B and back continuously.
- Platform does not overshoot endpoints.
- Player is carried smoothly while standing on it.
- Player does not jitter visibly on the platform.
- Player can move left/right while riding the platform.
- Player can jump from the platform.
- Platform works horizontally and vertically.
- Existing gameplay systems still work.
- Console contains no new errors or warnings.

## Risks

- Player slides off because only the platform moves
- Parenting Player causes scale/physics side effects
- Platform motion jitters because transform and physics updates are mixed
- Ground detection fails on the moving platform
- Player velocity is incorrectly overwritten
- Vertical platforms interfere with jump behaviour

## Future Extensions

- Falling platforms
- One-way platforms
- Platform switches
- Spline platforms
- Crumbling platforms
- Conveyors
