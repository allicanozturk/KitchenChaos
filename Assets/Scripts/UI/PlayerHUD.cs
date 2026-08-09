using KitchenChaos.Player;
using TMPro;
using UnityEngine;

namespace KitchenChaos.UI
{
    /// <summary>
    /// Shows the player's health and score on screen.
    /// Presentation only: it reads the values their owners already publish and never
    /// writes to them, so damage, respawn and scoring rules stay exactly where they are.
    /// The player is wired in through the Inspector rather than looked up, so a scene
    /// with two players would simply get two of these.
    /// </summary>
    public sealed class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerScore _score;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _scoreText;

        // What the labels currently read. Starting off any reachable value forces the
        // first frame to write, and comparing against it afterwards keeps the string
        // building to the frames where a value actually changed.
        private int _shownHealth = -1;
        private int _shownScore = -1;

        private void Awake()
        {
            if (_health == null || _score == null || _healthText == null || _scoreText == null)
            {
                // Fail once and loudly instead of throwing a NullReferenceException on
                // every frame.
                Debug.LogError($"{nameof(PlayerHUD)} needs health, score and both labels assigned.", this);
                enabled = false;
            }
        }

        // Reading in LateUpdate shows the state left by everything that ran this frame,
        // so a hit or a pickup is never displayed one frame late. Polling two ints is
        // cheaper to follow than an event contract, and it keeps health and score free
        // of any knowledge that a HUD exists.
        private void LateUpdate()
        {
            RefreshHealth();
            RefreshScore();
        }

        private void RefreshHealth()
        {
            int current = _health.CurrentHealth;
            if (current == _shownHealth)
            {
                return;
            }

            _shownHealth = current;
            _healthText.text = $"Health: {current} / {_health.MaxHealth}";
        }

        private void RefreshScore()
        {
            int current = _score.Current;
            if (current == _shownScore)
            {
                return;
            }

            _shownScore = current;
            _scoreText.text = $"Coins: {current}";
        }
    }
}
