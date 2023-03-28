using Core.Model;
using Entities;

namespace UseCase.Service.Tabs
{
    public interface IReadAdTabService
    {
        Task<List<string>> ReadAllAdTitlesExceedPage(long page);
        
        Task<bool> SearchAdTitle(string title);
        
        Task<AdDetails> ReadAdContentByTitle(string title, Post post);
    }
}
