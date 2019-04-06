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


            float airspeed = plane.velocity.magnitude;

            float indicatedAirspeed = Vector3.Project(plane.velocity, plane.transform.forward).magnitude;

            infoText.text = "Real airspeed\t<b>" + (airspeed * 3.6f).ToString("0") + "\t</b>km/h\nIndicated airspeed\t<b>" + (indicatedAirspeed * 3.6f).ToString("0") + "\t</b>km/h";
        }
    }
}