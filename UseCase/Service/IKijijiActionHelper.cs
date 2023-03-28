using Entities;

namespace UseCase.Service
{
    internal interface IKijijiActionHelper
    {
        Task ExecuteAndSaveResult<TOutPut>(Func<TOutPut> action, Post post, StepType type, bool skipAble = false);
    }
}
