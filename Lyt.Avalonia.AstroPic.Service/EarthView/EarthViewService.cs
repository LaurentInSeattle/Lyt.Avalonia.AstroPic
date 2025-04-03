namespace Lyt.Avalonia.AstroPic.Service.EarthView;

internal class EarthViewService
{
    private const string RootUrl = "https://new-images-preview-dot-earth-viewer.appspot.com/_api/";

    private static List<EarthViewPictureBasic>? EarthViewPictures;
    private static readonly Random random = new((int)DateTime.Now.Millisecond);

    private static void LoadPhotoLibrary()
    {
        if (EarthViewPictures == null)
        {
            EarthViewPictures = SerializationUtilities.LoadEarthViewPhotoLibrary(out string message);
            if (EarthViewPictures == null)
            {
                throw new Exception(message);
            }
        }
    }

    private static List<string> PickSomeRandomSlugs(int count)
    {
        if (EarthViewPictures == null)
        {
            throw new Exception("No Earthview photo library");
        }

        var list = new List<string>(count);
        while (count > 0)
        {
            int index = random.Next(0, EarthViewPictures.Count);
            string? slug = EarthViewPictures[index].Slug;
            if (!string.IsNullOrWhiteSpace(slug) && !list.Contains(slug))
            {
                list.Add(slug);
                count--;
            }
        }

        return list;
    }

    public static async Task<List<PictureMetadata>> GetPictures()
    {
        EarthViewService.LoadPhotoLibrary();
        List<string> slugs = PickSomeRandomSlugs(8);
        var list = new List<PictureMetadata>(8);
        HttpClient client = new();
        foreach (string slug in slugs)
        {
            string url = string.Concat(RootUrl, slug, ".json");
            string jsonMetadata = await client.GetStringAsync(url);
            EarthViewPicture earthViewPicture = SerializationUtilities.Deserialize<EarthViewPicture>(jsonMetadata);
            var pictureMetadata = new PictureMetadata(earthViewPicture);
            list.Add(pictureMetadata);
        }

        return list;

    }
}