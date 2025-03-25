namespace Lyt.Avalonia.AstroPic.Service.Nasa;

internal class NasaService
{
    public const string ApiKey = "DEMO_KEY";
    public const string Endpoint = "https://api.nasa.gov";

    public static async Task<List<Picture>> GetPictures(DateTime dateTime, int count = 1)
    {
        if ((count <= 0) || (count > 8))
        {
            throw new ArgumentException("Invalid count: max == 8");
        }

        try
        {
            var client = new RestClient(NasaService.Endpoint);
            var request = new RestRequest("/planetary/apod", Method.Get)
            {
                RequestFormat = DataFormat.Json
            };
            // request.AddQueryParameter("count", count.ToString());
            request.AddQueryParameter("api_key", NasaService.ApiKey);
            RestResponse response = await client.ExecuteAsync(request);
            response.StatusCode.ThrowIfBad();
            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                var responseObject = JsonSerializer.Deserialize<NasaPicture>(response.Content);
                if (responseObject is NasaPicture nasaPicture)
                {
                    var list = new List<Picture>() { new(nasaPicture) };
                    return list;
                }

                throw new Exception("Failed to deserialize nasa data");
            }
            else
            {
                throw new Exception("No Content (null or empty)");
            }

        }
        catch (Exception)
        {
            throw;  ;
        }
    }
}
