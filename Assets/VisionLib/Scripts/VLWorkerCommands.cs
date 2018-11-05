/** @addtogroup vlUnitySDK
 *  @{
 */

using System;

/// <summary>
///  C# Unity Namespace with command classes for communicating with the tracking thread.
/// </summary>
/// <remarks>
///  The communication with the tracking thread happens primarily through the
///  VLWorker class using certain commands. By using the classes from the
///  VLWorkerCommands namespace, one can ensure the correct format.
/// </remarks>
namespace VLWorkerCommands
{
    [Serializable]
    public class CommandBase
    {
        public string name;
        public CommandBase(string name)
        {
            this.name = name;
        }
    }

    /// <summary>
    ///  Creates a tracker from a vl-file.
    /// </summary>
    [Serializable]
    public class CreateTrackerCmd : CommandBase
    {
        private static readonly string defaultName = "createTracker";
        public string param;
        public CreateTrackerCmd(string trackingFile) : base(defaultName)
        {
            this.param = trackingFile;
        }
    }

    /// <summary>
    ///  Creates a tracker from a vl-string.
    /// </summary>
    [Serializable]
    public class CreateTrackerFromStringCmd : CommandBase
    {
        private static readonly string defaultName = "createTrackerFromString";

        [Serializable]
        public class Param
        {
            public string str;
            public string fakeFilename;
            public Param(string str, string fakeFilename)
            {
                this.str = str;
                this.fakeFilename = fakeFilename;
            }
        }

        public Param param;
        public CreateTrackerFromStringCmd(Param param) : base(defaultName)
        {
            this.param = param;
        }
    }

    /// <summary>
    ///  Creates a line tracker from a file.
    /// </summary>
    [Serializable]
    public class CreateLineTrackerCmd : CommandBase
    {
        private static readonly string defaultName = "createLineTracker";
        public string param;
        public CreateLineTrackerCmd(string trackingFile) : base(defaultName)
        {
            this.param = trackingFile;
        }
    }

    /// <summary>
    ///  Creates a line tracker from a string.
    /// </summary>
    [Serializable]
    public class CreateLineTrackerFromStringCmd : CommandBase
    {
        private static readonly string defaultName = "createLineTrackerFromString";

        [Serializable]
        public class Param
        {
            public string str;
            public string fakeFilename;
            public Param(string str, string fakeFilename)
            {
                this.str = str;
                this.fakeFilename = fakeFilename;
            }
        }

        public Param param;
        public CreateLineTrackerFromStringCmd(Param param) : base(defaultName)
        {
            this.param = param;
        }
    }

    /// <summary>
    ///  Creates a (binary) marker tracker from a file.
    /// </summary>
    [Serializable]
    public class CreateMarkerTrackerCmd : CommandBase
    {
        private static readonly string defaultName = "createMarkerTracker";
        public string param;
        public CreateMarkerTrackerCmd(string trackingFile) : base(defaultName)
        {
            this.param = trackingFile;
        }
    }

    /// <summary>
    ///  Creates a (binary) marker tracker from a string.
    /// </summary>
    [Serializable]
    public class CreateMarkerTrackerFromStringCmd : CommandBase
    {
        private static readonly string defaultName = "createMarkerTrackerFromString";

        [Serializable]
        public class Param
        {
            public string str;
            public string fakeFilename;
            public Param(string str, string fakeFilename)
            {
                this.str = str;
                this.fakeFilename = fakeFilename;
            }
        }

        public Param param;
        public CreateMarkerTrackerFromStringCmd(Param param) : base(defaultName)
        {
            this.param = param;
        }
    }

    /// <summary>
    ///  Creates a poster tracker from a file.
    /// </summary>
    [Serializable]
    public class CreatePosterTrackerCmd : CommandBase
    {
        private static readonly string defaultName = "createPosterTracker";
        public string param;
        public CreatePosterTrackerCmd(string trackingFile) : base(defaultName)
        {
            this.param = trackingFile;
        }
    }

    /// <summary>
    ///  Creates a poster tracker from a string.
    /// </summary>
    [Serializable]
    public class CreatePosterTrackerFromStringCmd : CommandBase
    {
        private static readonly string defaultName = "createPosterTrackerFromString";

        [Serializable]
        public class Param
        {
            public string str;
            public string fakeFilename;
            public Param(string str, string fakeFilename)
            {
                this.str = str;
                this.fakeFilename = fakeFilename;
            }
        }

        public Param param;
        public CreatePosterTrackerFromStringCmd(Param param) : base(defaultName)
        {
            this.param = param;
        }
    }

    /// <summary>
    ///  Sets the target number of frames per seconds of the tracking thread.
    /// </summary>
    [Serializable]
    public class SetTargetFpsCmd : CommandBase
    {
        private static readonly string defaultName = "setTargetFPS";
        public int param;
        public SetTargetFpsCmd(int fps) : base(defaultName)
        {
            this.param = fps;
        }
    }

    /// <summary>
    ///  Gets the current value of a certain attribute.
    /// </summary>
    [Serializable]
    public class GetAttributeCmd : CommandBase
    {
        private static readonly string defaultName = "getAttribute";
        public string param;
        public GetAttributeCmd(string attributeName) : base(defaultName)
        {
            this.param = attributeName;
        }
    }

    /// <summary>
    ///  Sets the value of a certain attribute.
    /// </summary>
    [Serializable]
    public class SetAttributeCmd : CommandBase
    {
        private static readonly string defaultName = "setAttribute";

        [Serializable]
        public struct Param
        {
            public string att;
            public string val;
            public Param(string attributeName, string attributeValue)
            {
                this.att = attributeName;
                this.val = attributeValue;
            }
        }

        public Param param;
        public SetAttributeCmd(Param param) : base(defaultName)
        {
            this.param = param;
        }
    }

    /// <summary>
    ///  Starts the tracking.
    /// </summary>
    [Serializable]
    public class RunTrackingCmd : CommandBase
    {
        private static readonly string defaultName = "runTracking";
        public RunTrackingCmd() : base(defaultName) {}
    }

    /// <summary>
    ///  Stops the tracking.
    /// </summary>
    [Serializable]
    public class PauseTrackingCmd : CommandBase
    {
        private static readonly string defaultName = "pauseTracking";
        public PauseTrackingCmd() : base(defaultName) {}
    }

    /// <summary>
    ///  Runs the tracking once.
    /// </summary>
    [Serializable]
    public class RunTrackingOnceCmd : CommandBase
    {
        private static readonly string defaultName = "runTrackingOnce";
        public RunTrackingOnceCmd() : base(defaultName) {}
    }

    // Return types

    /// <summary>
    ///  Error returned from VLWorker.JsonStringCallback.
    /// </summary>
    [Serializable]
    public struct CommandError
    {
        public string message;
    }

    /// <summary>
    ///  Result of GetAttributeCmd.
    /// </summary>
    [Serializable]
    public struct GetAttributeResult
    {
        public string value;
    }

    [Serializable]
    public class SetModelBoolPropertyCmd : CommandBase
    {
        private static readonly string defaultName = "setModelProperty";

        [Serializable]
        public struct Param
        {
            public string name;
            public string property;
            public bool value;
            public Param(string name, string property, bool enable)
            {
                this.name = name;
                this.property = property;
                this.value = enable;
            }
        }

        public Param param;
        public SetModelBoolPropertyCmd(string name, string property, bool enable) : base(defaultName)
        {
            this.param = new Param(name, property, enable);
        }
    }

    [Serializable]
    public class GetModelPropertiesCmd : CommandBase
    {
        private static readonly string defaultName = "getModelProperties";

        public GetModelPropertiesCmd() : base(defaultName) {}
    }
} // namespace VLWorkerCommands

/**@}*/