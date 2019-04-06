namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(PlaneController))]
    public class PlanePlayerInputManager : MonoBehaviour
    {
        private PlaneController PlaneController;

        private void Awake()
        {
            PlaneController = FindObjectOfType<PlaneController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            PlaneController.pitch = Input.GetAxis("pitch");
            PlaneController.roll = Input.GetAxis("roll");
            PlaneController.yaw = Input.GetAxis("yaw");
            PlaneController.flaps = (Input.GetAxis("flaps") + 1) / 2;
            PlaneController.throttle = (Input.GetAxis("throttle") + 1) / 2;

            if (Input.GetButtonUp("gear"))
            {
                PlaneController.gear = !PlaneController.gear;
            }
        }
    }
}