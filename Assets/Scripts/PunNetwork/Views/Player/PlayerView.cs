using Photon.Pun;
using PunNetwork.Services.PlayersStats;
using PunNetwork.Services.RoomPlayer;
using Services.Input;
using Services.PhotonPool;
using UnityEngine;
using Zenject;
using static Utils.Enumerators;

namespace PunNetwork.Views.Player
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerView : MonoBehaviour, IPunInstantiateMagicCallback
    {
        private const float MaxHealthPoints = 100;

        public Photon.Realtime.Player Player { get; private set; }
        public TeamRole TeamRole { get; private set; }
        public PhotonView PhotonView { get; private set; }
        public EnemiesTriggerCollider EnemiesTriggerCollider => _enemiesTriggerCollider;
        public PlayerUI PlayerUI => _playerUI;
        public float CurrentHealthPoints { get; private set; }

        [SerializeField] private MeshRenderer _teamMarker;
        [SerializeField] private ParticleSystem _destruction;
        [SerializeField] private PlayerUI _playerUI;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private EnemiesTriggerCollider _enemiesTriggerCollider;

        private IInputService _inputService;
        private IPhotonPoolService _photonPoolService;
        private IRoomPlayersService _roomPlayersService;
        private IPlayersStatsService _playersStatsService;

        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerShooting PlayerShooting { get; private set; }
        public PlayerEffects PlayerEffects { get; private set; }

        [Inject]
        private void Construct(
            IInputService inputService,
            IPhotonPoolService photonPoolService,
            IRoomPlayersService roomPlayersService,
            IPlayersStatsService playersStatsService)
        {
            _inputService = inputService;
            _photonPoolService = photonPoolService;
            _roomPlayersService = roomPlayersService;
            _playersStatsService = playersStatsService;

            PlayerMovement = new PlayerMovement(this, _inputService, _playerAnimator);
            PlayerShooting = new PlayerShooting(this, _inputService, _photonPoolService, _enemiesTriggerCollider, _playerAnimator);
            PlayerEffects = new PlayerEffects(this, _destruction, _collider);
        }

        public void SubscribeOnInput()
        {
            if (PhotonView.IsMine)
                _inputService.FireTriggeredEvent += PlayerShooting.FireJoystickTriggered;
        }

        public void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            PlayerMovement.Initialize();
            PlayerShooting.Initialize();
            
            _playerUI.SetHealthPoints(CurrentHealthPoints, MaxHealthPoints);
        }

        private void OnDestroy()
        {
            if (PhotonView.IsMine)
                _inputService.FireTriggeredEvent -= PlayerShooting.FireJoystickTriggered;
        }

        private void Update()
        {
            if (!PlayerMovement.CanControl())
                return;

            PlayerMovement.HandleMovement();
            PlayerShooting.HandleShooting();
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Player = info.Sender;
            _roomPlayersService.SendLocalPlayersSpawned(info.Sender, this);
            SetupView();
        }
        
        [PunRPC]
        public void RegisterHit(float damage)
        {
            if (!PhotonView.IsMine)
                return;

            var newHealthPoints = CurrentHealthPoints - damage;
            var resultHealthPoints = newHealthPoints <= 0 ? 0 : newHealthPoints;

            _playersStatsService.SendPlayerHp(resultHealthPoints);

            if (resultHealthPoints == 0)
                PhotonView.RPC(nameof(DestroyPlayer), RpcTarget.All);
        }

        [PunRPC]
        public void DestroyPlayer()
        {
            PlayerUI.gameObject.SetActive(false);
            _collider.enabled = false;
            GetComponent<CharacterController>().enabled = false;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;

            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
                renderer.enabled = false;

            _destruction.Play();
        }

        public void UpdateHealthPoints(float healthPoints)
        {
            CurrentHealthPoints = healthPoints;
            _playerUI.SetHealthPoints(healthPoints, MaxHealthPoints);
        }
        
        public void SetTeamRole(TeamRole role)
        {
            TeamRole = role;
            var markerColor = TeamMarker.GetColor(role);
            _teamMarker.material.color = markerColor;
        }

        private void SetupView()
        {
            _playerUI.SetNickName(_roomPlayersService.GetPlayerInfo(Player).ImmutableDataVo.Nickname);
            
        }
    }

    public static class TeamMarker
    {
        public static Color GetColor(TeamRole role) =>
            role switch
            {
                TeamRole.MyPlayer => Color.green,
                TeamRole.AllyPlayer => Color.blue,
                TeamRole.EnemyPlayer => Color.red
            };
    }
}