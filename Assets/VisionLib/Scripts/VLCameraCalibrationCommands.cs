/** @addtogroup vlUnitySDK
 *  @{
 */

using System;
using VLWorkerCommands;

/// <summary>
///  C# Unity Namespace with command classes for communicating with the camera calibration
///  inside the tracking thread.
/// </summary>
namespace VLCameraCalibrationCommands
{
	/// <summary>
	///  Starts the collection of frames.
	/// </summary>
	[Serializable]
	class CameraCalibrationCmd : CommandBase
	{
		public CameraCalibrationCmd(string name) : base(name) {}
	}

	[Serializable]
	class WriteCameraCalibrationCmd : CommandBase
	{
		public string uri;
		public WriteCameraCalibrationCmd(string uri) : base("write") {
			this.uri = uri;
		}
	}


} // namespace VLCameraCalibrationCommands

/**@}*/