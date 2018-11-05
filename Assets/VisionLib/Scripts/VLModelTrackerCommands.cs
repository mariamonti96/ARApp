/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using VLWorkerCommands;

/// <summary>
///  Namespace with command classes for communicating with the model-based
///  tracking.
/// </summary>
namespace VLModelTrackerCommands
{
    /// <summary>
    ///  Resets the tracking.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   NOTICE: Deprecated. Use <see cref="ResetSoftCmd"/> or
    ///   <see cref="ResetHardCmd"/> instead.
    ///  </para>
    /// </remarks>
    [Serializable]
    public class ResetTrackingCmd : CommandBase
    {
        private static readonly string defaultName = "resetTracking";
        public ResetTrackingCmd() : base(defaultName) {}
    }

    /// <summary>
    ///  Resets the tracking.
    /// </summary>
    [Serializable]
    public class ResetSoftCmd : CommandBase
    {
        private static readonly string defaultName = "resetSoft";
        public ResetSoftCmd() : base(defaultName) {}
    }

    /// <summary>
    ///  Resets the tracking and all keyframes.
    /// </summary>
    [Serializable]
    public class ResetHardCmd : CommandBase
    {
        private static readonly string defaultName = "resetHard";
        public ResetHardCmd() : base(defaultName) {}
    }

    /// <summary>
    ///  Get the initial pose.
    /// </summary>
    [Serializable]
    public class GetInitPoseCmd : CommandBase
    {
        private static readonly string defaultName = "getInitPose";
        public GetInitPoseCmd() : base(defaultName) {}
    }

    /// <summary>
    ///  Result of GetInitPoseCmd.
    /// </summary>
    [Serializable]
    public struct GetInitPoseResult
    {
        public float[] t;
        public float[] q;
    }

    /// <summary>
    ///  Set the initial pose.
    /// </summary>
    [Serializable]
    public class SetInitPoseCmd : CommandBase
    {
        private static readonly string defaultName = "setInitPose";

        [Serializable]
        public class Param
        {
            public float[] t;
            public float[] q;
            public Param(float tx, float ty, float tz,
                float qx, float qy, float qz, float qw)
            {
                this.t = new float[3] {
                    tx,
                    ty,
                    tz
                };
                this.q = new float[4] {
                    qx,
                    qy,
                    qz,
                    qw
                };
            }
        }

        public Param param;
        public SetInitPoseCmd(Param param) : base(defaultName)
        {
            this.param = param;
        }
    }

    /// <summary>
    ///  Write init data to default location with default name.
    /// </summary>
    [Serializable]
    public class WriteInitDataCmd : CommandBase
    {
        private static readonly string defaultName = "writeInitData";
        public WriteInitDataCmd() : base(defaultName) {}
    }

    /// <summary>
    ///  Write init data to custom location with custom file name.
    /// </summary>
    [Serializable]
    public class WriteInitDataWithPrefixCmd : CommandBase
    {
        private static readonly string defaultName = "writeInitData";
        public string param;
        public WriteInitDataWithPrefixCmd(string filePrefix) : base(defaultName)
        {
            this.param = filePrefix;
        }
    }

    /// <summary>
    ///  Read init data from custom location with custom file name.
    /// </summary>
    [Serializable]
    public class ReadInitDataWithPrefixCmd : CommandBase
    {
        private static readonly string defaultName = "readInitData";
        public string param;
        public ReadInitDataWithPrefixCmd(string filePrefix) : base(defaultName)
        {
            this.param = filePrefix;
        }
    }

     /// <summary>
    ///  Reset Offline init data 
    /// </summary>
    [Serializable]
    public class ResetInitDataCmd : CommandBase
    {
        private static readonly string defaultName = "resetInitData";
        public ResetInitDataCmd() : base(defaultName) {}
    }
} // namespace VLModelTrackerCommands

/**@}*/