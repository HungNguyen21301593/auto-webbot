using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Model;
using Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        public DataContext DataContext { get; }

        public PostRepository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public readonly List<AdStatus> ShouldBeRePostedStatuses = new()
        {
            AdStatus.New,
            AdStatus.DeleteFailed,
            AdStatus.PostedFailed,
            AdStatus.ReadFailed,
            AdStatus.ValidateFailed
        };

        public async Task<Post?> GetNextAdToPost()
        {
            return await DataContext.Posts.Where(p => ShouldBeRePostedStatuses.Contains(p.Status))
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
            return await DataContext.Posts.Where(p => p.Status != AdStatus.PostSucceeded).ToListAsync();
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
    }
}
