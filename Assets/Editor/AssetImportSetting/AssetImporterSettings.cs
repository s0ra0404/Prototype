using UnityEditor;
using UnityEngine;

public class RuleMaskImportSetting : AssetPostprocessor
{
    // 画像がインポートされる直前に呼ばれる
    private void OnPreprocessTexture()
    {
        var importer = (TextureImporter)assetImporter;

        // 対象を特定のパスに絞りたい場合はコメントアウトを外して下さい
        if (!assetPath.Contains("/RuleMask/")) return;

        // テクスチャタイプを Sprite に
        importer.textureType = TextureImporterType.Default;
        importer.textureShape = TextureImporterShape.Texture2D;
        importer.sRGBTexture = true;
        importer.alphaSource = TextureImporterAlphaSource.FromGrayScale;
        importer.alphaIsTransparency = false;
        
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Bilinear;
        importer.anisoLevel = 1;
        
        var settings = importer.GetDefaultPlatformTextureSettings(); 
        settings.maxTextureSize = 2048;
        settings.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
        settings.format = TextureImporterFormat.Automatic;
        settings.textureCompression = TextureImporterCompression.Compressed;
        settings.crunchedCompression = false;
        
        importer.SetPlatformTextureSettings(settings);
    }
}