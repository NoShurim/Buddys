using ScalingTypes = Jax_Beta_Fixed.Managers.DamageManager.ScalingTypes;


namespace Jax_Beta_Fixed.Bases
{
    class Scale
    {
        public readonly float[] scaling;
        public readonly ScalingTypes scalingType;

        public Scale(float[] scaling, ScalingTypes scalingType)
        {
            this.scaling = scaling;
            this.scalingType = scalingType;
        }
    }
}
