using System.Collections.Generic;
using KitchenChaos.Enemy;
using TMPro;
using UnityEngine;

namespace KitchenChaos.Level
{
    /// <summary>Keep this controller, its enemies and barrier in ONE world-reset root.</summary>
    [DisallowMultipleComponent]
    public sealed class EncounterGate : MonoBehaviour
    {
        [SerializeField] private EnemyHealth[] _enemies;
        [SerializeField] private GameObject _barrier;
        [SerializeField] private TMP_Text _label;
        private readonly HashSet<EnemyHealth> _remaining = new();
        private bool _ready;

        private void Start()
        {
            // Start runs after every enemy's Awake has initialized health.
            if (_barrier == null || _enemies == null || _enemies.Length == 0)
            {
                Debug.LogError("EncounterGate needs a barrier and authored enemies.", this);
                enabled = false;
                return;
            }
            foreach (var enemy in _enemies)
            {
                if (enemy == null)
                {
                    Debug.LogError("EncounterGate has a missing enemy reference; kept closed.", this);
                    enabled = false;
                    return;
                }
                if (enemy.CurrentHealth > 0 && _remaining.Add(enemy)) enemy.Died += OnEnemyDied;
            }
            _ready = true;
            Refresh();
        }

        private void OnEnemyDied()
        {
            _remaining.RemoveWhere(enemy => enemy == null || enemy.CurrentHealth <= 0);
            Refresh();
        }

        private void Refresh()
        {
            if (!_ready) return;
            bool open = _remaining.Count == 0;
            _barrier.SetActive(!open);
            if (_label != null)
            {
                _label.text = open ? "GECIT ACILDI" : "BARIKAT\n<size=65%>Kalan muhafiz: " + _remaining.Count + "</size>";
                _label.color = open ? new Color(.55f, 1f, .65f) : new Color(1f, .8f, .35f);
            }
        }

        private void OnDestroy()
        {
            if (_enemies == null) return;
            foreach (var enemy in _enemies) if (enemy != null) enemy.Died -= OnEnemyDied;
        }
    }
}
