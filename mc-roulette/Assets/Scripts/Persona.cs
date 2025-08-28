using System;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 玩家个性化皮肤
/// </summary>
public class Persona
{
    #region Public 方法
    public static Texture2D LoadDefaultSkin(DefaultSkin defaultSkin)
    {
        string path = string.Empty;
        switch (defaultSkin)
        {
            case DefaultSkin.Steve:
                path = "Textures/Steve";
                break;
            case DefaultSkin.Alex:
                path = "Textures/Alex";
                break;
        }
        Texture2D texture = Addressables.LoadAssetAsync<Texture2D>(path).WaitForCompletion();
        return texture;
    }

    /// <summary>
    /// 加载外部PNG格式的图片为玩家皮肤的<see cref="Texture2D"/>资源
    /// </summary>
    /// <param name="data">外部PNG格式的图片的二进制数据</param>
    public static Texture2D LoadSkinFromExternalPNG(byte[] data)
    {
        Texture2D raw = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        raw.filterMode = FilterMode.Point;
        raw.wrapMode = TextureWrapMode.Clamp;

        raw.LoadImage(data);

        // 满足尺寸要求（64*64、64*32或128*128），可直接作为玩家皮肤纹理。
        if ((raw.width == 64 && raw.height == 64) || (raw.width == 64 && raw.height == 32) || (raw.width == 128 && raw.height == 128))
        {
            raw.Apply();
            return raw;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 将尺寸为64*32的皮肤纹理转换到64*64的尺寸
    /// </summary>
    /// <param name="raw">原始皮肤（64*32尺寸）</param>
    /// <param name="customSkinBodyType">自定义皮肤的体型</param>
    public static Texture2D Cast6432To6464(Texture2D raw, CustomSkinBodyType customSkinBodyType)  
    {
        if (raw == null || !(raw.width == 64 && raw.height == 32)) 
        {
            Debug.LogError("原始皮肤纹理为null或其尺寸不符合要求（64*32）！");
            return null; 
        }

        Texture2D texture = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // 初始化纹理的所有像素为透明色
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, new Color(0, 0, 0, 0f));
            }
        }

        // 将64*32的源皮肤纹理映射的原有像素绘制到64*64纹理上
        for (int x = 0; x < raw.width; x++)
        {
            for (int y = 0; y < raw.height; y++)
            {
                Color color = raw.GetPixel(x, y);
                texture.SetPixel(x, 32 + y, color);
            }
        }

        switch (customSkinBodyType)
        {
            case CustomSkinBodyType.Bold:
                Cast6432To6464Bold(ref texture, ref raw);
                break;

            case CustomSkinBodyType.Slim:
                Cast6432To6464Slim(ref texture, ref raw);
                break;
        }

        texture.Apply();
        return texture;
    }
    #endregion

    #region Private 方法
    /// <summary>
    /// 将64*32的皮肤纹理转换为64*64的皮肤纹理（Bold样式）
    /// </summary>
    private static void Cast6432To6464Bold(ref Texture2D target, ref Texture2D source)
    {
        #region 手臂
        // Top (source: x44y12~x47y15, target: x39y12~x36y15)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Color color = source.GetPixel(44 + x, 12 + y);
                target.SetPixel(39 - x, 12 + y, color);
            }
        }

        // Bottom (source: x48y12~x51y15, target: x43y12~x40y15)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Color color = source.GetPixel(48 + x, 12 + y);
                target.SetPixel(43 - x, 12 + y, color);
            }
        }

        // Front (source: x44y0~x47y11, target: x39y0~x36y11)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(44 + x, 0 + y);
                target.SetPixel(39 - x, 0 + y, color);
            }
        }

        // Back (source: x52y0~x55y11, target: x47y0~x44y11)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(52 + x, 0 + y);
                target.SetPixel(47 - x, 0 + y, color);
            }
        }

        // left source: x40y0~x43y11, right target: x43y0~x40y11)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(40 + x, 0 + y);
                target.SetPixel(43 - x, 0 + y, color);
            }
        }

        // right source: x48y0~x51y11, left target: x35y0~x32y11
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(48 + x, 0 + y);
                target.SetPixel(35 - x, 0 + y, color);
            }
        }
        #endregion

        #region 腿部
        // Top (source: x4y12~x7y15, target: x23y12~x20y15)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Color color = source.GetPixel(4 + x, 12 + y);
                target.SetPixel(23 - x, 12 + y, color);
            }
        }

        // Bottom (source: x8y12~x11y15, target: x27y12~x24y15)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Color color = source.GetPixel(8 + x, 12 + y);
                target.SetPixel(27 - x, 12 + y, color);
            }
        }

        // Front (source: x4y0~x7y11, target: x23y0~x20y11)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(4 + x, 0 + y);
                target.SetPixel(23 - x, 0 + y, color);
            }
        }

        // Back (source: x12y0~x15y11, target: x31y0~x29y11)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(12 + x, 0 + y);
                target.SetPixel(31 - x, 0 + y, color);
            }
        }

        // left source: x0y0~x3y11, right target: x27y0~x24y11)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(0 + x, 0 + y);
                target.SetPixel(27 - x, 0 + y, color);
            }
        }

        // right source: x8y0~x11y11, left target: x19y0~x16y11)
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                Color color = source.GetPixel(8 + x, 0 + y);
                target.SetPixel(19 - x, 0 + y, color);
            }
        }
        #endregion
    }

    private static void Cast6432To6464Slim(ref Texture2D target, ref Texture2D source) 
    {

    }
    #endregion

    /// <summary>
    /// 自定义皮肤的体型
    /// </summary>
    [Serializable]
    public enum CustomSkinBodyType
    {
        /// <summary>
        /// 较宽手臂样式（Steve）
        /// </summary>
        Bold
            ,
        /// <summary>
        /// 较细手臂样式（Alex）
        /// </summary>
        Slim
    }

    /// <summary>
    /// 默认皮肤
    /// </summary>
    public enum DefaultSkin 
    {
        Steve,
        Alex
    }
}