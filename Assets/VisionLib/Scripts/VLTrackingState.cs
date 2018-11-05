/** @addtogroup vlUnitySDK
 *  @{
 */

using System;

/// <summary>
///  VLTrackingState stores the tracking states of all tracking object.
/// </summary>
[Serializable]
public class VLTrackingState
{
    /// <summary>
    ///  VLTrackedObjectState stores the tracking state of one tracked object
    /// </summary>
    [Serializable]
    public class TrackingObject
    {
        /// <summary>Name of the tracking object.</summary>
        /// <remarks>
        ///  Currently only one tracking object is supported and the name is
        ///  always 'TrackedObject'.
        /// </remarks>
        public string name;

        /// <summary>Tracking state</summary>
        /// <remarks>
        ///  Can be one of the following:
        ///  * "tracked": Object was tracked successful
        ///  * "critical": Object was tracked, but something disturbs the
        ///    tracking (e.g. motion blur or bad illumination). If the tracking
        ///    stays critical for too long, then the state might change to
        ///    "lost".
        ///  * "lost": Object could not be tracked.
        /// </remarks>
        public string state;

        /// <summary>
        ///  Quality value between 0.0 (worst quality) and 1.0 (best quality).
        ///  The concrete meaning depends on the used tracking method.
        /// </summary>
        public float quality;

        public float _InitInlierRatio;
        public int _InitNumOfCorresp;
        public float _TrackingInlierRatio;
        public int _TrackingNumOfCorresp;
        public float _SFHFrameDist;
        public float _Total3DFeatureCount;
        public int _NumberOfTemplates;
		public int _NumberOfTemplatesDynamic;
		public int _NumberOfTemplatesStatic;
		public string _WorldMappingStatus;
        public int _NumberOfLineModels;
        public int _TrackingImageWidth;
        public int _TrackingImageHeight;

        /// <summary>
        ///  The timestamp in seconds from 1.1.1970 and parts of the seconds as fraction.
        ///  Describes, when the process of the image has been started.
        /// </summary>
        public double timeStamp;
    }

    /// <summary>
    ///  Array with the tracking state of all tracking objects.
    /// </summary>
    public TrackingObject[] objects;
}
/**@}*/