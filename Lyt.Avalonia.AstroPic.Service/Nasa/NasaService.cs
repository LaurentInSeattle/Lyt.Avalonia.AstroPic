namespace Lyt.Avalonia.AstroPic.Service.Nasa;

internal class NasaService
{
    public const string ApiKey = "DEMO_KEY";
    public const string Endpoint = "https://api.nasa.gov";
    public const string Request = "/planetary/apod";

    public static async Task<List<PictureMetadata>> GetPictures()
    {
        var client = new RestClient(NasaService.Endpoint);
        var request = new RestRequest(NasaService.Request, Method.Get)
        {
            RequestFormat = DataFormat.Json
        };

        request.AddQueryParameter("api_key", NasaService.ApiKey);
        RestResponse response = await client.ExecuteAsync(request);
        response.StatusCode.ThrowIfBad();
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            var responseObject = JsonSerializer.Deserialize<NasaPicture>(response.Content);
            if (responseObject is NasaPicture nasaPicture)
            {
                var list = new List<PictureMetadata>() { new(nasaPicture) };
                return list;
            }

            throw new Exception("Failed to deserialize nasa data");
        }
        else
        {
            throw new Exception("No Content (null or empty)");
        }
    }
}
