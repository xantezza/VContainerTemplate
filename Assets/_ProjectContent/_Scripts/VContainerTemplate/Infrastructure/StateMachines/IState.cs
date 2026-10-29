using Cysharp.Threading.Tasks;

namespace VContainerTemplate.Infrastructure.StateMachines
{
    public interface IState
    {
        public UniTask Exit();
    }
}