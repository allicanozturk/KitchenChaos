using System;
using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Owns where the player reappears and puts it back there on request.
    /// Keeping this out of PlayerHealth lets checkpoints move the respawn point
    /// without touching how damage is handled.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerRespawn : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private Vector2 _spawnPosition;
        private PlayerInputReader _input;
        private PlayerJump _jump;
        private PlayerAttack _attack;
        private PlayerMobility _mobility;
        private PlayerVisual _visual;
        private KitchenChaos.Level.CheckpointWorldReset _worldReset;
        private Animator _animator;
        private bool _suspended;
        private bool _respawning;
        private bool _resumeSimulation;
        private bool _resumeInputBlocked;
        private float _resumeAnimatorSpeed;
        private int _lastRespawnFrame = -1;

        public bool CanInteract => isActiveAndEnabled && !_suspended && Time.frameCount != _lastRespawnFrame;

        /// <summary>Life state resets while controls are locked, before teleport.</summary>
        public event Action Respawning;

        /// <summary>Position and transient state are reset; controls resume after this event.</summary>
        public event Action Respawned;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();
            _jump = GetComponent<PlayerJump>();
            _attack = GetComponent<PlayerAttack>();
            _mobility = GetComponent<PlayerMobility>();
            _visual = GetComponent<PlayerVisual>();
            _worldReset = GetComponent<KitchenChaos.Level.CheckpointWorldReset>();
            _animator = GetComponent<Animator>();

            // The position authored in the scene is the spawn point, so no marker
            // object has to be created and wired up in the Inspector.
            _spawnPosition = _rigidbody.position;
        }

        /// <summary>
        /// Moves the point the player returns to after death. The latest call wins, so
        /// activating a checkpoint simply replaces the previous respawn point.
        /// </summary>
        public bool SetSpawnPosition(Vector2 position, KitchenChaos.Level.Checkpoint checkpoint = null)
        {
            if (!CanInteract)
                return false;

            _spawnPosition = position;
            if (_worldReset != null && _worldReset.isActiveAndEnabled)
                _worldReset.CaptureCheckpoint(checkpoint);
            return true;
        }

        public void SuspendForDeath()
        {
            if (_suspended)
                return;

            _resumeSimulation = _rigidbody.simulated;
            _resumeInputBlocked = _input != null && _input.IsGameplayBlocked;
            _resumeAnimatorSpeed = _animator != null ? _animator.speed : 1f;
            _suspended = true;
            _input?.SetGameplayBlocked(true);
            ResetTransientState(false);
            StopMotion();
            _rigidbody.simulated = false;
            if (_animator != null)
                _animator.speed = 0f;
        }

        public void Respawn()
        {
            if (_respawning)
                return;

            _respawning = true;
            try
            {
                SuspendForDeath();
                _lastRespawnFrame = Time.frameCount;
                Respawning?.Invoke();
                // Listeners clear old projectiles first. Opt-in test scene rebuilds
                // the current and future sections; older scenes retain healing only.
                if (_worldReset != null && _worldReset.isActiveAndEnabled)
                    _worldReset.RestoreAttempt();
                else
                    foreach (var enemy in FindObjectsByType<KitchenChaos.Enemy.EnemyHealth>())
                        if (enemy.gameObject.scene == gameObject.scene)
                            enemy.RestoreForPlayerRespawn();
                StopMotion();
                _rigidbody.position = _spawnPosition;
                // Update child origins immediately as well as the physics body.
                transform.position = new Vector3(_spawnPosition.x, _spawnPosition.y, transform.position.z);
                ResetTransientState(true);
                Respawned?.Invoke();
            }
            finally
            {
                ResumeControls();
                _respawning = false;
            }
        }

        private void ResetTransientState(bool resetAnimation)
        {
            _input?.ResetTransientState();
            _jump?.ResetTransientState();
            _attack?.ResetTransientState();
            _mobility?.ResetTransientState(resetAnimation);
            if (resetAnimation)
                _visual?.ResetTransientState();
        }

        private void StopMotion()
        {
            if (_rigidbody == null)
                return;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }

        public void ResumeControls()
        {
            if (!_suspended)
                return;

            StopMotion();
            if (_rigidbody != null)
                _rigidbody.simulated = _resumeSimulation;
            if (_animator != null)
                _animator.speed = _resumeAnimatorSpeed;
            _input?.SetGameplayBlocked(_resumeInputBlocked);
            _suspended = false;
        }

        private void OnDisable()
        {
            ResumeControls();
        }
    }
}
