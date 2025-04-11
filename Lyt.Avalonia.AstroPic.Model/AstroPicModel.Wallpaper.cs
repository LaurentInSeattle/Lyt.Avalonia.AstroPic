namespace Lyt.Avalonia.AstroPic.Model;

using static FileManagerModel;

public sealed partial class AstroPicModel : ModelBase
{
    public void SetWallpaper(PictureMetadata pictureMetadata, byte[] imageBytes)
    {
        ProviderKey provider = pictureMetadata.Provider;
        try
        {
            string name = provider.ToString();
            var fileId = new FileId(Area.User, Kind.BinaryNoExtension, name);
            if (this.fileManager.Exists(fileId))
            {
                this.fileManager.Delete(fileId);
            }

            string path = this.fileManager.MakePath(fileId);
            this.fileManager.Save<byte[]>(fileId, imageBytes);
            this.SetWallpaper(path);
            this.fileManager.Delete(fileId);
        }
        catch (Exception ex)
        {
            this.Logger.Warning(
                "Failed to set wallpaper " + provider.ToString() + "\n" + ex.ToString());
        }
    }

    public void RotateWallpaper()
    {
        if (this.Pictures.Values.Count == 0)
        {
            // No data, most likely first run
            return;
        }

        // Pickup a new wallpaper not present in the MRU list
        if (this.MruWallpapers.Count == this.Pictures.Values.Count)
        {
            this.Logger.Info("MRU Wallpapers cleared");
            this.MruWallpapers.Clear();
        }

        List<string> files = [];
        foreach (var picture in this.Pictures.Values)
        {
            string name = picture.ImageFilePath;
            string path = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, name);
            if (this.MruWallpapers.Contains(path))
            {
                continue;
            }

            files.Add(path);
        }

        if (files.Count == 1)
        {
            this.MruWallpapers.Clear();
            this.Logger.Info("MRU Wallpapers cleared");
            this.SetWallpaper(files[0]);
        }
        else
        {
            int index = this.random.Next(files.Count);
            this.SetWallpaper(files[index]);
        }
    }

    private void SetWallpaper(string path)
    {
        try
        {
            if (!Path.Exists(path))
            {
                throw new Exception("Does not exists.");
            }

            this.MruWallpapers.Add(path);
            this.wallpaperService.Set(path, WallpaperStyle.Fill);
            this.Logger.Info("Wallpaper set: " + path);
        }
        catch (Exception ex)
        {
            this.Logger.Warning(
                "Failed to set wallpaper: " + path + "\n" + ex.ToString());
        }
    }
}
