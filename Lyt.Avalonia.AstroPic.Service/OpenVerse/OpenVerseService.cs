namespace Lyt.Avalonia.AstroPic.Service.OpenVerse;

internal class OpenVerseService
{
    // See: https://api.openverse.org/v1/#tag/images/operation/images_search
    // Ex: https://api.openverse.org/v1/images/?q=landscape+nature+sea 

    // WSXGA: Just a tad less than HD 
    private const int MinWidth = 1600;

    // To ensure image diversity: 
    private static readonly List<string>  GeneralTags = ["landscape", "nature"];

    // Randomize through two categories 
    private static readonly List<string> MoodTags = 
        ["sky", "sunset" , "water", "rain", "storm" , "sunny",  "dark", "light"];
    private static readonly List<string> BiomeTags = 
        ["sea", "beach", "forest", "swamp", "savanah", "mountain" ,  "cliffs" , "coast"];
    private static readonly List<string> CountryTags = 
        ["france", "italy", "europe", "usa", "uk", "australia", "greece", "island", "tropical"]; 
    
    private static readonly List<List<string>> TagCategories = 
         [MoodTags, BiomeTags, CountryTags ];

    internal const string Endpoint = "https://api.openverse.org";
    internal const string Request = "/v1/images/";

    public static async Task<List<PictureMetadata>> GetPictures(IRandomizer randomizer)
    {
        int retries = 3;
        while (retries > 0)
        {
            var list = await TryGetPictures(randomizer);
            if (list.Count == 0)
            {
                Debug.WriteLine("No Content; Retrying... ");
                -- retries; 
            }
            else
            {
                return list;
            } 
        }

        throw new Exception("Still no Content after retries...");
    }

    private static async Task<List<PictureMetadata>> TryGetPictures(IRandomizer randomizer)
    {

        var client = new RestClient(OpenVerseService.Endpoint);
        var request = new RestRequest(OpenVerseService.Request, Method.Get)
        {
            RequestFormat = DataFormat.Json
        };
        request.AddQueryParameter("size", "large"); 
        request.AddQueryParameter("aspect_ratio", "wide");
        request.AddQueryParameter("category", "photograph");
        request.AddQueryParameter("mature", "false");

        // Randomize the tags
        string tags = GenerateTags(randomizer);
        request.AddQueryParameter("tags", tags);
        RestResponse response = await client.ExecuteAsync(request);
        response.StatusCode.ThrowIfBad();
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            var responseObject = JsonSerializer.Deserialize<OpenVerseResults>(response.Content);
            if (responseObject is OpenVerseResults results)
            {
                var list = new List<PictureMetadata>(20);
                List<OpenVersePicture> openVersePictures = results.OpenVersePictures;
                if (openVersePictures is null || openVersePictures.Count == 0)
                {
                    Debug.WriteLine("No Content (null or empty)");
                    return list; 
                }

                foreach (OpenVersePicture openVersePicture in openVersePictures)
                {
                    // Skip image if too small or size unknown 
                    int? width = openVersePicture.Width; 
                    if ( width is null || (int) width < MinWidth)
                    {
                        continue;
                    }

                    list.Add(new PictureMetadata(openVersePicture));
                }

                if (list.Count == 0)
                {
                    Debug.WriteLine("No Content (all images too small)");
                }

                return list;
            }

            throw new Exception("Failed to deserialize bing data");
        }
        else
        {
            throw new Exception("No Content (null or empty)");
        }
    }

    private static string GenerateTags(IRandomizer randomizer)
    {
        randomizer.Shuffle(TagCategories);
        var first = TagCategories[0];
        var second   = TagCategories[1];
        randomizer.Shuffle(first);
        randomizer.Shuffle(second);
        return string.Concat(
            GeneralTags[0], "+", GeneralTags[1], "+", first[0], "+" , second[0]);
    }
}
