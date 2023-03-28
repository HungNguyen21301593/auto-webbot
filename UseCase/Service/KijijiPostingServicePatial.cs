using Core.Model;
using Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UseCase.Service
{
    public partial class KijijiPostingService
    {
        private async Task StartRepostingWithTitle(Post post)
        {
            Logger.LogInformation("******************************************************");
            Logger.LogInformation($"Proceed reposting ad with id {post.Id}");
            Logger.LogInformation("******************************************************");
            var existingPost = await PostRepository.GetById(post.Id);
            if (existingPost is null)
            {
                return;
            }

            var title = JsonConvert.DeserializeObject<AdDetails>(post.AdDetailJson).AdTitle;
            if (title is null)
            {
                return;
            }
            existingPost.stepLogs = new List<StepLog>();
            await PostRepository.Update(existingPost);

            var isAdActive = await ReadAdTabService.SearchAdTitle(title);
            if (isAdActive)
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
                existingPost.Status =  await GetDeleteStatus(existingPost.Id);
                await PostRepository.Update(existingPost);
                Logger.LogInformation("******************************************************");
                Logger.LogInformation($"Done deleting ad with title {title}");
                Logger.LogInformation("******************************************************");

                await WaitAfterDelete();
            }

            await PostTabService.SubmitCategories(existingPost);
            await PostTabService.InputAdDetails(existingPost);
            existingPost.Status = await GetFinalPostStatus(existingPost.Id);
            await PostRepository.Update(existingPost);
            Logger.LogInformation("******************************************************");
            Logger.LogInformation($"Done reposting ad with title {title}");
            Logger.LogInformation("******************************************************");

            await DeviceRegistrationService.UpdateNumberOfAllowAds();
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
            await SetupTabs();

            Logger.LogInformation("******************************************************");
            Logger.LogInformation("Started ReadAllAd");
            Logger.LogInformation("******************************************************");
            await SigninService.Login("hungnguyen21301593@gmail.com", "ObayaShi21301593!@");
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
                        AdDetailJson = JsonConvert.SerializeObject(new AdDetails {AdTitle = title})
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

        private async Task SetupTabs()
        {
            //BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.Main.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.Signin.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.AllAds.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.Delete.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.PostNew.ToString());
            await SigninService.Login("hungnguyen21301593@gmail.com", "ObayaShi21301593!@");
        }
    }
}
