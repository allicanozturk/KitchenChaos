using KitchenChaos.Level;
using UnityEngine;

namespace KitchenChaos.Audio
{
    /// <summary>
    /// Plays a one-shot sound when the checkpoint it sits on is activated.
    /// The checkpoint survives its own activation, so it owns the source that plays it.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Checkpoint))]
    public sealed class CheckpointAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip _activationClip;

        private AudioSource _audioSource;
        private Checkpoint _checkpoint;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _checkpoint = GetComponent<Checkpoint>();
        }

        private void OnEnable()
        {
            _checkpoint.Activated += OnActivated;
        }

        private void OnDisable()
        {
            _checkpoint.Activated -= OnActivated;
        }

        private void OnActivated()
        {
            // An unassigned clip is a normal authoring state while sound design is still
            // in progress, so it stays silent instead of throwing or logging every time.
            if (_activationClip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(_activationClip);
        }
    }
}
