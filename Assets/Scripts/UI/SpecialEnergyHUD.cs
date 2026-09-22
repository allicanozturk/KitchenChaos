using KitchenChaos.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos.UI
{
    public sealed class SpecialEnergyHUD : MonoBehaviour
    {
        [SerializeField] private PlayerSpinAttack _special;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _fill;
        private int _shown = -1;
        private void LateUpdate()
        {
            if (_special == null || _label == null || _fill == null || _shown == _special.Energy) return;
            _shown = _special.Energy;
            bool ready = _shown >= _special.Capacity;
            _label.text = ready ? "DONUS HAZIR  [Y / Q]" : $"OZEL ENERJI  {_shown}/{_special.Capacity}  [Y / Q]";
            _fill.fillAmount = (float)_shown / Mathf.Max(1, _special.Capacity);
            _fill.color = ready ? new Color(1f, .8f, .2f) : new Color(.2f, .8f, .7f);
        }
    }
}
