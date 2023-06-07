namespace Ronix.Neural.Activation
{
    public sealed class NoActivationFunction : ActivationFunctionBase
    {
        internal NoActivationFunction() : base("Null", "No Activation Function (input layer)")
        {
        }

        public override double Derivative(double arg)
        {
            throw new InvalidOperationException("Attempt to invoke NoActivationFunction. This activation function should only be used on the input layer where no activation is performed");
        }

        public override double Value(double arg)
        {
            throw new InvalidOperationException("Attempt to invoke NoActivationFunction derivative. This activation function should only be used on the input layer where no activation is performed");
        }
    }
}
