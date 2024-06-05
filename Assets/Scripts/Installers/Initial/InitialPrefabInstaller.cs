using UnityEngine;
using Zenject;

namespace Installers.Initial
{
    [CreateAssetMenu(menuName = "Installers/InitialPrefabInstaller", fileName = "InitialPrefabInstaller")]
    public class InitialPrefabInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
        }
    }
}