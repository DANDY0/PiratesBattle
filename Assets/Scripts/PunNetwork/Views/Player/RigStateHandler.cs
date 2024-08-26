using Models;
using Utils;
using System.Collections;
using Databases;
using UnityEngine;
using Zenject;

namespace PunNetwork.Views.Player
{
    public class RigStateHandler : MonoBehaviour
    {
        private IAnimationConfigurationsDatabase _animationConfigurationsDatabase;

        public Transform WeaponTransform;
        public Transform RightIKRef;
        public Transform RightIKHint;
        public Transform LeftIKRef;
        public Transform LeftIKHint;

        private AnimationDataVo animationDataVo;
        private Coroutine currentCoroutine;
        private float _duration = 0.5f;
        private Enumerators.RigAnimatorState _rigAnimatorState = Enumerators.RigAnimatorState.Unknown;
        
        public void Initialize(IAnimationConfigurationsDatabase animationConfigurationsDatabase)
        {
            _animationConfigurationsDatabase = animationConfigurationsDatabase;
            animationDataVo = _animationConfigurationsDatabase.AnimationData;
        }
        
        public void OnRigStateChanged(Enumerators.RigAnimatorState newRigState)
        {
            if(newRigState == _rigAnimatorState)
                return;
            
            if (currentCoroutine != null) 
                StopCoroutine(currentCoroutine);

            AnimationData targetData = GetAnimationDataForState(newRigState);

            if (targetData != null) 
                currentCoroutine = StartCoroutine(UpdateRigState(targetData, _duration));
        }

        private AnimationData GetAnimationDataForState(Enumerators.RigAnimatorState rigState)
        {
            return animationDataVo.AnimationsData.Find(data => data.RigState == rigState);
        }

        private IEnumerator UpdateRigState(AnimationData targetData, float duration)
        {
            Vector3 initialWeaponPosition = WeaponTransform.localPosition;
            Quaternion initialWeaponRotation = WeaponTransform.localRotation;
            Vector3 initialRightRefPosition = RightIKRef.localPosition;
            Quaternion initialRightRefRotation = RightIKRef.localRotation;
            Vector3 initialRightHintPosition = RightIKHint.localPosition;
            Vector3 initialLeftRefPosition = LeftIKRef.localPosition;
            Quaternion initialLeftRefRotation = LeftIKRef.localRotation;
            Vector3 initialLeftHintPosition = LeftIKHint.localPosition;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;

                WeaponTransform.localPosition = Vector3.Lerp(initialWeaponPosition, targetData.WeaponPosition, t);
                WeaponTransform.localRotation =
                    Quaternion.Lerp(initialWeaponRotation, Quaternion.Euler(targetData.WeaponRotation), t);

                RightIKRef.localPosition = Vector3.Lerp(initialRightRefPosition, targetData.RightRefPosition, t);
                RightIKRef.localRotation = Quaternion.Lerp(initialRightRefRotation,
                    Quaternion.Euler(targetData.RightRefRotation), t);

                RightIKHint.localPosition = Vector3.Lerp(initialRightHintPosition, targetData.RightHintPosition, t);

                LeftIKRef.localPosition = Vector3.Lerp(initialLeftRefPosition, targetData.LeftRefPosition, t);
                LeftIKRef.localRotation =
                    Quaternion.Lerp(initialLeftRefRotation, Quaternion.Euler(targetData.LeftRefRotation), t);

                LeftIKHint.localPosition = Vector3.Lerp(initialLeftHintPosition, targetData.LeftHintPosition, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Убедимся, что на конечной позиции значения точно соответствуют целевым
            WeaponTransform.localPosition = targetData.WeaponPosition;
            WeaponTransform.localRotation = Quaternion.Euler(targetData.WeaponRotation);
            RightIKRef.localPosition = targetData.RightRefPosition;
            RightIKRef.localRotation = Quaternion.Euler(targetData.RightRefRotation);
            RightIKHint.localPosition = targetData.RightHintPosition;
            LeftIKRef.localPosition = targetData.LeftRefPosition;
            LeftIKRef.localRotation = Quaternion.Euler(targetData.LeftRefRotation);
            LeftIKHint.localPosition = targetData.LeftHintPosition;
        }
    }
}