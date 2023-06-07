namespace Ronix.Neural.Activation
{
    public static class ActivationFunctions
    {
        /// <summary>
        /// No activation function - use this for the input layer
        /// </summary>
        public static readonly NoActivationFunction Null = new();

        /// <summary>
        /// y = max(x, 0)
        /// </summary>
        public static readonly ReLuActivationFunction ReLu = new();
        /// <summary>
        /// y = max(x, 0.01x)
        /// </summary>
        public static readonly LeakyReLuActivationFunction LeakyReLu = new(0.01);
        /// <summary>
        /// y = x
        /// </summary>
        public static readonly LinearActivationFunction Linear = new();
        /// <summary>
        /// y = (e^x - e^-x) / (e^x + e^-x)
        /// </summary>
        public static readonly TanhActivationFunction Tanh = new();
        /// <summary>
        /// y = 1 / (1 + e^-x)
        /// </summary>
        public static readonly LogisticActivationFunction Logistic = new();
        /// <summary>
        /// Nonmonotonic bell-shaped: y = e^(-x^2)
        /// </summary>
        public static readonly BellActivationFunction Bell = new();

        public static IEnumerable<ActivationFunctionBase> GetAll()
        {
            yield return Null;
            yield return ReLu;
            yield return LeakyReLu;
            yield return Linear;
            yield return Tanh;
            yield return Logistic;
            yield return Bell;
        }

        public static ActivationFunctionBase GetByName(string name)
        {
            return GetAll().FirstOrDefault(it => it.Name == name);
        }
    }
}
