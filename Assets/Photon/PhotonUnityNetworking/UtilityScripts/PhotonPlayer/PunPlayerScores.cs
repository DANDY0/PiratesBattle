// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunPlayerScores.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Scoring system for PhotonPlayer
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Scoring system for PhotonPlayer
    /// </summary>
    public class PunPlayerScores : MonoBehaviour
    {
        public const string PlayerScoreProp = "score";
    }

    public static class ScoreExtensions
    {
        public static void SetScore(this Player player, float newScore)
        {
            var score = new Hashtable
            {
                [PunPlayerScores.PlayerScoreProp] = newScore
            };  // using PUN's implementation of Hashtable

            player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
        }

        public static void AddScore(this Player player, float scoreToAddToCurrent)
        {
            var current = player.GetScore();
            current += scoreToAddToCurrent;

            var score = new Hashtable
            {
                [PunPlayerScores.PlayerScoreProp] = current
            };  // using PUN's implementation of Hashtable

            player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
        }

        public static float GetScore(this Player player)
        {
            if (player.CustomProperties.TryGetValue(PunPlayerScores.PlayerScoreProp, out var score))
                return (float)score;

            return 0;
        }
    }
}