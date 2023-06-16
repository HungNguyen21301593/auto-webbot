using System.Drawing;

namespace UseCase.Service
{
    public interface IImageService
    {
        string FindMostSimilarImage(string queryImagePath, string folderPath);
    }
}
