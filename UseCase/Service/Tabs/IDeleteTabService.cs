using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCase.Service.Tabs
{
    public interface IDeleteTabService
    {
        Task DeleteAdByTitle(Post post, bool activeList = true);
    }
}
