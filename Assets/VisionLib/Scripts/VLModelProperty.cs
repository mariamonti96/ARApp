/** @addtogroup vlUnitySDK
 *  @{
 */

using System;

/// <summary>
///  VLModelProperties stores the internal states of the model which is used for model based tracking.
/// Such an object is usually passed, when subscribing to the OnGetModelProperties event.
/// </summary>
[Serializable]
public class VLModelProperties
{
   
    /// <summary>
    /// The internal id or name used for adressing the model (e.g. SetModelProperty)
    /// A name for setting the property should always carry an uri prefix scheme: 'name:'
    /// </summary>
    public string name;

    /// <summary>
    /// The model hash code used for internally identifying the validity along with the license.
    /// It is usually part of the license file, and can be used for creation of a valid license for the model.
    /// </summary>
    public string modelHash;

    /// <summary>
    /// The uri that has been used to load the model.
    /// </summary>
    public string uri;

    /// <summary>
    /// States if the model is currently enabled.
    /// </summary>
    public bool enabled;

    /// <summary>
    /// States if the model is used for occlusion.
    /// </summary>
    public bool occluder;

    /// <summary>
    /// The number of triangles in the whole model, used by the visionLib.
    /// </summary>
    public bool triangleCount;

    /// <summary>
    /// The number of subMeshes used by the loaded model.
    /// </summary>
    public bool subMeshCount;

}

/// <summary>
///  VLModelProperties stores the internal states of the model which is used for model based tracking.
/// Such an object is usually passed, when subscribing to the OnGetModelProperties event.
/// </summary>
[Serializable]
public class VLModelPropertiesStructure
{

	public VLModelProperties [] info;
}

/**@}*/
