using System;
using UnityEngine.Serialization;

namespace PunNetwork.NetworkData
{
    public class NetworkDataModel
    {
        [Serializable]
        public class ReadyPlayerInfo
        {
            public int ActorNumber;
            public string Nickname;
            public string CharacterName;
            public int AvatarID;
            
            public ReadyPlayerInfo(int actorNumber, string nickname, string characterName, int avatarID)
            {
                ActorNumber = actorNumber;
                Nickname = nickname;
                CharacterName = characterName;
                AvatarID = avatarID;
            }
        }
    }
}