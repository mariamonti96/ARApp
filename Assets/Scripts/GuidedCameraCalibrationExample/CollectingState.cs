using System;
using UnityEngine;
using UnityEngine.UI;

namespace GuidedCameraCalibrationExample
{
    public class CollectingState : GuidedCameraCalibrationExample.CalibrationState
    {
        private const float TIME_TO_CALIBRATE = 100.0f;
        private const float MAX_SPEED = 10.0f;
        private const float FREEZE_TIME = 3.0f;

        public GuidedCameraCalibrationExample wizard;

        public Camera trackingCamera;
        public Text instructionText;
        public Text stepText;
        public Text templatesText;
        public Text timeText;
        public Text speedText;
        public Button pauseButton;
        public Button proceedButton;
        public Text pauseButtonText;
        public Text proceedButtonText;

        public GameObject uiCanvas;
        public PointsCanvasBehaviour pointsCanvas;
        public CalibrationPlaneBehaviour calibrationPlane;

        private float timeupTimer = TIME_TO_CALIBRATE;
        private float freezeTimer = 0;
        private bool isPaused = false;

        private float speed = 0.0f;
        private Vector3 lastCameraPosition;
        private bool isTracked = false;

        private string calibrationState = "";
        private int collectedTemplates = 0;

        private Resolution resolution;

        public override void Load()
        {
            calibrationPlane.gameObject.SetActive(true);
            calibrationPlane.Current = 0;

            timeupTimer = TIME_TO_CALIBRATE;
            freezeTimer = 0;

            speed = 0.0f;
            lastCameraPosition = trackingCamera.transform.position;
            isTracked = false;
            isPaused = false;

            resolution = Screen.currentResolution;
            pointsCanvas.Fit(calibrationPlane.Width, calibrationPlane.Height);

            wizard.worker.SetCameraCalibrationState("run");
        }

        public override void Unload()
        {
            calibrationPlane.gameObject.SetActive(false);
        }

        public override void Update()
        {
            // Detect resolution change
            if (resolution.width != Screen.currentResolution.width || resolution.height != Screen.currentResolution.height)
            {
                pointsCanvas.Fit(calibrationPlane.Width, calibrationPlane.Height);
                resolution = Screen.currentResolution;
            }

            // Track the speed based on the distance moved
            speed = Vector3.Distance(trackingCamera.transform.position, lastCameraPosition);
            lastCameraPosition = trackingCamera.transform.position;

            // No interaction is possible during a freeze and a restart is executed after it is over.
            if (freezeTimer > 0)
            {
                freezeTimer = Math.Max(0, freezeTimer - Time.deltaTime);

                if (freezeTimer == 0)
                {
                    RestartButtonClick();
                }

                return;
            }

            if (isPaused)
            {
                speedText.text = "-";
            }
            else
            {
                templatesText.text = collectedTemplates + "";

                timeupTimer = Math.Max(0, timeupTimer - Time.deltaTime);
                timeText.color = timeupTimer < TIME_TO_CALIBRATE / 6 ? Color.red :
                                        timeupTimer < TIME_TO_CALIBRATE / 2 ? Color.yellow : Color.green;
                timeText.text = Math.Ceiling(timeupTimer) + " s";

                speedText.text = string.Format("{0:0.00}", speed);
                
                // In case of wrong usage, the interaction is freezed
                // for some time and an error message is shown.
                if (timeupTimer == 0 || speed > MAX_SPEED)
                {
                    if (timeupTimer == 0)
                    {
                        instructionText.text = "Time is up! You have been too slow.";
                    }
                    else if (speed > MAX_SPEED)
                    {
                        instructionText.text = "Too fast! Avoid fast movements, as bluring will harm the calibration results.";
                    }

                    instructionText.color = Color.red;
                    freezeTimer = FREEZE_TIME;
                    pauseButton.interactable = false;
                    proceedButton.interactable = false;
                    return;
                }
            }

            // Whenever the on-screen points match the plane,
            // and it is not the last one, the next plane is rendered. 
            if (pointsCanvas.IsMatching)
            {
                if (calibrationPlane.IsLast)
                {
                    wizard.State = wizard.optimizingState;
                }
                else
                {
                    calibrationPlane.Current++;
                }
            }
        }

        #region Buttons

        public void BackButtonClick()
        {
            wizard.State = wizard.deviceState;
        }

        public void RestartButtonClick()
        {
            timeupTimer = TIME_TO_CALIBRATE;
            freezeTimer = 0;

            calibrationPlane.Current = 0;

            pauseButton.interactable = true;
            proceedButton.interactable = true;
            
            wizard.worker.SetCameraCalibrationState("reset");
        }

        public void PauseButtonClick()
        {
            switch (calibrationState)
            {
                case "paused":
                    wizard.worker.SetCameraCalibrationState("run");
                    pauseButtonText.text = "Pause";
                    isPaused = false;
                    calibrationPlane.gameObject.SetActive(true);
                    break;
                
                case "collecting":
                case "collectingActive":
                    wizard.worker.SetCameraCalibrationState("pause");
                    pauseButtonText.text = "Run";
                    isPaused = true;
                    calibrationPlane.gameObject.SetActive(false);
                    break;
            }
        }

        public void ProceedButtonClick()
        {
            if (calibrationState == "paused")
            {
                // Optimizing can only be entered from "running"
                wizard.worker.SetCameraCalibrationState("run");
            }

            wizard.State = wizard.optimizingState;
        }

        #endregion

        #region Events

        private void OnEnable()
        {
            // Bugfix to enforce re-layouting
            uiCanvas.SetActive(false);
            uiCanvas.SetActive(true);
            pointsCanvas.gameObject.SetActive(false);
            pointsCanvas.gameObject.SetActive(true);

            calibrationPlane.PlaneChanged += PlaneChanged;
            VLWorkerBehaviour.OnTrackingState += TrackingStateChanged;
            VLWorkerBehaviour.OnTrackingStates += StoreTrackingStates;
        }

        private void OnDisable()
        {
            calibrationPlane.PlaneChanged -= PlaneChanged;
            VLWorkerBehaviour.OnTrackingState -= TrackingStateChanged;
            VLWorkerBehaviour.OnTrackingStates -= StoreTrackingStates;
        }

        private void PlaneChanged()
        {
            stepText.text = (calibrationPlane.Current + 1) + "/" + calibrationPlane.NumberOfPlanes;
        }

        private void TrackingStateChanged(int state, string objectID)
        {
            if (freezeTimer > 0)
            {
                return;
            }

            bool oldIsTracked = isTracked;
            isTracked = (state == 100);

            if (isTracked != oldIsTracked)
            {
                lastCameraPosition = trackingCamera.transform.position;
            }

            if (isTracked)
            {
                calibrationPlane.gameObject.SetActive(!isPaused);

                instructionText.text = "Move the camera to a position where all points of the same color match.";
                instructionText.color = Color.white;
            }
            else
            {
                calibrationPlane.gameObject.SetActive(false);

                instructionText.text = "Point the camera on the image you prepared until tracking starts.";
                instructionText.color = Color.yellow;
            }
        }

        private void StoreTrackingStates(VLTrackingState state)
        {
            if (state.objects.Length > 0)
            {
                calibrationState = state.objects[0].state;
                collectedTemplates = state.objects[0]._NumberOfTemplates;
            }
        }

        #endregion
    }
}