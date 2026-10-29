using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainerTemplate.Infrastructure.StateMachines.GameLoop;

namespace VContainerTemplate.UI.Global
{
    [RequireComponent(typeof(Button))]
    public class GameLoopStateSwitchButton : MonoBehaviour
    {
        [SerializeField] private TargetGameLoopState _targetState = 0;
        [HideInInspector] [SerializeField] private Button _button;

        private GameLoopStateMachine _gameLoopStateMachine;

        [Inject]
        private void Inject(
            GameLoopStateMachine gameLoopStateMachine)
        {
            _gameLoopStateMachine = gameLoopStateMachine;
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
            await _gameLoopStateMachine.Enter(_targetState);
        }
    }
}