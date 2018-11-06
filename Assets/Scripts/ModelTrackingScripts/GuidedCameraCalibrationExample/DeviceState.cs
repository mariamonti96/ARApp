using UnityEngine;
using UnityEngine.UI;

namespace GuidedCameraCalibrationExample
{
    public class DeviceState : GuidedCameraCalibrationExample.CalibrationState
    {
        public GuidedCameraCalibrationExample wizard;

        public Dropdown devicesDropdown;

        public Button proceedButton;

        public VLDeviceInfo.Camera SelectedDevice
        {
            get;
            private set;
        }

        private VLDeviceInfo.Camera[] devices;

        public override void Load()
        {
            VLDeviceInfo deviceInfo = wizard.worker.GetDeviceInfo();

            if (deviceInfo == null)
            {
                devices = new VLDeviceInfo.Camera[] {};
            }
            else
            {
                devices = deviceInfo.availableCameras;
            }

            devicesDropdown.options.Clear();
            
            if (devices.Length == 0)
            {
                devicesDropdown.options.Add(new Dropdown.OptionData("No camera found"));
                devicesDropdown.value = 0;
                devicesDropdown.interactable = false;
                proceedButton.interactable = false;
            }
            else
            {
                foreach (var device in devices)
                {
                    devicesDropdown.options.Add(new Dropdown.OptionData(device.cameraName));
                }

                devicesDropdown.value = 0;
                devicesDropdown.interactable = true;
                proceedButton.interactable = true;
            }
            
            SelectedDeviceChanged();
        }

        public void SelectedDeviceChanged()
        {
            devicesDropdown.RefreshShownValue();
        }

        public void BackButtonClick()
        {
            wizard.State = wizard.imageState;
        }

        public void ProceedButtonClick()
        {
            SelectedDevice = devices[devicesDropdown.value];

            wizard.worker.StartTracking("cameraCalibration.vl?input.useDeviceID=" + SelectedDevice.deviceID);	

            wizard.State = wizard.collectingState;
        }
    }
}