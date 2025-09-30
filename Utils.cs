using System;
using System.IO;
using BlueprintCore.Utils;
using UnityEngine;

namespace AviaryClasses;

public static class Utils
{
    private static readonly LogWrapper Logger = LogWrapper.Get("AviaryClasses.Utils");

    public static Sprite LoadIcon(string fileName, Sprite fallback, int size = 128)
    {
        try {
            string imagesDir = Path.Combine(Main.ModEntry.Path, "images");
            string fullPath = Path.Combine(imagesDir, fileName);
            if (!File.Exists(fullPath)) {
                Logger.Error($"Icon file not found: {fullPath}");
                return fallback;
            }

            byte[] data = File.ReadAllBytes(fullPath);
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) {
                name = fileName
            };

            if (!ImageConversion.LoadImage(texture, data)) {
                Logger.Error($"Failed to load texture data: {fullPath}");
                return fallback;
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        } catch (Exception ex) {
            Logger.Error($"Exception loading icon {fileName}", ex);
            return fallback;
        }
    }
}
