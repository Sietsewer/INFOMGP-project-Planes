namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Wing characteristics")]
    public class WingCharacteristics : ScriptableObject
    {
        public float liftMultiplier = 1.0f;

        public float AngleOfAttackLimit = 30;
        public AnimationCurve LiftCoefficientCurve = new AnimationCurve(new Keyframe(-30f, -1.45f), new Keyframe(-20f, -1.7f), new Keyframe(20f, 1.7f), new Keyframe(30f, 1.45f));

        public float LiftCoefficient(float angleOfAttack)
        {
            if (-AngleOfAttackLimit < angleOfAttack && angleOfAttack < AngleOfAttackLimit)
            {
                return LiftCoefficientCurve.Evaluate(angleOfAttack) * liftMultiplier;
            }

            return 0;
        }
    }
}