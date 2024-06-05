using System;
using Core.Abstracts;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using States;
using States.Core;
using Utils;
using Views;
using Zenject;

namespace Controllers
{
    public class PreviewMatchAnimationController : Controller<PreviewMatchAnimationView>
    {
        public void Start()
        {
            View.Reset();
            View.Show();
            
            View.PlayAnimation()
                .AppendInterval(.1f)
                .OnComplete(() =>
                {
                    if (!PhotonNetwork.IsMasterClient) return;
                    var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    var sendOptions = new SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(GameEventCodes.StartMatchEventCode, null, raiseEventOptions, sendOptions);
                });
        }

        public void Hide()
        {
            View.Hide();
        }
    }
}