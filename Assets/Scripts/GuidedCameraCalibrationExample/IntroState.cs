using UnityEngine;

namespace GuidedCameraCalibrationExample
{
    public class IntroState : GuidedCameraCalibrationExample.CalibrationState
    {
        public GuidedCameraCalibrationExample wizard;

        public void ExitButtonClick()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void ProceedButtonClick()
        {
            wizard.State = wizard.imageState;
        }
    }
}