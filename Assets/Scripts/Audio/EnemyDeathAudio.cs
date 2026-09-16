using KitchenChaos.Enemy;
using UnityEngine;

namespace KitchenChaos.Audio
{
    /// <summary>
    /// Plays a one-shot sound when the enemy it sits on dies.
    /// The enemy is deactivated and destroyed as it dies, which would cut off a source of
    /// its own, so the clip is played through a source that outlives it. That source is
    /// assigned in the Inspector rather than looked up, so no manager or global is needed.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyDeathAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _deathClip;

        private EnemyHealth _health;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();

            if (_audioSource == null)
            {
                // A missing source is a wiring mistake rather than pending sound design,
                // so it fails once and loudly instead of going quietly unheard.
                Debug.LogError($"{nameof(EnemyDeathAudio)} needs an Audio Source assigned to an object that outlives this enemy.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _health.Died += OnDied;
        }

        private void OnDisable()
        {
            _health.Died -= OnDied;
        }

        private void OnDied()
        {
            // An unassigned clip is a normal authoring state while sound design is still
            // in progress, so it stays silent instead of throwing or logging every time.
            if (_deathClip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(_deathClip);
        }
    }
}
