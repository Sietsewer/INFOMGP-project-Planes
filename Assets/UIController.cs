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

        // Update is called once per frame
        void Update()
        {
            if (sliderPitch != null) sliderPitch.value = Input.GetAxis("pitch");

            if (sliderRoll != null) sliderRoll.value = Input.GetAxis("roll");

            if (sliderYaw != null) sliderYaw.value = Input.GetAxis("yaw");

            if (sliderThrottle != null) sliderThrottle.value = Input.GetAxis("throttle");


        }
    }
}