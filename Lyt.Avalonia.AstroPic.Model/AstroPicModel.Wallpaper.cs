namespace Lyt.Avalonia.AstroPic.Model;

using Lyt.Avalonia.AstroPic.Service;
using System.Xml.Linq;
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
            this.SetWallpaperInfo(pictureMetadata);
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

        List<Picture> pictures = [];
        foreach (var picture in this.Pictures.Values)
        {
            string name = picture.ImageFilePath;
            string path = this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, name);
            if (this.MruWallpapers.Contains(path))
            {
                continue;
            }

            pictures.Add(picture);
        }

        Picture selectedPicture; 
        if (pictures.Count == 1)
        {
            this.MruWallpapers.Clear();
            this.Logger.Info("MRU Wallpapers cleared");
            selectedPicture = pictures[0];
        }
        else
        {
            int index = this.random.Next(pictures.Count);
            selectedPicture = pictures[index];
        }

        string wallpaperPath = 
            this.fileManager.MakePath(Area.User, Kind.BinaryNoExtension, selectedPicture.ImageFilePath);
        this.SetWallpaper(wallpaperPath);
        this.SetWallpaperInfo(selectedPicture.PictureMetadata) ;
    }

    private void SetWallpaperInfo (PictureMetadata pictureMetadata)
    {
        if (!string.IsNullOrWhiteSpace(pictureMetadata.TranslatedTitle))
        {
            this.WallpaperInfo = 
                new WallpaperInfo(
                    pictureMetadata.TranslatedTitle, pictureMetadata.TranslatedDescription);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(pictureMetadata.Title))
            {
                this.WallpaperInfo = 
                    new WallpaperInfo(pictureMetadata.Title, pictureMetadata.Description);
            }
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
