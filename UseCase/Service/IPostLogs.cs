using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using FireBaseAuthenticator.Model;

namespace UseCase.Service
{
    public interface IPostLogs
    {
        Task<List<Post>> Read();
    }
}
