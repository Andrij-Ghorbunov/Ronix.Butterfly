namespace Ronix.Neural.Helpers
{
    public static class NeuralNetworkConverter
    {
        public static LayeredNeuralNetwork BackpropToEvolution(LayeredNeuralNetworkWithBackpropagation backprop)
        {
            using var stream = new MemoryStream();
            backprop.Save(stream);
            using var stream2 = new MemoryStream(stream.ToArray());
            return LayeredNeuralNetwork.LoadFromFile(stream2);
        }

        public static LayeredNeuralNetworkWithBackpropagation EvolutionToBackprop(LayeredNeuralNetwork evo)
        {
            using var stream = new MemoryStream();
            evo.Save(stream);
            return LayeredNeuralNetworkWithBackpropagation.LoadFromFile(stream);
        }
    }
}
