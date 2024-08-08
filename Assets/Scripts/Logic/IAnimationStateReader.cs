using Utils;

namespace Logic
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
        Enumerators.AnimatorState State { get; }
    }
}