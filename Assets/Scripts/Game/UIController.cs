namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIController : MonoBehaviour
    {
        public Slider sliderPitch;
        public Slider sliderRoll;
        public Slider sliderYaw;
        public Slider sliderThrottle;

        public Text infoText;
        public Rigidbody plane;

        // Update is called once per frame
        void Update()
        {
            if (sliderPitch != null) sliderPitch.value = Input.GetAxis("pitch");
            if (sliderRoll != null) sliderRoll.value = Input.GetAxis("roll");
            if (sliderYaw != null) sliderYaw.value = Input.GetAxis("yaw");
            if (sliderThrottle != null) sliderThrottle.value = Input.GetAxis("throttle");

            Debug.Assert(infoText != null && plane != null, "No infotext or plane set", this);

            float indicatedAirspeed = Vector3.Project(plane.velocity, plane.transform.forward).magnitude * 3.6f;

            float altitude = plane.position.y;

            float aoa = Vector3.SignedAngle(plane.velocity, plane.transform.forward, plane.transform.right);

            float climb = plane.velocity.y;

            infoText.text = string.Format("Airspeed\n<size={0}><b>{1:0}</b></size> km/h\nClimb\n<size={0}><b>{2:0}</b></size> m/s\nA.O.A.\n<size={0}><b>{3:0}</b></size> °\nAltitude\n<size={0}><b>{4:0}</b></size> m\n",
                20, indicatedAirspeed, climb, aoa, altitude);
        }
    }
}