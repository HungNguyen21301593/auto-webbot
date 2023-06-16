using Core.Model;
using Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Util;

namespace Infastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        public DataContext DataContext { get; }

        public PostRepository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        

        public async Task<Post?> GetNextAdToPost()
        {
            return await DataContext.Posts.Where(p => Consts.ShouldBeRePostedStatuses.Contains(p.Status))
                .OrderBy(p => p.Created)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetAllByTitleOrDefault(string title)
        {
            var postsWithSameTitle = await DataContext.Posts
                .ToListAsync();

            return postsWithSameTitle
                .Where(p => JsonConvert.DeserializeObject<AdDetails>(p.AdDetailJson).AdTitle == title)
                .ToList();
        }

        public async Task<Post?> GetById(Guid id)
        {
            return await DataContext
                  .Posts
                  .Include(p => p.stepLogs)
                  .Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Post> Create(Post? post = null)
        {
            var newPost = post ?? new Post()
            {
                Id = Guid.NewGuid(),
                Status = AdStatus.New,
                AdDetailJson = "{}",
            };
            DataContext.Posts.Add(newPost);
            await DataContext.SaveChangesAsync();
            return newPost;
        }

        public async Task<Post> Update(Post coreRequest)
        {
            DataContext.Posts.Update(coreRequest);
            await DataContext.SaveChangesAsync();
            return coreRequest;
        }

        public async Task<List<Post>> GetAllFailedOrOnGoingPost()
        {
            return await DataContext.Posts.Where(p => p.Status !=  AdStatus.PostSucceeded && !Consts.ShouldBeRePostedStatuses.Contains(p.Status))
                .Include(p => p.stepLogs)
                .ToListAsync();
        }

        public async Task<List<Post>> UpdateRange(List<Post> posts)
        {
            DataContext.Posts.UpdateRange(posts);
            await DataContext.SaveChangesAsync();
            return posts;
        }

        public async Task<List<Post>> GetWithHistory()
        {
            var posts = await DataContext.Posts
                .Include(p => p.stepLogs)
                .ToListAsync();
            return posts;
        }

        public async Task Remove(Post coreRequest)
        {
            var postToBeRemoved = await DataContext.Posts
                .Include(p => p.stepLogs)
                .FirstOrDefaultAsync(p => p.Id == coreRequest.Id);
            if (postToBeRemoved != null)
            {
                throw new Exception($"Could not found any post with ID {coreRequest.Id}");
            }
            DataContext.StepLog.RemoveRange(coreRequest.stepLogs);
            DataContext.Posts.RemoveRange(coreRequest);
            await DataContext.SaveChangesAsync();
        }
    }
}
