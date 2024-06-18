using System;
using UnityEngine.Serialization;

namespace PunNetwork.NetworkData
{
    public class NetworkDataModel
    {
        [Serializable]
        public class PlayerSpawnedData
        {
            public int ActorNumber;
            public string Nickname;
            public int CharacterID;
            public int AvatarID;
            
            public PlayerSpawnedData(int actorNumber, string nickname, int characterID, int avatarID)
            {
                ActorNumber = actorNumber;
                Nickname = nickname;
                CharacterID = characterID;
                AvatarID = avatarID;
            }
        }
    }
}