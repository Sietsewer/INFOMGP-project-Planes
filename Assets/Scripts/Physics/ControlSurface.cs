namespace Physics
{
    using System;
    using UnityEngine;
    
    public class ControlSurface : MonoBehaviour
    {
        public float maxAngleUp = 30;
        public float maxAngleDown = 30;
        public bool invert = false;

        [Range(0, 1), SerializeField]
        private float position = 0.5f;
        
        private float angle = 0.0f;

        public Axis rotationAxis = Axis.XAxis;

        private Quaternion initialRotation;

        private Vector3 rotationVector;

        private void ClampValues()
        {
            maxAngleUp = Mathf.Abs(maxAngleUp);
            maxAngleDown = Mathf.Abs(maxAngleDown);

            position = Mathf.Clamp01(position);
        }

        public void SetAngle(float angle)
        {
            ClampValues();

            angle = Mathf.Clamp(angle, -maxAngleUp, maxAngleDown);

            angle += maxAngleUp;

            float totalAngle = maxAngleUp + maxAngleDown;

            position = angle / totalAngle;
        }

        public void SetPosition(float position)
        {
            this.position = position;
        }

        // Start is called before the first frame update
        private void Start()
        {
            ClampValues();

            switch (rotationAxis)
            {
                case Axis.XAxis:

                    rotationVector = new Vector3(1.0f, 0.0f, 0.0f);
                    break;

                case Axis.YAxis:

                    rotationVector = new Vector3(0.0f, 1.0f, 0.0f);
                    break;

                case Axis.ZAxis:

                    rotationVector = new Vector3(0.0f, 0.0f, 1.0f);
                    break;

                default:
                    throw new Exception("Unreachable code reached.");
            }

            if (invert) rotationVector = -rotationVector;
        }

        private void Awake()
        {
            initialRotation = transform.localRotation;
        }

        // Update is called once per frame
        private void Update()
        {
            angle = Mathf.Lerp(-maxAngleUp, maxAngleDown, position);

            transform.localRotation = initialRotation * Quaternion.Euler(rotationVector * angle);
        }

        public enum Axis : byte
        {
            XAxis = 0,
            YAxis = 1,
            ZAxis = 2
        }
    }
}