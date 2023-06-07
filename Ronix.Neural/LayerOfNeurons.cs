using Ronix.Neural.Activation;

namespace Ronix.Neural
{
    public class LayerOfNeurons
    {
        public int NumberOfNeurons { get; }

        public ActivationFunctionBase ActivationFunction { get; set; }

        public LayerOfNeurons(int numberOfNeurons, ActivationFunctionBase activationFunction)
        {
            NumberOfNeurons = numberOfNeurons;
            ActivationFunction = activationFunction;
        }

        public LayerOfNeurons(int numberOfNeurons)
        {
            NumberOfNeurons = numberOfNeurons;
            ActivationFunction = ActivationFunctions.Null;
        }
    }
}
