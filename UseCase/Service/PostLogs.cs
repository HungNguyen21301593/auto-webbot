using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using FireBaseAuthenticator.Model;
using Infastructure.Repositories;

namespace UseCase.Service
{
    public class PostLogs : IPostLogs
    {
        private readonly IPostRepository postRepository;

        public PostLogs(IPostRepository postRepository)
        {
            this.postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        }

        public async Task<List<Post>> Read()
        {
            return await postRepository.GetWithHistory();
        }
    }
}
