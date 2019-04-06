namespace Game
{
    using System;
    using Physics;

    [Serializable]

    public class BoundControlSurface
    {
        public ControlSurfaceType SurfaceType;
        public ControlSurface Surface;
        public bool inverted;

        public void SetPosition(float position)
        {
            Surface.SetPosition(inverted ? 1 - position : position);
        }

        public void SetAngle(float angle)
        {
            Surface.SetAngle(inverted ? -angle : angle);
        }

        public enum ControlSurfaceType
        {
            Aileron,
            Rudder,
            Elevator,
            Flap
        }
    }
}
