using Cysharp.Threading.Tasks;

namespace VContainerTemplate.Infrastructure.StateMachines
{
    public interface IEnterableState
    {
        UniTask Enter();
        
    }
}