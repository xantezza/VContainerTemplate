using Cysharp.Threading.Tasks;

namespace VContainerTemplate.Infrastructure.StateMachines
{
    public abstract class SequentialStateMachine : BaseStateMachine
    {
        private bool _completed;
        
        public async UniTask NextState(bool forceWhenCompleted = false)
        {
            if (_completed && !forceWhenCompleted) return;
            
            var indexOfActiveState = -1;
            if (_activeState != null)
            {
                indexOfActiveState = _statesList.IndexOf(_activeState);
                if (indexOfActiveState == _statesList.Count - 1)
                {
                    await _activeState.Exit();
                    _activeState = null;
                    _completed = true;
                    return;
                }
            }

            await Enter(indexOfActiveState + 1);
        }
    }
}