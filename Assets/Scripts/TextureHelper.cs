using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureHelper
{

    public enum TextureMapType
    {
        None,
        Albedo,
        Normal,
        Displacement,
        Roughness,
        Metallic,
        Occlusion,
        Emission,
        Color
    }

    private TextureMapType ToTextureMapType(string fileName)
    {
        if (fileName.Contains("Color"))
        {
            return TextureMapType.Albedo;
        }
        if (fileName.Contains("Albedo"))
        {
            return TextureMapType.Albedo;
        }
        else if (fileName.Contains("Normal"))
        {
            return TextureMapType.Normal;
        }
        else if (fileName.Contains("Displacement"))
        {
            return TextureMapType.Displacement;
        }
        else if (fileName.Contains("Roughness"))
        {
            return TextureMapType.Roughness;
        }
        else if (fileName.Contains("Metallic"))
        {
            return TextureMapType.Metallic;
        }
        else if (fileName.Contains("Occlusion"))
        {
            return TextureMapType.Occlusion;
        }
        else if (fileName.Contains("Emission"))
        {
            return TextureMapType.Emission;
        }
        else
        {
            return TextureMapType.None;
        }
    }

    private string ToTextureName(TextureMapType mapType)
    {
        switch (mapType)
        {
            case TextureMapType.Albedo:
                return "_MainTex";
            case TextureMapType.Normal:
                return "_BumpMap";
            case TextureMapType.Displacement:
                return "_ParallaxMap";
            case TextureMapType.Roughness:
                return "_SpecGlossMap";
            case TextureMapType.Metallic:
                return "_MetallicGlossMap";
            case TextureMapType.Occlusion:
                return "_OcclusionMap";
            case TextureMapType.Emission:
                return "_EmissionMap";
            default:
                return null;
        }
    }

    public string getTextureKey(string fileName)
    {
        return ToTextureName(ToTextureMapType(fileName));
    }
}
