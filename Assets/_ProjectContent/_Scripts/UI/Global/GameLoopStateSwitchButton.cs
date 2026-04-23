using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine;
using VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.Factory;

namespace VContainerTemplate.UI.Global
{
    [RequireComponent(typeof(Button))]
    public class GameLoopStateSwitchButton : MonoBehaviour
    {
        [SerializeField] private TargetGameLoopState _targetState = 0;
        [HideInInspector] [SerializeField] private Button _button;

        private IGameLoopStateMachineFactory _gameLoopStateMachineFactory;

        [Inject]
        private void Inject(
            IGameLoopStateMachineFactory gameLoopStateMachineFactory)
        {
            _gameLoopStateMachineFactory = gameLoopStateMachineFactory;
        }

        private void OnValidate()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private async void OnClick()
        {
            await _gameLoopStateMachineFactory.GetFrom(this).Enter(_targetState);
        }
    }
}