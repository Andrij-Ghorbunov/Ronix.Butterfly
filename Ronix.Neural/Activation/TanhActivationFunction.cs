namespace Ronix.Neural.Activation
{
    public sealed class TanhActivationFunction : ActivationFunctionBase
    {
        internal TanhActivationFunction() : base("Tanh", "Tanh: y = (e^x - e^-x) / (e^x + e^-x)")
        {
        }

        public override double Value(double arg)
        {
            return Math.Tanh(arg);
        }

        public override double Derivative(double arg)
        {
            var x = Math.Tanh(arg);
            return 1 - x * x;
        }
    }
}
