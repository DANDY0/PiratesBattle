using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PunNetwork.Views.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private Image _fillHealthImage;

        private const float HealthChangeDuration = .5f;

        public void SetNickName(string value) => _nicknameText.text = value;

        public void SetHealthPoints(float currentHp, float maxHp)
        {
            if (maxHp <= 0)
            {
                _fillHealthImage.fillAmount = 0;
                return;
            }

            var targetFillAmount = currentHp / maxHp;
            _fillHealthImage.DOFillAmount(targetFillAmount, HealthChangeDuration);
        }
    }
}