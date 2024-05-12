using System;
using UnityEngine;
using Utils;

namespace Services.PunNetwork
{
    [RequireComponent(typeof(MeshRenderer))]
    public class TeamMarker : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetView(Enumerators.TeamRole role)
        {
            var markerColor = role switch
            {
                Enumerators.TeamRole.MyPlayer => Color.green,
                Enumerators.TeamRole.AllyPlayer => Color.blue,
                Enumerators.TeamRole.EnemyPlayer => Color.red,
                _ => new Color()
            };

            _meshRenderer.sharedMaterial.color = markerColor;
        }
    }
}