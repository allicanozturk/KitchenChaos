using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Audio
{
    /// <summary>
    /// Turns the player's own gameplay signals into one-shot sound effects.
    /// Presentation only, in the same spirit as <see cref="PlayerVisual"/>: it listens to
    /// what the player systems already announce and never calls back into them, so
    /// movement, combat, damage and scoring behave identically without this component.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PlayerJump))]
    [RequireComponent(typeof(PlayerAttack))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerScore))]
    public sealed class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip _jumpClip;
        [SerializeField] private AudioClip _attackClip;
        [SerializeField] private AudioClip _damageClip;
        [SerializeField] private AudioClip _coinPickupClip;

        private AudioSource _audioSource;
        private PlayerJump _jump;
        private PlayerAttack _attack;
        private PlayerHealth _health;
        private PlayerScore _score;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _jump = GetComponent<PlayerJump>();
            _attack = GetComponent<PlayerAttack>();
            _health = GetComponent<PlayerHealth>();
            _score = GetComponent<PlayerScore>();
        }

        private void OnEnable()
        {
            _jump.Jumped += OnJumped;
            _attack.Attacked += OnAttacked;
            _health.Damaged += OnDamaged;
            _score.ScoreAdded += OnScoreAdded;
        }

        private void OnDisable()
        {
            _jump.Jumped -= OnJumped;
            _attack.Attacked -= OnAttacked;
            _health.Damaged -= OnDamaged;
            _score.ScoreAdded -= OnScoreAdded;
        }

        private void OnJumped()
        {
            Play(_jumpClip);
        }

        private void OnAttacked()
        {
            Play(_attackClip);
        }

        private void OnDamaged()
        {
            Play(_damageClip);
        }

        // Coins are the only thing that scores, and one is destroyed the moment it is
        // touched, so the pickup is heard from the player that collected it.
        private void OnScoreAdded(int amount)
        {
            Play(_coinPickupClip);
        }

        private void Play(AudioClip clip)
        {
            // An unassigned clip is a normal authoring state while sound design is still
            // in progress, so it stays silent instead of throwing or logging every time.
            if (clip == null)
            {
                return;
            }

            // PlayOneShot rather than Play, so overlapping events layer instead of
            // cutting each other off on the single shared source.
            _audioSource.PlayOneShot(clip);
        }
    }
}
