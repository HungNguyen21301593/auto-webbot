using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace UseCase.Service.Tabs
{
    public interface IPostTabService
    {
        Task SubmitCategories(Post post);
        Task InputAdDetails(Post post);
    }
}
