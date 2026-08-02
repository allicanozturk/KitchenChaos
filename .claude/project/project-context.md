# Project Context

## Project

**Name:** Kitchen Chaos

## Vision

Create a modern commercial-quality 2D action platformer inspired by classic platform games while following clean architecture, maintainable code and professional Unity development practices.

---

# Goals

- Release the game on Steam.
- Learn Unity professionally.
- Maintain clean, scalable architecture.
- Build reusable gameplay systems.
- Use an AI-first development workflow.

---

# Current Development Stage

Project Bootstrap

The Unity project has been initialized and the core development workflow has been established.

No gameplay systems have been implemented yet.

---

# Current Sprint

Sprint 01 – Player Movement Foundation

---

# Repository Status

## Scenes

- Bootstrap
- MainMenu
- TestArena

## Folder Structure

```
Assets/
├── Art/
├── Audio/
├── Materials/
├── Prefabs/
├── Scenes/
├── Scripts/
│   ├── Core/
│   ├── Managers/
│   ├── Player/
│   └── UI/
└── Settings/
```

---

# Installed Unity Packages

- Input System
- Cinemachine
- Localization
- Addressables

---

# Current Gameplay State

Implemented:

- Horizontal player movement (`PlayerMovement`, Rigidbody2D + New Input System)
- Generated Input System wrapper (`InputSystem_Actions`)

Pending Editor setup (scene work, owned by the developer):

- Ground GameObject
- Player GameObject
- Rigidbody2D
- CapsuleCollider2D
- SpriteRenderer

Not Implemented:

- Jump
- Camera Follow
- Animations
- Combat
- Health System
- UI

---

# AI Workflow

Claude Code is responsible for:

- Code generation
- Refactoring
- Architecture suggestions
- Documentation updates
- Code review assistance

The developer is responsible for:

- Gameplay decisions
- Unity scene editing
- Asset management
- Play testing
- Final approval

---

# Coding Status

Current Branch

main

Current Milestone

Milestone 1

Current Sprint

Sprint01

---

# Important Constraints

- Unity 6 LTS (6000.5.6f1)
- URP 2D
- C#
- New Input System
- Rigidbody2D Physics
- Follow all rules defined inside `.claude/CLAUDE.md`
- Never implement features outside the current sprint scope.

---

# Definition of Done

A sprint is complete only if:

- Code compiles successfully.
- No Console errors.
- Manual tests pass.
- Documentation is updated.
- Sprint review is completed.
- Changes are committed to Git.