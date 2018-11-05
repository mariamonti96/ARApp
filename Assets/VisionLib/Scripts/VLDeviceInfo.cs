/** @addtogroup vlUnitySDK
 *  @{
 */

using System;

/// <summary>
///  VLDeviceInfo stores information about the system and available cameras. 
/// </summary>
[Serializable]
public class VLDeviceInfo
{
    /// <summary>
    ///  TrackingIssue stores an issue when tracking or when initialized
    /// </summary>
    [Serializable]
    public class Camera
    {

        /// <summary>
        ///  Format stores available formats of the camera device
        /// </summary>
        [Serializable]
        public class Format
        {
            /// <summary>
            ///  number of pixels in horizontal direction
            /// </summary>
            /// <remarks>
            ///  default is 640 (VGA)
            /// </remarks>
            public int width;

            /// <summary>
            ///  number of pixels in vertical direction
            /// </summary>
            /// <remarks>
            ///  default is 480 (VGA)
            /// </remarks>
            public int height;

            /// <summary>
            ///  compression format (e.g. YUY2)
            /// </summary>
            public string compression;

        }

        /// <summary>Refers to a specific camera of the visionLib SDK.</summary>
        /// <remarks>
        ///  You should use it for direct referring to a camera in some cases.
        /// </remarks>
        public string deviceID;

        /// <summary>Refers to an internal reference used for identifying the camera in the system.</summary>
        /// <remarks>
        ///  This is only for internal use.
        /// </remarks>
        public string internalID;

        /// <summary>A human readable information of the device itself.</summary>
        /// <remarks>
        ///  You might use this in order to present this to the user of your software.
        /// </remarks>
        public string cameraName;

        /// <summary>A hint for describing the position of the camera.</summary>
        /// <remarks>
        ///  If available, the value can be "front", "back", "unknown".
        /// </remarks>
        public string position;

        /// <summary>A hint for describing the best resolution used for the camera.</summary>
        /// <remarks>
        ///  This might be empty, depending on the OS.
        /// </remarks>
        public string prefRes;

        /// <summary>
        ///  Array holding the available formats: image resolution and compression type.
        /// </summary>
        public Format[] availableFormats;


    }

    /// <summary>Platform that we the vlSDK is working on.</summary>
    /// <remarks>
    ///  Can be: "iOS", "macOS", "Android", "Windows", "UWP"
    /// </remarks>
    public string os;

    /// <summary>The manufacture of the device.</summary>
    /// <remarks>
    ///  If available. (e.g "Apple", "Samsung" ....)
    /// </remarks>
    public string manufacture;

    /// <summary>The model.</summary>
    /// <remarks>
    ///  If available. (e.g "iPhone", "iPad" ....)
    /// </remarks>
    public string model;

    /// <summary>The model.</summary>
    /// <remarks>
    ///  If available. (e.g "3", "4" ....)
    /// </remarks>
    public string modelVersion;

    /// <summary>A unified ID.</summary>
    /// <remarks>
    ///  If available. Note, that on mobile devices these kind of identifiers are NEVER available.
    /// </remarks>
    public string unifiedID;

    /// <summary>A device hardware descriptor.</summary>
    /// <remarks>
    ///  If available. (e.g. iPhone6,1)
    /// </remarks>
    public string internalModelID;

    /// <summary>The bundle identifier used.</summary>
    /// <remarks>
    ///  If available. (e.g. de.frauhofer.igd.vrar.superapp)
    /// </remarks>
    public string appID;

    /// <summary>The number of processors used by the vlSDK.</summary>
    /// <remarks>
    ///  If available. 
    /// </remarks>
    public int numberOfProcessors;

    /// <summary>Horizontal native resolution of the device.</summary>
    /// <remarks>
    ///  If available. 
    /// </remarks>
    public int nativeResX;

    /// <summary>Vertical native resolution of the device.</summary>
    /// <remarks>
    ///  If available. 
    /// </remarks>
    public int nativeResY;

    /// <summary>The current orientation in visionLib SDK system.</summary>
    /// <remarks>
    ///  If available. 
    /// </remarks>
    public int currentDisplayOrientation;

    /// <summary>If an internal eventlogger system is user.</summary>
    /// <remarks>
    ///  Not available. 
    /// </remarks>
    public bool usingEventLogger;

    /// <summary>If the user has allowed the use of the camera.</summary>
    /// <remarks>
    ///  Not available. 
    /// </remarks>
    public bool cameraAllowed;

    /// <summary>
    ///  Array holding the available camera devices.
    /// </summary>
    public Camera[] availableCameras;


}
/**@}*/