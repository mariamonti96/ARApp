using System;
using UnityEngine;
using UnityEngine.UI;

namespace GuidedCameraCalibrationExample
{
    public class DoneState : GuidedCameraCalibrationExample.CalibrationState
    {
        private const float MESSAGE_DURATION = 3.0f;

        public GuidedCameraCalibrationExample wizard;

        public Text leftValuesText;
        public Text rightValuesText;
        public Text resultText;

        public InputField pathField;
        public Button saveButton;

        private float messageTimer = 0.0f;

        public override void Load()
        {
            pathField.text = "local_storage_dir:/calibration.json";
            wizard.worker.SetCameraCalibrationState("getResults");
            messageTimer = 0.0f;
        }

        public override void Update()
        {
            if (messageTimer > 0)
            {
                messageTimer = Math.Max(0, messageTimer - Time.deltaTime);

                if (messageTimer == 0)
                {
                    resultText.text = "";
                }
            }
        }

        public void RestartButtonClick()
        {
            wizard.worker.SetCameraCalibrationState("reset");
            wizard.State = wizard.collectingState;
        }

        public void SaveButtonClick()
        {
            wizard.worker.SetCameraCalibrationState("write");
        
            if (wizard.worker.WriteCameraCalibration(pathField.text))
            {
                resultText.text = "The camera calibration has been successfully saved";
                resultText.color = Color.green;
                messageTimer = MESSAGE_DURATION;
            }
            else
            {
                resultText.text = "The camera calibration could not be saved";
                resultText.color = Color.red;
                messageTimer = MESSAGE_DURATION;
            }
        }

        public void QuitButtonClick()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private void OnEnable()
        {
            VLWorkerBehaviour.OnCameraCalibrationData += OnCalibDataResult;
        }

        private void OnDisable()
        {
            VLWorkerBehaviour.OnCameraCalibrationData -= OnCalibDataResult;
        }

        private void OnCalibDataResult(VLCameraCalibration calibration)
        {
            leftValuesText.text =
                calibration.intrinsics.width + "\n" +
                calibration.intrinsics.height + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.cx) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.cy) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.fx) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.fy) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.s);
            
            rightValuesText.text =
                string.Format("{0:0.##}", calibration.intrinsics.k1) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.k2) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.k3) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.k4) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.k5) + "\n" +
                string.Format("{0:0.##}", calibration.intrinsics.calibrationError) + "\n" +
                calibration.intrinsics.quality;
            
            if (calibration.calibrated)
            {
                resultText.text = "";
                saveButton.interactable = true;
            }
            else
            {
                resultText.text = "Calibrating went wrong - please restart and try again";
                resultText.color = Color.red;
                saveButton.interactable = false;
            }
        }
    }
}