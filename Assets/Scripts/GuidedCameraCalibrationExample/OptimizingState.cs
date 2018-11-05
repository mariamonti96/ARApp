using UnityEngine;
using UnityEngine.UI;

namespace GuidedCameraCalibrationExample
{
    public class OptimizingState : GuidedCameraCalibrationExample.CalibrationState
    {
        public GuidedCameraCalibrationExample wizard;

        public Text instructionText;

        public override void Load()
        {
            wizard.worker.SetCameraCalibrationState("optimize");
        }

        public void ResetButtonClick()
        {
            wizard.worker.SetCameraCalibrationState("reset");
        }

        private void OnEnable()
        {
            VLWorkerBehaviour.OnTrackingStates += StoreTrackingStates;
        }

        private void OnDisable()
        {
            VLWorkerBehaviour.OnTrackingStates -= StoreTrackingStates;
        }

        private void StoreTrackingStates(VLTrackingState state)
        {
            string calibrationState = state.objects.Length > 0 ? state.objects[0].state : null;
            
            switch (calibrationState)
            {
                // Stopping finished
                case "paused":
                    wizard.State = wizard.collectingState;
                    break;

                // Start stopping
                case "stopping":
                    instructionText.text = "Stopping...";
                    break;

                // Optimizing finished
                case "done":
                    wizard.State = wizard.doneState;
                    break;
                
                // Start optimizing
                default:
                    instructionText.text = "The result is being optimized. Please wait and be patient as this takes some time.";
                    break;
            }
        }
    }
}