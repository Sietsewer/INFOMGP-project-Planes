namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Wing characteristics")]
    public class WingCharacteristics : ScriptableObject
    {
        public float liftMultiplier = 1.0f;

        public float AngleOfAttackLimitMax = 30;
        public float AngleOfAttackLimitMin = -30;
        public AnimationCurve LiftCoefficientCurve = new AnimationCurve(new Keyframe(-30f, -1.45f), new Keyframe(-20f, -1.7f), new Keyframe(20f, 1.7f), new Keyframe(30f, 1.45f));

        public float dragMultiplier;
        public AnimationCurve DragCoefficientCurve = new AnimationCurve(new Keyframe(-90f, -1.0f), new Keyframe(0f, 0.0f), new Keyframe(90, 1.0f));

        public float LiftCoefficient(float angleOfAttack)
        {
            if (AngleOfAttackLimitMin < angleOfAttack && angleOfAttack < AngleOfAttackLimitMax)
            {
                return LiftCoefficientCurve.Evaluate(angleOfAttack) * liftMultiplier;
            }

            return 0.0f;
        }

        public float DragCoefficient(float angleToSurface)
        {
            return LiftCoefficientCurve.Evaluate(angleToSurface) * dragMultiplier;
        }
    }
}