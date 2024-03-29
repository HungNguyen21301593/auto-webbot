﻿using Core.Model;
using Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Util;

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

        private async Task TryStartRepostingWithTitle(Setting paramsSetting)
        {
            var post = await PostRepository.GetNextAdToPost();
            if (post is null)
            {
                Logger.LogInformation($"There is no active post available, no work will be proceeded");
                return;
            }

            var existingPost = await PostRepository.GetById(post.Id);
            if (existingPost is null)
            {
                Logger.LogInformation($"Could not found any ad with id {post.Id} from the database, so return");
                return;
            }
            var title = JsonConvert.DeserializeObject<AdDetails>(existingPost.AdDetailJson).AdTitle;
            if (title is null)
            {
                Logger.LogInformation($"Could not get the title from the saved ads on db with id {existingPost.Id}, content: {existingPost.AdDetailJson}");
                return;
            }
            Logger.LogInformation($"Ad with title {title} is on status {existingPost.Status}");
            if (!Consts.ShouldBeRePostedStatuses.Contains(existingPost.Status))
            {
                Logger.LogInformation($"Ad with title {title} has already started posting, so return and do nothing");
                return;
            }
            try
            {
                Logger.LogInformation($"Found ad with title {title}, proceed repost");

                var result = await StartRepostingWithTitle(existingPost, paramsSetting);
                if (!result)
                {
                    Logger.LogInformation($"Ad with title {title} wasn't reposted, so reset status, and retry later");
                    await TryResetSteps(existingPost);
                    return;
                }
                Logger.LogInformation("******************************************************");
                Logger.LogInformation($"Done reposting ad with title {title}");
                Logger.LogInformation("******************************************************");

                await DeviceInfoChart.UpdateRemainingPostAndSaveDeviceInfo();
                Logger.LogInformation("******************************************************");
                Logger.LogInformation($"Updated remaining posts");
                Logger.LogInformation("******************************************************");
            }
            catch (Exception e)
            {
                Logger.LogError($"There was an error with posting ad id {post.Id} so reset status, and retry later, {e}");
                Logger.LogError("******************************************************");
                Logger.LogError($"Failed reposting ad with title {title}");
                Logger.LogError("******************************************************");
                await TryResetSteps(existingPost);
                await FreezeWhenAnythingFailed();
            }
        }

        private async Task<bool> StartRepostingWithTitle(Post existingPost, Setting paramsSetting)
        {
            string title = JsonConvert.DeserializeObject<AdDetails>(existingPost.AdDetailJson).AdTitle ?? throw new ArgumentNullException();
            Logger.LogInformation("******************************************************");
            Logger.LogInformation($"Proceed reposting ad with title {title}");
            Logger.LogInformation("******************************************************");
            var isAdAlreadyPresent = await ReadAdTabService.SearchAdTitle(title);
            var isAdExceedPage = await ReadAdTabService.IsAdExceedPage(title, paramsSetting.PageToTrigger);
            if (!isAdAlreadyPresent
                && existingPost.ShouldRepostEvenIfNotShowing())
            {
                Logger.LogInformation($"Ad with title {title} is not present, and has status {existingPost.Status}, so proceed repost only.");
                await SetAdAsStarted(existingPost);
                return await ProceedRepostOnly(existingPost, title);
            }

            if (!isAdAlreadyPresent
                && !existingPost.ShouldRepostEvenIfNotShowing())
            {
                Logger.LogInformation($"Ad with title {title} is not present, and has status {existingPost.Status}, so proceed to remove completely.");
                await ProceedRemove(existingPost);
                return false;
            }

            if (isAdAlreadyPresent && isAdExceedPage)
            {
                Logger.LogInformation($"Ad with title {title} is present and exceed page {paramsSetting.PageToTrigger}, so proceed delete, then repost.");
                await SetAdAsStarted(existingPost);
                return await ProceedDeleteThenRepost(existingPost, title, paramsSetting.PageToTrigger);
            }

            Logger.LogInformation($"Ad with title {title} is present and NOT exceed page {paramsSetting.PageToTrigger}, so do nothing, return.");
            return false;
        }

        private async Task SetAdAsStarted(Post existingPost)
        {
            existingPost.Status = AdStatus.Started;
            existingPost.stepLogs = new List<StepLog>();
            await PostRepository.Update(existingPost);
        }

        private async Task<bool> ProceedDeleteThenRepost(Post existingPost, string title, long Page)
        {
            var deleteResult = await ProceedDelete(existingPost, title);
            
            if (!deleteResult)
            {
                Logger.LogError("Ad deleted failed, so proceed to freeze, return and reset");
                await FreezeWhenAnythingFailed();
                return false;
            }
            var postSuccess = await ProceedRePost(existingPost, title);
            if (!postSuccess)
            {
                Logger.LogError("Ad posted failed, so proceed to freeze, return and reset");
                 await FreezeWhenAnythingFailed();
                return false;
            }
            return postSuccess;
        }

        private async Task<bool> ProceedRepostOnly(Post existingPost, string title)
        {
            var postSuccess = await ProceedRePost(existingPost, title);
            if (!postSuccess)
            {
                Logger.LogError("Ad posted failed, so proceed to freeze, return and reset");
                await FreezeWhenAnythingFailed();
                return false;
            }
            return postSuccess;
        }

        private async Task ProceedRemove(Post existingPost)
        {
            existingPost.Status = AdStatus.PostSucceeded;
            await PostRepository.Update(existingPost);
        }

        private async Task<bool> ProceedRePost(Post existingPost, string title)
        {
            await PostTabService.SubmitCategories(existingPost);
            await PostTabService.InputAdDetails(existingPost);
            existingPost.Status = await GetFinalPostStatus(existingPost.Id);
            await PostRepository.Update(existingPost);

            var isAdPresentOnActiveList = await ReadAdTabService.SearchAdTitle(title, activeList: true);
            var isAdPresentOnInActiveList = await ReadAdTabService.SearchAdTitle(title, activeList: false);
            if (!isAdPresentOnActiveList && !isAdPresentOnInActiveList)
            {
                Logger.LogError("ValidateFailed: All posting steps has been executed successfully but is not showed");
                existingPost.Status = AdStatus.ValidateFailed;
                await PostRepository.Update(existingPost);
            }
            Logger.LogInformation($"Ad status: {existingPost.Status} details: {JsonConvert.SerializeObject(existingPost.AdDetailJson)}");
            return existingPost.Status == AdStatus.PostSucceeded;
        }

        private async Task<bool> ProceedDelete(Post existingPost, string title)
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

            var isAdPresentAfterDelete = await ReadAdTabService.SearchAdTitle(title);
            if (isAdPresentAfterDelete)
            {
                Logger.LogError("ValidateFailed: All delete steps has been executed successfully but ad still present");
                existingPost.Status = AdStatus.ValidateFailed;
                await PostRepository.Update(existingPost);
                return false;
            }

            if (existingPost.Status == AdStatus.DeleteSucceeded)
            {
                await WaitAfterDelete();
            }
            return existingPost.Status == AdStatus.DeleteSucceeded;
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

        private async Task TryResetSteps(Post post)
        {
            try
            {
                post.Created = DateTime.UtcNow;
                post.stepLogs = new List<StepLog>();
                await PostRepository.Update(post);
            }
            catch (Exception)
            {
            }
        }

        private async Task SetupTabs()
        {
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.Signin.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.AllAds.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.Delete.ToString());
            BrowserManager.OpenNewTabAndSetName(KijijiBrowserTabs.PostNew.ToString());
        }


        private async Task FreezeWhenAnythingFailed()
        {
            var freezeTime = TimeSpan.FromHours(2);
            Console.WriteLine($"There was an error with reposting ad, proceed to frezee for {freezeTime}");
           await Task.Delay(freezeTime);
        }
    }
}
