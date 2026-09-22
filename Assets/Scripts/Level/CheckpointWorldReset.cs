using System;
using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Level
{
    /// <summary>Authored checkpoint sections, pristine inactive templates, no scene reload.</summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(-2000)]
    public sealed class CheckpointWorldReset : MonoBehaviour
    {
        [Serializable]
        private sealed class Entry
        {
            public GameObject Root;
            public int Section;
            [NonSerialized] public GameObject Template;
            [NonSerialized] public GameObject Current;
            [NonSerialized] public Transform Parent;
            [NonSerialized] public Vector3 Position, Scale;
            [NonSerialized] public Quaternion Rotation;
            [NonSerialized] public bool Active;
        }
        [SerializeField] private Checkpoint[] _checkpoints;
        [SerializeField] private Entry[] _entries;
        private GameObject _templateBank;
        private PlayerScore _score;
        private int _section, _savedScore;
        private bool _ready;

        private void Awake()
        {
            // Before any gameplay Awake mutates poses or caches runtime state.
            _score = GetComponent<PlayerScore>();
            _savedScore = _score != null ? _score.Current : 0;
            if (_entries == null || _checkpoints == null) { enabled = false; return; }
            _templateBank = new GameObject("Checkpoint Pristine Templates");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(_templateBank, gameObject.scene);
            _templateBank.SetActive(false);
            foreach (var entry in _entries)
            {
                if (entry.Root == null) continue;
                var t = entry.Root.transform;
                entry.Current = entry.Root; entry.Parent = t.parent;
                entry.Position = t.localPosition; entry.Rotation = t.localRotation; entry.Scale = t.localScale;
                entry.Active = entry.Root.activeSelf;
                // Inactive parent prevents Awake/OnEnable, projectiles, sounds, etc.
                entry.Template = Instantiate(entry.Root, _templateBank.transform, false);
                entry.Template.name = entry.Root.name;
            }
            _ready = true;
        }

        public void CaptureCheckpoint(Checkpoint checkpoint)
        {
            if (!_ready || checkpoint == null) return;
            int index = Array.IndexOf(_checkpoints, checkpoint);
            if (index < 0 || _section == index + 1) return;
            _section = index + 1;
            _savedScore = _score != null ? _score.Current : 0;
        }

        public void RestoreAttempt()
        {
            if (!_ready) return;
            // Detached death art belongs to the failed attempt, not the templates.
            foreach (var effect in FindObjectsByType<KitchenChaos.Enemy.TomatoPopPlayback>())
                if (effect.gameObject.scene == gameObject.scene) { effect.gameObject.SetActive(false); Destroy(effect.gameObject); }
            foreach (var effect in FindObjectsByType<KitchenChaos.Enemy.PeaPodPlayback>())
                if (effect.gameObject.scene == gameObject.scene) { effect.gameObject.SetActive(false); Destroy(effect.gameObject); }
            foreach (var entry in _entries)
            {
                if (entry.Section < _section || entry.Template == null) continue;
                if (entry.Current != null)
                {
                    // Disable now so delayed Destroy cannot leave duplicate colliders.
                    entry.Current.SetActive(false);
                    Destroy(entry.Current);
                }
                var replacement = Instantiate(entry.Template, _templateBank.transform, false);
                replacement.SetActive(false);
                replacement.name = entry.Template.name;
                var t = replacement.transform;
                t.SetParent(entry.Parent, false);
                t.localPosition = entry.Position; t.localRotation = entry.Rotation; t.localScale = entry.Scale;
                entry.Current = replacement;
                replacement.SetActive(entry.Active);
            }
            _score?.RestoreCheckpointValue(_savedScore);
            Physics2D.SyncTransforms();
        }

        private void OnDestroy()
        {
            if (_templateBank != null) Destroy(_templateBank);
        }
    }
}
