using Utils;

namespace Services.PunNetwork
{
    public interface ICustomPropertiesService
    {
        public void SetPlayerProperty(Enumerators.PlayerProperty property, object value);
    }
}