using System;
using Behaviours;
using UnityEngine;
using Zenject;
using System.Collections;
using UnityEngine.EventSystems;

namespace Services.Input
{
    using UnityEngine;
    using System.Collections;

    using UnityEngine;
    using System.Collections;

    public class InputService : IInputService, IInitializable, ITickable
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public event Action StartFiringEvent;
        public event Action StopFiringEvent;
        public event Action SetAutoFireEvent;
        public event Action SetPreciseFireEvent;
        
        public bool IsFiring { get; private set; }
        public bool IsAutoFiring { get; private set; }
        public bool IsPreciseFiring { get; private set; }
        public Vector2 MoveAxis => GetInputAxis(MoveHorizontal, MoveVertical);
        public Vector2 LookAxis => GetInputAxis(LookHorizontal, LookVertical);

        protected const string MoveHorizontal = "Horizontal";
        protected const string MoveVertical = "Vertical";
        protected const string LookHorizontal = "ShootHorizontal";
        protected const string LookVertical = "ShootVertical";
        
        private Coroutine _fireRoutine;
        private float _fireDelay = 0.3f;
        private float _preciseLimitValue = 0.5f;

        public InputService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void Initialize()
        {
            SimpleInput.FireTriggeredEvent += FireTriggeredEvent;
        }

        public void Tick()
        {
            HandleShooting();
        }

        private void FireTriggeredEvent(bool state)
        {
            if (state) StartFiring();
            else StopFiring();
        }

        private Vector2 GetInputAxis(string horizontal, string vertical) =>
            new Vector2(SimpleInput.GetAxis(horizontal), SimpleInput.GetAxis(vertical));

        private void HandleShooting()
        {
            if (LookAxis.magnitude > 0 && _fireRoutine == null)
                _fireRoutine = _coroutineRunner.StartCoroutine(FireRoutine());
        }

        private IEnumerator FireRoutine()
        {
            yield return new WaitForSeconds(_fireDelay);
            while (IsFiring)
            {
                HandleFiringLogic();
                yield return null;
            }
        }

        private void HandleFiringLogic()
        {
            float magnitude = LookAxis.magnitude;
            bool shouldAutoFire = magnitude <= _preciseLimitValue && !IsAutoFiring;
            bool shouldPrecisionFire = magnitude > _preciseLimitValue && !IsPreciseFiring;

            if (shouldAutoFire)
                TriggerAutoFire();
            else if (shouldPrecisionFire) 
                TriggerPrecisionFire();
        }

        private void TriggerAutoFire()
        {
            Debug.Log("AUTOFIRE Started");
            AutoTargetAndFire();
            SetAutoFireEvent?.Invoke();
            IsAutoFiring = true;
            IsPreciseFiring = false;
        }

        private void TriggerPrecisionFire()
        {
            Debug.Log($"Precision firing at direction: {LookAxis}");
            SetPreciseFireEvent?.Invoke();
            IsAutoFiring = false;
            IsPreciseFiring = true;
        }

        private void StartFiring()
        {
            IsFiring = true;
            StartFiringEvent?.Invoke();
            Debug.Log("Started firing.");
        }

        private void StopFiring()
        {
            if (_fireRoutine != null)
            {
                _coroutineRunner.StopCoroutine(_fireRoutine);
                _fireRoutine = null;
            }

            IsFiring = false;
            IsAutoFiring = false;
            IsPreciseFiring = false;
            StopFiringEvent?.Invoke();
            Debug.Log("Stopped firing.");
        }

        private void AutoTargetAndFire()
        {
            Debug.Log("Auto-targeting and firing at nearest enemy.");
        }
    }
}