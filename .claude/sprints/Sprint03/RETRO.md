## What went well

Cinemachine camera follow was configured without custom code.
The camera follows horizontal movement and jumping correctly.

## Problems encountered

The Unity 6 Cinemachine interface differs from older tutorials.
Tracking Target and Position Control replaced the older Follow workflow.

## Lessons learned

Cinemachine Camera uses Tracking Target in the current package version.
The Follow position controller provides configurable damping.

## Camera values selected

- X Damping: 0.2
- Y Damping: 0.3
- Z Damping: 0
- Follow Offset: (0, 0, -10)