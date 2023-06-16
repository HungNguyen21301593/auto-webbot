using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class ImageComparator
{
    public static string FindHighestSizeSimilarImage(string queryImagePath, string folderPath, double similarityThreshold)
    {
        string[] imageFiles = Directory.GetFiles(folderPath, "*.jpg");

        using (var queryImage = Image.FromFile(queryImagePath))
        {
            var similarImages = imageFiles
                .Select(file => new { Path = file, Image = Image.FromFile(file) })
                .Where(image => CalculateSsim(queryImage, image.Image) >= similarityThreshold)
                .OrderByDescending(image => new FileInfo(image.Path).Length);

            return similarImages.FirstOrDefault()?.Path;
        }
    }

    // Function to calculate the SSIM between two images
    private static double CalculateSsim(Image image1, Image image2)
    {
        using var bitmap1 = new Bitmap(image1);
        using var bitmap2 = new Bitmap(image2);

        return 0;
    }
}
