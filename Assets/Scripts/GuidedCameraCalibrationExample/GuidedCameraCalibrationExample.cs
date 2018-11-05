using UnityEngine;

namespace GuidedCameraCalibrationExample
{
    public class GuidedCameraCalibrationExample : MonoBehaviour
    {
        public VLWorkerBehaviour worker;

        public IntroState introState;
        public ImageState imageState;
        public DeviceState deviceState;
        public CollectingState collectingState;
        public OptimizingState optimizingState;
        public DoneState doneState;
        
        private CalibrationState state = null;
        public CalibrationState State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != null)
                {
                    state.Unload();
                    state.gameObject.SetActive(false);
                }

                state = value;
                state.gameObject.SetActive(true);
                state.Load();
            }
        }

        public void Start()
        {
            State = introState;
        }

        public void OnDestroy()
        {
            worker.StopTracking();
        }

        public abstract class CalibrationState : MonoBehaviour
        {
            public virtual void Load()
            {
            }

            public virtual void Unload()
            {
            }

            public virtual void Update()
            {
            }
        }
    }
}