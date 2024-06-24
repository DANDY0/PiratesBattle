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
            public string CharacterName;
            public int AvatarID;
            
            public PlayerSpawnedData(int actorNumber, string nickname, string characterName, int avatarID)
            {
                ActorNumber = actorNumber;
                Nickname = nickname;
                CharacterName = characterName;
                AvatarID = avatarID;
            }
            
        }
    }
}