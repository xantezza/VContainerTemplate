using System;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.Services.Log;
using VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.Factory;
using VContainerTemplate.Infrastructure.StateMachines.GameLoopStateMachine.States;

namespace VContainerTemplate.Infrastructure
{
    public class EntryPoint : MonoBehaviour
    {
        [Inject] private IGameLoopStateMachineFactory _stateMachineFactory;

        public static bool HasStarted { get; private set; }
        
        public async void Start()
        {
            try
            {
                HasStarted = true;
                await _stateMachineFactory.GetFrom(this).Enter<EntryPointState>();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}