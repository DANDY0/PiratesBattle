using Controllers;
using States.Core;

namespace States
{
    public class GameResultsState : IState
    {
        private readonly MatchResultsController _matchResultsController;

        public GameResultsState
        (
            MatchResultsController matchResultsController
        )
        {
            _matchResultsController = matchResultsController;
        }
        
        public void Enter()
        {
            _matchResultsController.Show();
        }

        public void Exit()
        {
            
        }
    }
}