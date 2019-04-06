namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Physics;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlaneController : MonoBehaviour
    {
        public List<BoundControlSurface> ControlSurfaces;
        public List<WheelCollider> Wheels;

        public List<Thruster> Thrusters;

        [Range(-1, 1)]
        public float pitch;
        [Range(-1, 1)]
        public float roll;
        [Range(-1, 1)]
        public float yaw;
        [Range(0, 1)]
        public float flaps;

        [Range(0, 1)]
        public float brakes;

        [Range(0, 1)]
        public float throttle;

        private Animator animator;
        new private Rigidbody rigidbody;

        public float maxFlaps = 90;

        private bool p_gear;
        public bool gear;

        private void Awake()
        {
            animator = FindObjectOfType<Animator>();

            p_gear = gear = animator.GetBool(ANIMATOR_LANDING_GEAR);

            rigidbody = FindObjectOfType<Rigidbody>();
        }

        private void Reset()
        {
            foreach(var surface in GetComponentsInChildren<ControlSurface>())
            {
                ControlSurfaces.Add(new BoundControlSurface()
                {
                    Surface = surface,
                    SurfaceType = BoundControlSurface.ControlSurfaceType.Elevator
                });
            }
        }

        const string ANIMATOR_LANDING_GEAR = "Landing gear";

        private void Update()
        {
            bool weightOnWheels = false;
            
            foreach (WheelCollider wheel in Wheels)
            {
                wheel.brakeTorque = brakes;
                wheel.motorTorque = throttle > float.Epsilon ? 1 : 0;

                if(!weightOnWheels && wheel.isGrounded)
                {
                    weightOnWheels = true;
                }
            }

            if (gear != p_gear)
            {
                if (weightOnWheels)
                {
                    gear = p_gear;
                }
                else
                {
                    p_gear = gear;
                    animator.SetBool(ANIMATOR_LANDING_GEAR, gear);
                }
            }
            
            pitch = Mathf.Clamp(pitch, -1, 1);
            roll = Mathf.Clamp(roll, -1, 1);
            yaw = Mathf.Clamp(yaw, -1, 1);

            flaps = Mathf.Clamp(flaps, 0, 90);

            foreach(BoundControlSurface surface in ControlSurfaces)
            {
                switch (surface.SurfaceType)
                {
                    case BoundControlSurface.ControlSurfaceType.Aileron:
                        surface.SetPosition((roll + 1) / 2);
                        break;
                    case BoundControlSurface.ControlSurfaceType.Elevator:
                        surface.SetPosition((pitch + 1) / 2);
                        break;
                    case BoundControlSurface.ControlSurfaceType.Rudder:
                        surface.SetPosition((yaw + 1) / 2);
                        break;
                    case BoundControlSurface.ControlSurfaceType.Flap:
                        surface.SetAngle(flaps * maxFlaps);
                        break;
                    default: continue;
                }
            }
            
            foreach (Thruster thruster in Thrusters)
            {
                thruster.throttle = throttle;
            }
            
            Vector3 comLine = Vector3.up;
            Vector3 com = rigidbody.worldCenterOfMass;
            Debug.DrawLine(com, com + comLine, Color.magenta);
        }
    }
}