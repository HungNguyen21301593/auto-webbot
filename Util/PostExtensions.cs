using Core.Model;
using Entities;
using Newtonsoft.Json;

namespace Util
{
    public static class PostExtensions
    {
        public static AdDetails GetAdDetails(this Post post)
        {
            return JsonConvert.DeserializeObject<AdDetails>(post.AdDetailJson);
        }

        public static LogTreeStatus MapToLogTreeStatus(this AdStatus status)
        {
            switch (status)
            {
                case AdStatus.New:
                    return LogTreeStatus.New;
                case AdStatus.Started:
                case AdStatus.ReadSucceeded:
                case AdStatus.DeleteSucceeded:
                case AdStatus.ValidateSucceeded:
                    return LogTreeStatus.Pending;
                case AdStatus.ReadFailed:
                case AdStatus.DeleteFailed:
                case AdStatus.ValidateFailed:
                case AdStatus.PostedFailed:
                    return LogTreeStatus.Failed;
                case AdStatus.PostSucceeded:
                    return LogTreeStatus.Passed;
                default:
                    throw new NotSupportedException($"status {status} not supported");
            }
        }

        public static LogTreeStatus MapResultToLogTreeStatus(this Result result)
        {
            switch (result)
            {
                case Result.Success:
                case Result.Skip:
                    return LogTreeStatus.Passed;
                case Result.Failed:
                    return LogTreeStatus.Failed;
                default:
                    return LogTreeStatus.Pending;
            }
        }
    }
}
