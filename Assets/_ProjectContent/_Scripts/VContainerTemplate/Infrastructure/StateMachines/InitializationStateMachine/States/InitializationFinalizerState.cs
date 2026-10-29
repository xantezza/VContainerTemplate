using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Tayx.Graphy.Utils.NumString;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.Analytics;
using VContainerTemplate.Infrastructure.StateMachines.GameLoop;
using VContainerTemplate.Infrastructure.StateMachines.GameLoop.States;

namespace VContainerTemplate.Infrastructure.StateMachines.Initialization.States
{
    [UsedImplicitly]
    public class InitializationFinalizerState : BaseInitializationState, IEnterableState
    {
        private readonly GameLoopStateMachine _gameLoopStateMachine;
        private readonly IAnalyticsService _analyticsService;

        [Inject]
        public InitializationFinalizerState(
            InitializationStateMachine stateMachine,
            GameLoopStateMachine gameLoopStateMachine,
            IAnalyticsService analyticsService
        ) : base(stateMachine)
        {
            _analyticsService = analyticsService;
            _gameLoopStateMachine = gameLoopStateMachine;
        }

        public async UniTask Enter()
        {
            _analyticsService.SendEvent(AnalyticsNames.LoadTime, Time.realtimeSinceStartup);
            CollectHardwareData();
#if SKIP_MENU && DEV
            await _gameLoopStateMachine.Enter<GameplayState>();
#else
            await _gameLoopStateMachine.Enter<MenuState>();
#endif
        }

        private void CollectHardwareData()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append("\n")
                .Append("CPU: ")
                .Append(SystemInfo.processorType)
                .Append(" [")
                .Append(SystemInfo.processorCount)
                .Append(" cores]\n");

            sb.Append("RAM: ")
                .Append(SystemInfo.systemMemorySize)
                .Append(" MB\n");

            sb.Append("Graphics API: ")
                .Append(SystemInfo.graphicsDeviceVersion)
                .Append("\n");

            sb.Append("GPU: ")
                .Append(SystemInfo.graphicsDeviceName)
                .Append("\n");

            sb.Append("VRAM: ")
                .Append(SystemInfo.graphicsMemorySize)
                .Append("MB. Max texture size: ")
                .Append(SystemInfo.maxTextureSize)
                .Append("px. Shader level: ")
                .Append(SystemInfo.graphicsShaderLevel)
                .Append("\n");

            Resolution res = Screen.currentResolution;
            sb.Append("Screen: ")
                .Append(res.width)
                .Append('x')
                .Append(res.height)
#if UNITY_2022_2_OR_NEWER
                .Append("@")
                .Append(((int)Screen.currentResolution.refreshRateRatio.value).ToStringNonAlloc())
#else
                .Append("@")
                .Append(res.refreshRate)
#endif
                .Append("Hz\n");

            sb.Append("OS: ")
                .Append(SystemInfo.operatingSystem)
                .Append(" [")
                .Append(SystemInfo.deviceType)
                .Append(']')
                .Append('\n');

            var hardwareDataString = sb.ToString();
            _analyticsService.SendEvent(AnalyticsNames.HardwareData, hardwareDataString);
        }
    }
}