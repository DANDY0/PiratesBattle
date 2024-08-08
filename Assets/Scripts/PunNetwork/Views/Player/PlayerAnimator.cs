using System;
using Logic;
using UnityEngine;
using static Utils.Enumerators;

namespace PunNetwork.Views.Player
{
    public class PlayerAnimator : MonoBehaviour, IAnimationStateReader
    {
        private Animator _animator;
        
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int Aim = Animator.StringToHash("Aim");

        private readonly int _walkStateHash = Animator.StringToHash("_Walk");
        private readonly int _idleStateHash = Animator.StringToHash("_Idle");
        private readonly int _walkAimStateHash = Animator.StringToHash("_WalkAim");
        private readonly int _idleAimStateHash = Animator.StringToHash("_IdleAim");

        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;
        public AnimatorState State { get; private set; }
        public RigAnimatorState RigState { get; private set; }

        private void Awake() => _animator = GetComponent<Animator>();

        public void MoveAnimation(bool isMoving) => _animator.SetBool(Move, isMoving);

        public void FireAim(bool isAim) => _animator.SetBool(Aim, isAim);

        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            StateEntered?.Invoke(State);
        }

        public void ExitedState(int stateHash) =>
            StateExited?.Invoke(StateFor(stateHash));

        private AnimatorState StateFor(int stateHash)
        {
            AnimatorState state;

            if (stateHash == _idleStateHash)
            {
                state = AnimatorState.Idle;
                RigState = RigAnimatorState.Calm;
            }
            else if (stateHash == _walkStateHash)
            {
                state = AnimatorState.Walk;
                RigState = RigAnimatorState.Firing;
            }
            else if (stateHash == _idleAimStateHash)
            {
                state = AnimatorState.IdleAim;
                RigState = RigAnimatorState.Calm;
            }
            else if (stateHash == _walkAimStateHash)
            {
                state = AnimatorState.WalkAim;
                RigState = RigAnimatorState.Firing;
            }
            else
                state = AnimatorState.Unknown;

            Debug.LogWarning($"StateFor: {state}");
            return state;
        }
    }
}