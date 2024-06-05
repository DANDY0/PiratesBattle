using Core.Abstracts;
using DG.Tweening;
using Enums;
using TMPro;
using UnityEngine;

namespace Views
{
    public class MatchResultsView : Window
    {
        [SerializeField] private TMP_Text _text;
        public override EWindow Name => EWindow.MatchResults;

        private void Awake()
        {
            Reset();
        }

        public Sequence PlayAnimation() =>
            DOTween.Sequence()
                .Append(_text.DOFade(1, .5f))
                .AppendInterval(2)
                .Append(_text.DOFade(0, .5f));

        public void Reset()
        {
            _text.alpha = 0;
        }
    }
}