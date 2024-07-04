using Controllers;
using Photon.Pun;
using States.Core;
using Utils;
using Utils.Extensions;


namespace States
{
    public class MatchPreviewState : IState
    {
        private readonly PreviewMatchAnimationController _previewMatchAnimationController;
        
        public MatchPreviewState
        (
            PreviewMatchAnimationController previewMatchAnimationController
        )
        {
            _previewMatchAnimationController = previewMatchAnimationController;
        }

        public void Enter()
        {
            _previewMatchAnimationController.Start();
        }

        public void Exit()
        {
            _previewMatchAnimationController.Hide();
        }
    }
}