using System;
using Logic;
using UnityEngine;

namespace PunNetwork.Views.Player
{
    public class PlayerAnimator : MonoBehaviour, IAnimationStateReader
    {
        private Animator _animator;
        
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int Aim = Animator.StringToHash("Aim");
        // private static readonly int Walk = Animator.StringToHash("Walk");
        // private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        // private static readonly int Hit = Animator.StringToHash("Hit");
        // private static readonly int Die = Animator.StringToHash("Die");

        private readonly int _moveStateHash = Animator.StringToHash("Move");
        private readonly int _aimStateHash = Animator.StringToHash("Aim");
        // private readonly int _walkingStateHash = Animator.StringToHash("Walk");
        // private readonly int _attackStateHash = Animator.StringToHash("attack01");
        // private readonly int _deathStateHash = Animator.StringToHash("die");

        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;
        public AnimatorState State { get; private set; }

        private void Awake() => _animator = GetComponent<Animator>();

        // public void PlayHit() => _animator.SetTrigger(Hit);
        // public void PlayDeath() => _animator.SetTrigger(Die);

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
            // else if (stateHash == _attackStateHash)
                // state = AnimatorState.Attack;
            // else if (stateHash == _moveStateHash)
                // state = AnimatorState.Walk;
            if (stateHash == _moveStateHash)
                state = AnimatorState.Move;
            else if (stateHash == _aimStateHash)
                state = AnimatorState.Aim;
            else
                state = AnimatorState.Unknown;

            return state;
        }
    }
}