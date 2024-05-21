using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Utils.Extensions
{
    public static class PlayerExtensions
    {
        public static void SetCustomProperty(this Player player, Enumerators.PlayerProperty property, object value)
        {
            var props = new Hashtable
            {
                { property.ToString(), value }
            };
            player.SetCustomProperties(props);
        }

        public static bool TryGetCustomProperty(this Player player, Enumerators.PlayerProperty propertyKey, out object propertyValue)
        {
            var isSuccess = player.CustomProperties.TryGetValue(
                propertyKey.ToString(), out var value);
            propertyValue = value;
            return isSuccess;
        }
    }
}