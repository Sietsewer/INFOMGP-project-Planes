namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        public Transform TargetObject;

        public Mode CameraMode = Mode.Track;

        private void Awake()
        {
            Debug.Assert(minDistance >= 0, "Minimum camera distance must be greater than zero.", this);
            Debug.Assert(minDistance <= initialDistance && initialDistance <= maxDistance, "Initial distance must be between min and max distance.", this);
            Debug.Assert(maxDistance >= minDistance, "Maximum camera distance must be greater than minimum distance.", this);
        }

        private void Start()
        {
            previousMousePos = Input.mousePosition;

            zOffset = Mathf.Clamp(initialDistance, minDistance, maxDistance);
        }

        public float minDistance = 0;
        public float maxDistance = 100;
        public float initialDistance = 10;

        private float xOffset;
        private float yOffset = 20;
        private float zOffset;

        private Vector3 previousMousePos;

        void OrbitUpdate()
        {
            if (TargetObject == null) return;

            if (Input.GetMouseButton(0))
            {
                Vector3 mouseDelta = Input.mousePosition - previousMousePos;

                xOffset += (mouseDelta.x / Screen.dpi) * 20;
                yOffset -= (mouseDelta.y / Screen.dpi) * 20;

                xOffset %= 360;
                yOffset = Mathf.Clamp(yOffset, -89.99f, 89.99f); // Not 90 to prevent gimbal lock
            }

            zOffset -= Input.mouseScrollDelta.y * 0.5f;
            zOffset = Mathf.Clamp(zOffset, minDistance, maxDistance);

            Quaternion rotation = Quaternion.Euler(yOffset, xOffset, 0);

            Vector3 offset = rotation * Vector3.back * zOffset;

            transform.position = TargetObject.position + offset;
            transform.LookAt(TargetObject);

            previousMousePos = Input.mousePosition;
        }

        void TrackUpdate()
        {
            if (TargetObject == null) return;

            transform.LookAt(TargetObject);
        }
        
        void Update()
        {
            if (CameraMode == Mode.Orbit || CameraMode == Mode.Track) Debug.Assert(TargetObject != null, "For tracking camera modes, tracking target must be set!", this);

            switch (CameraMode)
            {
                case Mode.Orbit:
                    OrbitUpdate();
                    break;
                case Mode.Track:
                    TrackUpdate();
                    break;
            }
        }

        public enum Mode
        {
            Orbit,
            Track
        }
    }
}