using UnityEngine;

public class CalibrationPlaneBehaviour : MonoBehaviour
{
    public delegate void PlaneChangedEvent();

    public event PlaneChangedEvent PlaneChanged;

    private struct CalibrationPlane
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }

    private CalibrationPlane[] calibrationPlanes = new CalibrationPlane[]
    {
        new CalibrationPlane()
        {
            position = new Vector3(0.032f, 0.02135f, 0),
            scale = 0.6f * new Vector3(5, 3.6f, 2.0f),
            rotation = Quaternion.Euler(180, 0, 0)
        },
        new CalibrationPlane()
        {
            position = 0.8f * new Vector3(0.032f, 0.02135f, 0),
            scale = 0.6f * new Vector3(5, 3.6f, 2.0f),
            rotation = Quaternion.Euler(180, 30, 0)
        },
        new CalibrationPlane()
        {
            position = 0.8f * new Vector3(0.032f, 0.02135f, 0),
            scale = 0.6f * new Vector3(5, 3.6f, 2.0f),
            rotation = Quaternion.Euler(180, -30, 0)
        },
        new CalibrationPlane()
        {
            position = 0.8f * new Vector3(0.032f, 0.02135f, 0),
            scale = 0.6f * new Vector3(5, 3.6f, 2.0f),
            rotation = Quaternion.Euler(180 + 30, 0, 0)
        },
        new CalibrationPlane()
        {
            position = 0.8f * new Vector3(0.032f, 0.02135f, 0),
            scale = 0.6f * new Vector3(5, 3.6f, 2.0f),
            rotation = Quaternion.Euler(180 - 30, 0, 0)
        }
    };

    private int current = -1;
    public int Current
    {
        get
        {
            return current;
        }
        set
        {
            int newValue = value % calibrationPlanes.Length;

            if (current != newValue)
            {
                current = newValue;

                transform.localPosition = calibrationPlanes[current].position;
                transform.localScale = calibrationPlanes[current].scale;
                transform.localRotation = calibrationPlanes[current].rotation;

                if (PlaneChanged != null)
                {
                    PlaneChanged();
                }
            }
        }
    }

    public int NumberOfPlanes
    {
        get
        {
            return calibrationPlanes.Length;
        }
    }

    public bool IsFirst
    {
        get
        {
            return current == 0;
        }
    }

    public bool IsLast
    {
        get
        {
            return current == (calibrationPlanes.Length - 1);
        }
    }

    public float Width
    {
        get
        {
            return transform.localScale.x;
        }
    }

    public float Height
    {
        get
        {
            return transform.localScale.y;
        }
    }
}
