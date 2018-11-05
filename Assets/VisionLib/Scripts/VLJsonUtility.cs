/** @addtogroup vlUnitySDK
 *  @{
 */

using UnityEngine;

/// <summary>
///  VisionLib functions for working with JSON data.
/// </summary>
/// <remarks>
///  Right now it's just using the JsonUtility class from UnityEngine.
/// </remarks>
public class VLJsonUtility
{
    public static T FromJson<T>(string json)
    {
        return UnityEngine.JsonUtility.FromJson<T>(json);
    }

    public static void FromJsonOverwrite(string json, object objectToOverwrite)
    {
        UnityEngine.JsonUtility.FromJsonOverwrite(json, objectToOverwrite);
    }

    public static string ToJson(object obj)
    {
        return UnityEngine.JsonUtility.ToJson(obj);
    }

    public static string ToJson(object obj, bool prettyPrint)
    {
        return UnityEngine.JsonUtility.ToJson(obj, prettyPrint);
    }
}

/**@}*/