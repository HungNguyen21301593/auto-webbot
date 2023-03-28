using Entities;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using UseCase.Service.Tabs;

namespace UseCase.Service
{
    public class KijijiActionHelper
    {
        public IStepLogRepository StepLogRepository { get; }
        public ILogger<KijijiActionHelper> Logger { get; }

        public KijijiActionHelper(IStepLogRepository stepLogRepository,
            ILogger<KijijiActionHelper> logger)
        {
            StepLogRepository = stepLogRepository;
            Logger = logger;
        }

        public async Task<TOutPut?> ExecuteAndSaveResult<TOutPut>(Func<TOutPut> action, Post post, StepType type, bool skipAble = false)
        {
            var result = Execute(action, type, skipAble);
            await StepLogRepository.Write(new StepLog
            {
                Id = Guid.NewGuid(),
                Result = result.Result,
                Type = result.Type,
                Message = result.Message,
                Post = post
            });
            return result.Output;
        }

        private ActionResult<TOutPut> Execute<TOutPut>(Func<TOutPut> action, StepType type, bool skipAble = false)
        {
            try
            {
                var output = action();
                return new ActionResult<TOutPut>
                {
                    Output = output,
                    Result = Result.Success,
                    Type = type,
                    Message = GetMessage(type)
                };
            }
            catch (Exception e)
            {
                Logger.LogInformation(e.Message);
                if (skipAble)
                {
                    return new ActionResult<TOutPut>
                    {
                        Output = default,
                        Result = Result.Skip,
                        Type = type,
                        Message = GetMessage(type)
                    };
                }
                return new ActionResult<TOutPut>
                {
                    Output = default,
                    Result = Result.Failed,
                    Type = type,
                    Message = GetMessage(type)
                };
            }
        }

        public string GetMessage(StepType type)
        {
            switch (type)
            {
                case StepType.ReadDynamicText:
                    return "Read the advertisement dynamic text";
                case StepType.ReadTitle:
                    return "Read the title of the advertisement.";
                case StepType.DownloadPics:
                    return "Download pictures";
                case StepType.ReadCategories:
                    return "Read the advertisement categories";
                case StepType.ReadAdDescription:
                    return "Read the advertisement description";
                case StepType.ReadTags:
                    return "Read the advertisement tags";
                case StepType.ReadAddress:
                    return "Read the advertisement address";
                case StepType.ReadLocation:
                    return "Read the advertisement location";
                case StepType.ReadPrice:
                    return "Read the advertisement price";
                case StepType.ReadCompany:
                    return "Read the advertisement company";
                case StepType.ReadTypes:
                    return "Read the advertisement types";
                case StepType.ReadCarYear:
                    return "Read the advertisement car year";
                case StepType.ReadCarKm:
                    return "Read the advertisement car km";
                case StepType.ReadPhoneNumber:
                    return "Read the advertisement phone number";
                case StepType.SearchAdBeforeDelete:
                    return "Search for the advertisement before deleting it";
                case StepType.SubmitDelete:
                    return "Submit the delete advertisement form";
                case StepType.InputTitle:
                    return "Input the advertisement title";
                case StepType.SelectCategories:
                    return "Select the advertisement categories";
                case StepType.InputDescription:
                    return "Input the advertisement description";
                case StepType.SelectAdtype:
                    return "Select the advertisement type";
                case StepType.SelectForSaleBy:
                    return "Select for sale by";
                case StepType.SelectMoreInfo:
                    return "Select more information about the advertisement";
                case StepType.SelectFulfillment:
                    return "Select the advertisement fulfillment";
                case StepType.SelectPayment:
                    return "Select the advertisement payment";
                case StepType.SelectTags:
                    return "Select the advertisement tags";
                case StepType.TryInputPicture:
                    return "Try to input the advertisement picture";
                case StepType.InputLocation:
                    return "Input the advertisement location";
                case StepType.InputAddress:
                    return "Input the advertisement address";
                case StepType.InputPrice:
                    return "Input the advertisement price";
                case StepType.InputCompany:
                    return "Input the advertisement company";
                case StepType.InputPhone:
                    return "Input the advertisement phone number";
                case StepType.InputDynamicInputs:
                    return "Input the advertisement dynamic inputs";
                case StepType.InputCarYear:
                    return "Input the advertisement car year";
                case StepType.InputCarKm:
                    return "Input the advertisement car km";
                case StepType.SelectBasicPakage:
                    return "Select the basic advertisement package";
                case StepType.ActiveTermAndCondition:
                    return "Activate the terms and conditions";
                case StepType.SubmitPost:
                    return "Submit the advertisement post form";
                default:
                    return "Invalid step type";
            }
        }

        public class ActionResult<T>
        {
            public T? Output;
            public StepType Type;
            public Result Result;
            public string Message;
        }
    }
}