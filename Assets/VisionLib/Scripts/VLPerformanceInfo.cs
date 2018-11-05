/** @addtogroup vlUnitySDK
 *  @{
 */

using System;

/// <summary>
///  VLPerformanceInfo stores information about the tracking performance.
/// </summary>
[Serializable]
public struct VLPerformanceInfo
{
    /// <summary>
    /// The tracking processing time in milliseconds. This excludes the sleep
    /// duration for achieving the target FPS.
    /// </summary>
    public int processingTime;
}
/**@}*/