using Models;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Databases/AnimationConfigurationsDatabase", fileName = "AnimationConfigurationsDatabase")]
    public class AnimationConfigurationsDatabase: ScriptableObject, IAnimationConfigurationsDatabase
    {
        [SerializeField] private AnimationDataVo _animationData;
        
        public AnimationDataVo AnimationData => _animationData;
    }
}