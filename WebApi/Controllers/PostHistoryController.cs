using Entities;
using Microsoft.AspNetCore.Mvc;
using UseCase.Service;
using Util;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostHistoryController : ControllerBase
    {
        private readonly IPostLogs postLogs;
        private readonly KijijiActionHelper kijijiActionHelper;

        public PostHistoryController(IPostLogs postLogs, KijijiActionHelper kijijiActionHelper)
        {
            this.postLogs = postLogs ?? throw new ArgumentNullException(nameof(postLogs));
            this.kijijiActionHelper = kijijiActionHelper;
        }

        [HttpGet]
        [Route("get")]
        public async Task<List<LogTreeNode>> Get()
        {
            var posts = await postLogs.Read();
            var models = posts.Select(post => new LogTreeNode
            {
                Name = post.GetAdDetails().AdTitle ?? "Unknown",
                Status = post.Status.MapToLogTreeStatus(),
                Created = post.Created,
                Children = post.stepLogs.Select(s => new LogTreeNode
                {
                    Status = s.Result.MapResultToLogTreeStatus(),
                    Name = kijijiActionHelper.GetMessage(s.Type) ?? "Unknown",
                    Created = s.Created
                }).ToList()
            }).ToList();
            var newPosts = models.Where(m => m.Status == Core.Model.LogTreeStatus.New).ToList();
            var PendingPosts = models.Where(m => m.Status == Core.Model.LogTreeStatus.Pending).ToList();
            var failedPosts = models.Where(m => m.Status == Core.Model.LogTreeStatus.Failed).ToList();
            var passedPosts = models.Where(m => m.Status == Core.Model.LogTreeStatus.Passed).ToList();
            var progressNodes = new List<LogTreeNode>
            {
                new LogTreeNode{
                Name = $"New ({newPosts.Count})",
                Created= DateTime.Now,
                Status =Core.Model.LogTreeStatus.New,
                Children = newPosts
                },
                new LogTreeNode{
                Name = $"Pending ({PendingPosts.Count})",
                Created= DateTime.Now.AddDays(-1),
                Status =Core.Model.LogTreeStatus.Pending,
                Children = PendingPosts
                },
                new LogTreeNode{
                Name = $"Failed ({failedPosts.Count})",
                Created= DateTime.Now.AddDays(-2),
                Status =Core.Model.LogTreeStatus.Failed,
                Children = failedPosts
                },
                new LogTreeNode{
                Name = $"Passed ({passedPosts.Count})",
                Created= DateTime.Now.AddDays(-3),
                Status =Core.Model.LogTreeStatus.Passed,
                Children = passedPosts
                }
            };
            return progressNodes;
        }
    }
}
