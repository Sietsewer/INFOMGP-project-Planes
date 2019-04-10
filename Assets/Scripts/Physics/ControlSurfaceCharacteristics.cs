namespace Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Control surface characteristics")]
    public class ControlSurfaceCharacteristics : ScriptableObject
    {
        public float additionalMultiplier = 1.0f;

        public AnimationCurve additionalCoefficientCurve = new AnimationCurve (new Keyframe(-90, -0.5f), new Keyframe(90, 0.5f));

        public float multiplierMultiplier = 1.0f;

        public AnimationCurve coefficientMultiplierCurve = new AnimationCurve(new Keyframe(-90, 1.0f), new Keyframe(90, 1.0f));

        public float ModifyLiftCoefficient(float lifCoefficient, float angleOfAttack)
        {
            return lifCoefficient * (coefficientMultiplierCurve.Evaluate(angleOfAttack) * multiplierMultiplier) + (additionalCoefficientCurve.Evaluate(angleOfAttack) * additionalMultiplier);
        }
    }
}