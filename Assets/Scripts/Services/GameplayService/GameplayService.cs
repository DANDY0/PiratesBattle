using Utils;
using Zenject;

namespace Services.GameplayService
{
    public class GameplayService: IGameplayService
    {
        private ICameraSwitcher _cameraSwitcher;

        public GameplayService(ICameraSwitcher cameraSwitcher)
        {
            _cameraSwitcher = cameraSwitcher;
        }
        public void Activate()
        {
            _cameraSwitcher.Init();
            _cameraSwitcher.SwitchToPlayerCamera();
        }
    }
}