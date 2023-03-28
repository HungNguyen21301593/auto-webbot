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
    public class RequestRepository : IRequestRepository
    {
        public DataContext DataContext { get; }

        public RequestRepository(DataContext dataContext)
        {
            DataContext = dataContext;

        }

        public async Task<Request> Create()
        {
            var newRequest = new Request
            {
                Id = Guid.NewGuid(),
            };
            DataContext.Requests.Add(newRequest);
            await DataContext.SaveChangesAsync();
            return newRequest;
        }

        public async Task<Request> Update(Request coreRequest)
        {
            var request = await DataContext.Requests.FindAsync(coreRequest.Id);
            if (request == null)
            {
                throw new Exception("Could not found request");
            }
            await DataContext.SaveChangesAsync();
            return request;
        }
    }
}
