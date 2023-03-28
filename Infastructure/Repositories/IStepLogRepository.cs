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
    public interface IStepLogRepository
    {
        Task<List<StepLog>> GetAllByPostId(Guid id);
        Task Write(StepLog stepLog);
    }
}
