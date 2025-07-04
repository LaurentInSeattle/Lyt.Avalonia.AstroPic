# AstroPic ~ Image Downloader and Wallpaper App'
Downloads and manages images from various online providers: 

- the NASA Astronomy Pictures of the Day (APOD) 
- Bing Wallpaper of the day 
- Google Earth View images
- NASA Epic (Camera onboard the DSCOVR spacecraft.)
- OpenVerse.Org

Then...
- Set image as Wallpaper (Windows Only for now)
- Rotating wallpapers
- Include your own images into the wallpaper collection.
- Managing the image collection 
- Handle user settings.

Localization: 
- Human translated: Spanish, Italian, French and English
- Machine translated: Ukrainian, Bulgarian, Armenian, Greek, German, Japanese, Chinese, Korean. 
 
<p align="left"><img src="AstroPicScreenshot.png" height="500"/>

# Last Improvements...

- Now showing translated image title and descriptions.
- Image information overlay 
- Localization for additional languages using this translation tool: 
 https://github.com/LaurentInSeattle/Lyt.Avalonia.Translator 
- The localization tool has been integrated in the Visual Studio 2022 build as a "pre-build event".

<p align="left"><img src="AstroPicCollectionScreenshot.png" height="500"/>

# Given up, On hold: 

- Mac OS support. (No machine for testing.)

- Autostart: Tired to struggle with account security and permissions  

<p align="left"><img src="AstroPicSettingsScreenshot.png" height="500"/>

# Build your own...

- Clone this repo'
- => Clone the "Lyt.Framework" repo' side by side
- => Clone the "Lyt.Avalonia" repo' side by side
- Open the solution in Visual Studio and build.

Tested with VS 2022 and Avalonia 11.3.1.
Should likely work with Rider, but not tested.

