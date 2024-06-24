using ExitGames.Client.Photon;
using Photon.PhotonUnityNetworking.Code.Common;
using Photon.Realtime;

namespace Utils.Extensions
{
    public static class PlayerExtensions
    {
        public static bool SetCustomProperty(this Player player, Enumerators.PlayerProperty property, object value)
        {
            var props = new Hashtable
            {
                { property.ToString(), value }
            };
            return player.SetCustomProperties(props);
        }

        public static bool TryGetCustomProperty(this Player player, Enumerators.PlayerProperty propertyKey, out object propertyValue)
        {
            var isSuccess = player.CustomProperties.TryGetValue(
                propertyKey.ToString(), out var value);
            propertyValue = value;
            return isSuccess;
        }

        public static void ResetCustomProperties(this Player player) => 
            player.SetCustomProperties(new Hashtable());
    }
}