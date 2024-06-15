using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace PunNetwork.Views
{
 

    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private Image _fillHealthImage;

        private readonly float _healthChangeDuration = 0.5f; 
        
        public void SetNickName(string value) => _nicknameText.text = value;
            
        public void SetHealthPoints(float currentHp, float maxHp)
        {
            if (maxHp <= 0)
            {
                _fillHealthImage.fillAmount = 0;
                return;
            }

            float targetFillAmount = currentHp / maxHp;
            _fillHealthImage.DOFillAmount(targetFillAmount, _healthChangeDuration); 
        }
    }
}