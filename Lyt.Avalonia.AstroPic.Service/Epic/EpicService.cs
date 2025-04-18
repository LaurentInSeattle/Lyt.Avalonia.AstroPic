namespace Lyt.Avalonia.AstroPic.Service.Epic; 

internal class EpicService
{
    /*
    Example queries

    https://api.nasa.gov/EPIC/api/natural/images?api_key=DEMO_KEY
    https://api.nasa.gov/EPIC/api/natural/date/2019-05-30?api_key=DEMO_KEY
    https://api.nasa.gov/EPIC/api/natural/all?api_key=DEMO_KEY
    https://api.nasa.gov/EPIC/archive/natural/2019/05/30/png/epic_1b_20190530011359.png?api_key=DEMO_KEY

    */

    public const string ApiKey = "DEMO_KEY";
    public const string Endpoint = "https://api.nasa.gov";
    public const string Request = "/EPIC/api/natural/images";

    public static async Task<List<PictureMetadata>> GetPictures()
    {
        var client = new RestClient(EpicService.Endpoint);
        var request = new RestRequest(EpicService.Request, Method.Get)
        {
            RequestFormat = DataFormat.Json
        };

        request.AddQueryParameter("api_key", EpicService.ApiKey);
        RestResponse response = await client.ExecuteAsync(request);
        response.StatusCode.ThrowIfBad();
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            var responseObject = JsonSerializer.Deserialize<List<EpicPicture>>(response.Content);
            if (responseObject is List<EpicPicture> epicPictures)
            {
                var list = new List<PictureMetadata>(epicPictures.Count);  
                foreach (var epicPicture in epicPictures)
                {
                    list.Add(new PictureMetadata(epicPicture));
                }

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
