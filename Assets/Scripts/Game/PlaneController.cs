namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Physics;

    public class PlaneController : MonoBehaviour
    {
        public List<BoundControlSurface> ControlSurfaces;

        [Range(-1, 1)]
        public float pitch;
        [Range(-1, 1)]
        public float roll;
        [Range(-1, 1)]
        public float yaw;
        [Range(0, 90)]
        public float flaps;

        // Start is called before the first frame update
        private void Start()
        {

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

        // Update is called once per frame
        private void Update()
        {
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
                        surface.SetAngle(flaps);
                        break;
                    default: continue;
                }
            }
        }
    }
}