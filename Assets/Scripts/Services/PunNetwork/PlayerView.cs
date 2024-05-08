using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;

public class PlayerView : MonoBehaviour, IPunInstantiateMagicCallback
{
    public PlayerInfo Info;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Info = new PlayerInfo { TeamType = Enumerators.TeamType.MyPlayer };
        else if (info.Sender.ActorNumber % 2 == PhotonNetwork.LocalPlayer.ActorNumber % 2)
            Info = new PlayerInfo { TeamType = Enumerators.TeamType.AllyPlayer };
        else
            Info = new PlayerInfo { TeamType = Enumerators.TeamType.EnemyPlayer };

        Info.TeamId = info.Sender.ActorNumber % 2;

        gameObject.transform.position = new Vector3(info.Sender.ActorNumber, 0, 0);
    }

    [Serializable]
    public class PlayerInfo
    {
        public int TeamId;
        public Enumerators.TeamType TeamType;
    }

    public class TeamCreator
    {
        public const string TeamProperty = "Team";

        public void AssignTeam()
        {
            var player = PhotonNetwork.LocalPlayer;
            var players = PhotonNetwork.PlayerList;

            int teamID = (players.ToList().IndexOf(player) % 2) + 1;
            Hashtable props = new Hashtable
            {
                { TeamProperty, teamID }
            };

            player.SetCustomProperties(props);
        }

        public int GetPlayerTeam(Player player)
        {
            object teamId;
            if (player.CustomProperties.TryGetValue(TeamProperty, out teamId))
            {
                return (int)teamId;
            }

            return 0;
        }
    }
}