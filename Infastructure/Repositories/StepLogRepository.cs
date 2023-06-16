using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories
{
    public class StepLogRepository : IStepLogRepository
    {
        public DataContext DataContext { get; }

        public StepLogRepository(DataContext dataContext)
        {
            DataContext = dataContext;

        }

        public async Task Write(List<StepLog> stepLogs)
        {
            DataContext.StepLog.AddRange(stepLogs);
            await DataContext.SaveChangesAsync();
        }

        public async Task Write(StepLog stepLog)
        {
            return;
            DataContext.StepLog.Add(stepLog);
            await DataContext.SaveChangesAsync();
        }

        public async Task<List<StepLog>> GetAllByPostId(Guid id)
        {
            var result = await DataContext
                .StepLog
                .Include(l => l.Post)
                .Where(log => log.Post.Id == id).ToListAsync();
            return result;
        }
    }
}
