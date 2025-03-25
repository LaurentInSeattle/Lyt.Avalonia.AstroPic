namespace Lyt.Avalonia.AstroPic.Service.Bing;

internal class BingService
{
   // "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=100&mkt=en-US"

    internal const string Endpoint = "https://www.bing.com";
    internal const string Request = "/HPImageArchive.aspx";

    public static async Task<List<Picture>> GetPictures(DateTime dateTime, int count = 1)
    {
        if ((count <= 0) || (count > 8))
        {
            throw new ArgumentException("Invalid count: max == 8");
        }

        try
        {
            var client = new RestClient(BingService.Endpoint);
            var request = new RestRequest(BingService.Request, Method.Get)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddQueryParameter("format", "js"); // maybe not needed
            request.AddQueryParameter("idx", "0");
            request.AddQueryParameter("n", count.ToString());
            request.AddQueryParameter("mkt", "en-US");
            RestResponse response = await client.ExecuteAsync(request);
            response.StatusCode.ThrowIfBad();
            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                var responseObject = JsonSerializer.Deserialize<BingPictures>(response.Content);
                if (responseObject is BingPictures bingPictures)
                {
                    var list = new List<Picture>(count);
                    foreach (BingPicture bingPicture in bingPictures.BingPictureList)
                    {
                        list.Add(new Picture(bingPicture)); 
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
        catch (Exception)
        {
            throw; ;
        }
    }
}
