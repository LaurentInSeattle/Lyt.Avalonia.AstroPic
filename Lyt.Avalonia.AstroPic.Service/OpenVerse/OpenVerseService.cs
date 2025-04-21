namespace Lyt.Avalonia.AstroPic.Service.OpenVerse;

internal class OpenVerseService
{
    // See: https://api.openverse.org/v1/#tag/images/operation/images_search
    // Ex: https://api.openverse.org/v1/images/?q=landscape+nature+sea 

    // TODO: To ensure image diversity: 
    // general tags, always there : landscape nature
    // Randomize through two categories : 
    // mood tags : sky sunset water rain storm sunny dark light 
    // season tags : winter spring summer fall  
    // biome tags : sea beach forest swamp savanah mountain cliffs coast 
    // country tags : finland france italy europe usa uk australia greece island tropical

    internal const string Endpoint = "https://api.openverse.org";
    internal const string Request = "/v1/images/";

    public static async Task<List<PictureMetadata>> GetPictures()
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

        // TODO: Randomize the tags
        request.AddQueryParameter("tags", "landscape+nature+sea");
        RestResponse response = await client.ExecuteAsync(request);
        response.StatusCode.ThrowIfBad();
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            var responseObject = JsonSerializer.Deserialize<OpenVerseResults>(response.Content);
            if (responseObject is OpenVerseResults results)
            {
                List<OpenVersePicture> openVersePictures = results.OpenVersePictures;
                if (openVersePictures is null || openVersePictures.Count == 0)
                {
                    throw new Exception("No Content (null or empty)");
                }

                var list = new List<PictureMetadata>(20);
                foreach (OpenVersePicture openVersePicture in openVersePictures)
                {
                    list.Add(new PictureMetadata(openVersePicture));
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
}
