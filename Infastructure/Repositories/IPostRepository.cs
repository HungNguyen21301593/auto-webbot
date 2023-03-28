using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Entities;

namespace Infastructure.Repositories
{
    public interface IPostRepository
    {
        Task<Post?> GetNextAdToPost();
        Task<List<Post>> GetAllByTitleOrDefault(string title);
        Task<List<Post>> GetAllFailedOrOnGoingPost();
        Task<Post?> GetById(Guid id);
        Task<Post> Create(Post? post);
        Task<Post> Update(Post coreRequest);
        Task<List<Post>> UpdateRange(List<Post> posts);
        Task<List<Post>> GetWithHistory();
    }
}
