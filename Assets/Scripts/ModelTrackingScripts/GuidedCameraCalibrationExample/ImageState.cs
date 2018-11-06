using UnityEngine;

namespace GuidedCameraCalibrationExample
{
    public class ImageState : GuidedCameraCalibrationExample.CalibrationState
    {
        public GuidedCameraCalibrationExample wizard;

        public void BackButtonClick()
        {
            wizard.State = wizard.introState;
        }

        public void ProceedButtonClick()
        {
            wizard.State = wizard.deviceState;
        }
    }
}