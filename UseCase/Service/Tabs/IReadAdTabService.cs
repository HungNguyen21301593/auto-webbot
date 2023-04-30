using Core.Model;
using Entities;

namespace UseCase.Service.Tabs
{
    public interface IReadAdTabService
    {
        Task<List<string>> ReadAllAdTitlesExceedPage(long page);

        Task<bool> SearchAdTitle(string title);

        Task<bool> IsAdExceedPage(string title, long page);

        Task<AdDetails> ReadAdContentByTitle(string title, Post post);
    }
}
