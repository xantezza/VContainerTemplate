using System;
using UnityEngine;
using VContainer;
using VContainerTemplate.Infrastructure.Logger;
using VContainerTemplate.Infrastructure.StateMachines.Initialization;

namespace VContainerTemplate.Infrastructure
{
    public class EntryPoint : MonoBehaviour
    {
        [Inject] private InitializationStateMachine _initializationStateMachine;

        public static bool HasStarted { get; private set; }
        
        public async void Start()
        {
            try
            {
                HasStarted = true;
                
                await _initializationStateMachine.NextState();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}