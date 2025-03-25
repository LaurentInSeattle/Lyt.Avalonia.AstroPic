namespace Lyt.Avalonia.AstroPic.Service.EarthView;

// See: https://github.com/varietywalls/variety/blob/master/variety/plugins/builtin/downloaders/EarthviewDownloader.py 

internal class EarthViewService
{
    // DATA_URL = "https://new-images-preview-dot-earth-viewer.appspot.com/_api/photos.json"
    // ROOT_URL = "https://new-images-preview-dot-earth-viewer.appspot.com/"; 

    /*
    def download_queue_item(self, item):
            item = Util.fetch_json("https://new-images-preview-dot-earth-viewer.appspot.com/_api/" + item["slug"] + ".json")
            region = item["region"]
            filename = "{}{} (ID-{}).jpg".format(
                region + ", " if region and region != "-" else "", item["country"], item["id"]
            )
            origin_url = EarthviewDownloader.ROOT_URL + str(item["slug"])
            image_url = item["photoUrl"]
            if not image_url.startswith("http"):
                image_url = "https://" + image_url

            extra_metadata = {"description": item.get("name"), "author": item.get("attribution")}
            return self.save_locally(
                origin_url, image_url, local_filename=filename, extra_metadata=extra_metadata
            )
    */
}