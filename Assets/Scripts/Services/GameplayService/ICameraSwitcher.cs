namespace Services.GameplayService
{
    public interface ICameraSwitcher
    {
        public void SwitchToPlayerCamera();
        public void SwitchToOverviewCamera();

        void Init();
    }
}