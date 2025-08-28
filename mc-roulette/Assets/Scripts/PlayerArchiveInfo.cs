using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DefaultSkin = Persona.DefaultSkin;
using CustomSkinBodyType = Persona.CustomSkinBodyType;

/// <summary>
/// 玩家存档信息
/// </summary>
[Serializable]
public class PlayerArchiveInfo
{
    private string name = "Steve";

    private bool isUsingCustomSkin = false;
    private byte[] customSkinData = null;
    private CustomSkinBodyType customSkinBodyType = CustomSkinBodyType.Bold;
    private DefaultSkin defaultSkin = DefaultSkin.Steve;

    private Texture2D _skinTexture;

    #region 属性
    /// <summary>
    /// 玩家名称
    /// </summary>
    public string Name => name;

    /// <summary>
    /// 玩家是否使用自定义皮肤？
    /// </summary>
    public bool IsUsingCustomSkin => isUsingCustomSkin;

    /// <summary>
    /// 玩家自定义皮肤的体型类型
    /// </summary>
    public CustomSkinBodyType CustomSkinBodyType => customSkinBodyType;

    /// <summary>
    /// 基于玩家自定义皮肤的头像纹理
    /// </summary>
    public Texture2D AvatarTexture 
    {
        get 
        {
            return _skinTexture;
        }
    }

    /// <summary>
    /// 玩家自定义皮肤纹理
    /// </summary>
    public Texture2D SkinTexture
    {
        get
        {
            if (_skinTexture == null)
            {
                if (isUsingCustomSkin && customSkinData != null && customSkinData.Length > 0)
                {
                    _skinTexture = Persona.LoadSkinFromExternalPNG(customSkinData);
                }
                else
                {
                    isUsingCustomSkin = false;
                    _skinTexture = Persona.LoadDefaultSkin(defaultSkin);
                }
            }

            return _skinTexture;
        }
    }
    #endregion

    #region Public 方法
    public static PlayerArchiveInfo LoadFromJson(string json)
    {
        return JsonConvert.DeserializeObject<PlayerArchiveInfo>(json, new MyJsonConverter());
    }

    public string ToJson() 
    {
        var jsonSettings = new JsonSerializerSettings
        {
            Converters = { new MyJsonConverter() },
            Formatting = Formatting.Indented
        };

        return JsonConvert.SerializeObject(this, jsonSettings);
    }

    public void SetCustomSkin(Texture2D skin, CustomSkinBodyType bodyType)
    {
        if ((skin.width == 64 && skin.height == 64) || (skin.width == 128 && skin.height == 128))
        {
            isUsingCustomSkin = true;
            customSkinBodyType = bodyType;
            customSkinData = skin.EncodeToPNG();

            _skinTexture = skin;
        }
        else
        {
            Debug.LogError("自定义皮肤的尺寸不符合要求（64*64或128*128）！");
        }
    }
    #endregion

    #region 内部类型
    private class MyJsonConverter : JsonConverter<PlayerArchiveInfo>
    {
        public override PlayerArchiveInfo ReadJson(JsonReader reader, Type objectType, PlayerArchiveInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            PlayerArchiveInfo archive = new PlayerArchiveInfo();

            archive.name = obj["name"].Value<string>() ?? "Steve";

            #region 个性化皮肤
            var persona = obj["persona"].Value<JObject>();
            if(persona == null)
            {
                archive.isUsingCustomSkin = false;
                archive.customSkinData = null;
                archive.customSkinBodyType = CustomSkinBodyType.Bold;
                archive.defaultSkin = DefaultSkin.Steve;
            }
            else 
            {
                archive.isUsingCustomSkin = persona.ContainsKey("using-custom-skin") ? persona["using-custom-skin"].ToObject<bool>() : false;
                archive.customSkinBodyType = CustomSkinBodyType.Bold;

                if (archive.isUsingCustomSkin)
                {
                    archive.customSkinBodyType = persona.ContainsKey("body-type") ? 
                        (CustomSkinBodyType)persona["body-type"].ToObject<int>() : CustomSkinBodyType.Bold;

                    if (persona.ContainsKey("data"))
                    {
                        archive.customSkinData = persona["data"].ToObject<byte[]>();
                    }
                    else
                    {
                        archive.isUsingCustomSkin = false;
                        archive.customSkinData = null;
                        archive.customSkinBodyType = CustomSkinBodyType.Bold;
                        archive.defaultSkin = DefaultSkin.Steve;
                    }
                }
                else
                {
                    archive.customSkinData = null;
                    archive.defaultSkin = persona.ContainsKey("default") ?
                        (DefaultSkin)persona["default"].ToObject<int>() :
                        DefaultSkin.Steve;
                }
            }
            #endregion

            return archive;
        }

        public override void WriteJson(JsonWriter writer, PlayerArchiveInfo value, JsonSerializer serializer)
        {
            #region 个性化皮肤
            JObject persona = new JObject
            {
                { "using-custom-skin", value.isUsingCustomSkin },
            };

            if (value.isUsingCustomSkin)
            {
                persona.Add("data", value.customSkinData);
                persona.Add("body-type", (int)value.customSkinBodyType);
            }
            else
            {
                persona.Add("default", (int)value.defaultSkin);
            }
            #endregion

            JObject obj = new JObject
            {
                { "name", value.name },
                { "persona", persona }
            };

            obj.WriteTo(writer);
        }
    }
    #endregion
}