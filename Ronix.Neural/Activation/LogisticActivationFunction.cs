namespace Ronix.Neural.Activation
{
    public class LogisticActivationFunction : ActivationFunctionBase
    {
        internal LogisticActivationFunction() : base("Logistic", "Logistic: 1 / (1 + e^-x)")
        {
        }

        public override double Value(double arg)
        {
            return 1 / (1 + Math.Exp(-arg));
        }

        public override double Derivative(double arg)
        {
            var v = Value(arg);
            return v * (1 - v);
        }
    }
}
