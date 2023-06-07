namespace Ronix.Neural.Activation
{
    public sealed class ReLuActivationFunction : ActivationFunctionBase
    {
        public override bool IsBounded => false;

        internal ReLuActivationFunction() : base("ReLU", "ReLU: y = max(x, 0)")
        {
        }

        public override double Value(double arg)
        {
            return arg < 0 ? 0 : arg;
        }

        public override double Derivative(double arg)
        {
            return arg < 0 ? 0 : 1;
        }
    }
}
