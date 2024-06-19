using System.Linq;
using Photon.PhotonUnityNetworking.Code.Common.Pool;
using Photon.Pun;
using Services.GamePools;
using Zenject;

namespace PunNetwork.ObjectPooling
{
    public class PhotonPoolObjPunCallbacks : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        private IGamePoolsService _gamePoolsService;
        
        [Inject]
        private void Construct
        (
            IGamePoolsService gamePoolsService
        )
        {
            _gamePoolsService = gamePoolsService;
        }
        
        public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (info.photonView.InstantiationData?.Last() is PoolObjectDataVo { Ifs: true } dataVo) 
                _gamePoolsService.SetItemReady(dataVo.Key, this);
        }
    }
}