namespace Ronix.Neural.Activation
{
    public sealed class LinearActivationFunction : ActivationFunctionBase
    {
        public override bool IsBounded => false;

        internal LinearActivationFunction() : base("Linear", "Linear: y = x")
        {
        }

        public override double Value(double arg)
        {
            return arg;
        }

        public override double Derivative(double arg)
        {
            return 1;
        }
    }
}
