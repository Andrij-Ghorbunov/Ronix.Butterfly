namespace Ronix.Neural.Activation
{
    public sealed class LeakyReLuActivationFunction : ActivationFunctionBase
    {
        private readonly double _leak;
        public override bool IsBounded => false;

        internal LeakyReLuActivationFunction(double leak) : base($"Leaky{leak:f3}", $"Leaky ReLU: y = max(x, {leak:f3}x)")
        {
            if (leak < 0 || leak > 1) throw new ArgumentOutOfRangeException(nameof(leak), "Leak must be between 0 and 1");
            _leak = leak;
        }

        public override double Value(double arg)
        {
            return arg < 0 ? _leak * arg : arg;
        }

        public override double Derivative(double arg)
        {
            return arg < 0 ? _leak : 1;
        }
    }
}
