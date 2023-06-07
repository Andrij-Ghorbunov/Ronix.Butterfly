namespace Ronix.Neural.Activation
{
    public class BellActivationFunction : ActivationFunctionBase
    {
        internal BellActivationFunction() : base("Bell", "Bell: e^(-x^2)")
        {
        }

        public override double Value(double arg)
        {
            return Math.Exp(-arg * arg);
        }

        public override double Derivative(double arg)
        {
            return -2 * arg * Value(arg);
        }
    }
}
