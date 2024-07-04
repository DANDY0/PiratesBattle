using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.MasterEvent;
using States;
using States.Core;
using Utils;
using Zenject;
using static Utils.Enumerators;


namespace PunNetwork.Services.MatchInfo
{
    public class MatchInfoService : IMatchInfoService, IInitializable, IDisposable
    {
        private readonly IMasterEventService _masterEventService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IPhotonTeamsManager _photonTeamsManager;

        public GameResult GameResult { get; private set; }

        public MatchInfoService
        (
            IMasterEventService masterEventService,
            IGameStateMachine gameStateMachine,
            IPhotonTeamsManager photonTeamsManager
        )
        {
            _masterEventService = masterEventService;
            _gameStateMachine = gameStateMachine;
            _photonTeamsManager = photonTeamsManager;
        }


        public void Initialize()
        {
            _masterEventService.Subscribe(GameEventCodes.StartMatchEventCode, OnStartMatch);
            _masterEventService.Subscribe(GameEventCodes.EndMatchEventCode, OnEndMatch);
        }

        public void Dispose()
        {
            _masterEventService.Unsubscribe(GameEventCodes.StartMatchEventCode, OnStartMatch);
            _masterEventService.Unsubscribe(GameEventCodes.EndMatchEventCode, OnEndMatch);
        }

        private void OnStartMatch(object eventContent)
        {
            _gameStateMachine.Enter<GameplayState>();
        }

        private void OnEndMatch(object eventContent)
        {
            var winningTeam = (byte)eventContent;
            GameResult = winningTeam == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code
                ? GameResult.Win
                : GameResult.Lose;
            _gameStateMachine.Enter<GameResultsState>();
        }
    }
}