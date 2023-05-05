using Core.Model;
using Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UseCase.Service
{
    public partial class KijijiPostingService
    {
        private async Task TryLogInAndLogOutIfNewAccountSetting(Setting? Setting)
        {
            var currentSetting = GlobalLockResourceService.CurrentSetting;
            
            if (Setting == null) throw new ArgumentNullException(nameof(Setting));
            if (Setting?.KijijiEmail is null)
            {
                Logger.LogInformation($"The KijijiEmail is invalid {Setting?.KijijiEmail} So return");
                return;
            }

            if (Setting?.KijijiPassword is null)
            {
                Logger.LogInformation($"The KijijiPassword is invalid {Setting?.KijijiPassword} So return");
                return;
            }

            if (currentSetting == null)
            {
                // first login
                await SigninService.Login(Setting.KijijiEmail, Setting.KijijiPassword);
                GlobalLockResourceService.CurrentSetting = Setting;
                return;
            }

            if (currentSetting?.KijijiEmail != Setting.KijijiEmail || currentSetting?.KijijiPassword != Setting.KijijiPassword)
            {
                // update account
                await SigninService.Logout();
                await SigninService.Login(Setting.KijijiEmail, Setting.KijijiPassword);
                GlobalLockResourceService.CurrentSetting = Setting;
                return;
            }

            // default
            await SigninService.Login(Setting.KijijiEmail, Setting.KijijiPassword);
            GlobalLockResourceService.CurrentSetting = Setting;
        }

        private async Task StartRepostingWithTitle(Post post, Setting paramsSetting)
        {
            Logger.LogInformation("******************************************************");
            Logger.LogInformation($"Proceed reposting ad with id {post.Id}");
            Logger.LogInformation("******************************************************");
            var existingPost = await PostRepository.GetById(post.Id);
            if (existingPost is null)
            {
                Logger.LogInformation($"Could not found any ad with id {post.Id} from the database, so return");
                return;
            }

            var title = JsonConvert.DeserializeObject<AdDetails>(post.AdDetailJson).AdTitle;
            if (title is null)
            {
                Logger.LogInformation($"Could not get the title from the saved ads on db with id {post.Id}, content: {post.AdDetailJson}");
                await ResetPostStatusAndSteps(existingPost);
                return;
            }
            existingPost.stepLogs = new List<StepLog>();
            await PostRepository.Update(existingPost);

            var isAdAlreadyPresent = await ReadAdTabService.SearchAdTitle(title);
            var isAdExceedPage = await ReadAdTabService.IsAdExceedPage(title, paramsSetting.PageToTrigger);
            if (!isAdAlreadyPresent)
            {
                await ProceedRepostOnly(existingPost, title);
                return;
            }
            if (isAdAlreadyPresent && isAdExceedPage)
            {
                await ProceedDeleteThenRepost(existingPost, title,paramsSetting.PageToTrigger);
                return;
            }

            Logger.LogInformation($"Ad with title {title} is present and NOT exceed page {paramsSetting.PageToTrigger}, so do nothing, return.");
            await ResetPostStatusAndSteps(existingPost);
        }

        private async Task ProceedRepostOnly(Post existingPost, string title)
        {
            Logger.LogInformation($"Ad with title {title} is not present so procede repost.");
            var postSuccess = await ProceedRePost(existingPost, title);
            if (!postSuccess)
            {
                await ResetPostStatusAndSteps(existingPost);
            }
        }

        private async Task ProceedDeleteThenRepost(Post existingPost, string title, long Page)
        {
            Logger.LogInformation($"Ad with title {title} is present and exceed page {Page}, so proceed delete, then repost.");
            await ProceedDelete(existingPost, title);
            var postSuccess = await ProceedRePost(existingPost, title);
            if (!postSuccess)
            {
                await ResetPostStatusAndSteps(existingPost);
            }
        }

        private async Task<bool> ProceedRePost(Post existingPost, string title)
        {
            await PostTabService.SubmitCategories(existingPost);
            await PostTabService.InputAdDetails(existingPost);
            existingPost.Status = await GetFinalPostStatus(existingPost.Id);
            await PostRepository.Update(existingPost);

            if (existingPost.Status != AdStatus.PostSucceeded)
            {
                Logger.LogInformation("******************************************************");
                Logger.LogInformation($"Failed reposting ad with title {title}");
                Logger.LogInformation("******************************************************");
                return false;
            }

            Logger.LogInformation("******************************************************");
            Logger.LogInformation($"Done reposting ad with title {title}");
            Logger.LogInformation("******************************************************");
            await DeviceInfoChart.UpdateRemainingPostAndSaveDeviceInfo();
            return true;
        }

        private async Task ProceedDelete(Post existingPost, string title)
        {
            var adDetails = await ReadAdTabService.ReadAdContentByTitle(title, existingPost);
            existingPost.AdDetailJson = JsonConvert.SerializeObject(adDetails);
            existingPost.Status = await GetReadStatus(existingPost.Id);
            await PostRepository.Update(existingPost);
            Logger.LogInformation("******************************************************");
            Logger.LogInformation($"Done reading ad with title {title}");
            Logger.LogInformation($"Content: {JsonConvert.SerializeObject(adDetails)}");
            Logger.LogInformation("******************************************************");


            await DeleteTabService.DeleteAdByTitle(existingPost);
            existingPost.Status = await GetDeleteStatus(existingPost.Id);
            await PostRepository.Update(existingPost);
            Logger.LogInformation("******************************************************");
            Logger.LogInformation($"Done deleting ad with title {title}");
            Logger.LogInformation("******************************************************");

            await WaitAfterDelete();
        }

        private async Task WaitAfterDelete()
        {
            var setting = await SettingRepository.Read();
            Logger.LogInformation($"Wait after deleted: {setting?.WaitAfterDeleteInterval}");
            await Task.Delay(TimeSpan.FromMinutes(setting?.WaitAfterDeleteInterval ?? 0));
        }

        private async Task<AdStatus> GetReadStatus(Guid postId)
        {
            var stepLogs = await StepLogRepository.GetAllByPostId(postId);
            if (stepLogs.Any(l => l.Result == Result.Failed))
            {
                return AdStatus.ReadFailed;
            }
            return AdStatus.ReadSucceeded;
        }

        private async Task<AdStatus> GetDeleteStatus(Guid postId)
        {
            var stepLogs = await StepLogRepository.GetAllByPostId(postId);
            if (stepLogs.Any(l => l.Result == Result.Failed))
            {
                return AdStatus.DeleteFailed;
            }
            return AdStatus.DeleteSucceeded;
        }

        private async Task<AdStatus> GetFinalPostStatus(Guid postId)
        {
            var stepLogs = await StepLogRepository.GetAllByPostId(postId);
            if (stepLogs.Any(l => l.Result == Result.Failed))
            {
                return AdStatus.PostedFailed;
            }
            return AdStatus.PostSucceeded;
        }

        private async Task ReadAllAdExceedPage(long page)
        {
            Logger.LogInformation("******************************************************");
            Logger.LogInformation("Started ReadAllAd");
            Logger.LogInformation("******************************************************");

            var adTitles = await ReadAdTabService.ReadAllAdTitlesExceedPage(page);

            foreach (var title in adTitles)
            {
                var allExistingWithSameTitle = await PostRepository.GetAllByTitleOrDefault(title);
                if (allExistingWithSameTitle.All(p => p.Status == AdStatus.PostSucceeded))
                {
                    var post = new Post
                    {
                        Id = Guid.NewGuid(),
                        Status = AdStatus.New,
                        AdDetailJson = JsonConvert.SerializeObject(new AdDetails { AdTitle = title })
                    };
                    Logger.LogWarning($"Post with title {title} is new or has already posted successfully, proceed to register");
                    await PostRepository.Create(post);
                }
                Logger.LogWarning($"Post with title {title} already registered, so skip");
            }
            Logger.LogInformation("******************************************************");
            Logger.LogInformation("Done ReadAllAd");
            Logger.LogInformation("******************************************************");
        }

        private async Task ResetPostStatusAndSteps(Post post)
        {
            post.Status = AdStatus.New;
            post.stepLogs = new List<StepLog>();
            await PostRepository.Update(post);
        }

        private async Task SetupTabs()
        {
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.Signin.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.AllAds.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.Delete.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.PostNew.ToString());
        }

        private async Task CleanBrowserAndReInit()
        {
            BrowserManager.GetDriver().Manage().Cookies.DeleteAllCookies();
        }
    }
}
