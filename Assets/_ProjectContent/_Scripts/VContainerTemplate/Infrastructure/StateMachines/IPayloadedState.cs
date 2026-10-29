using Cysharp.Threading.Tasks;

namespace VContainerTemplate.Infrastructure.StateMachines
{
    public interface IPayloadedState<in TPayload>
    {
        UniTask Enter(TPayload payload);
    }
}