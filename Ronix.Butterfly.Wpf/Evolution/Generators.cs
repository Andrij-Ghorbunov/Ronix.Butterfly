using Ronix.Butterfly.Wpf.Backpropagation;
using Ronix.Neural;
using Ronix.Neural.Activation;
using Ronix.Neural.Helpers;
using System;

namespace Ronix.Butterfly.Wpf.Evolution
{
    public static class Generators
    {
        public static LayerOfNeurons[] Layers = new[]
        {
            new LayerOfNeurons(Constants.InputNeurons),
            new LayerOfNeurons(60, ActivationFunctions.Tanh),
            new LayerOfNeurons(50, ActivationFunctions.Tanh),
            new LayerOfNeurons(Constants.OutputNeurons, ActivationFunctions.Linear),
        };

        public static Dataset Dataset1, Dataset50;

        public static double LearningRate = 0.1;
        public static int Iterations = 10;
        public static int? MaxBlockSize = null;

        public static LayeredNeuralNetwork Default(Random random)
        {
            var network = new LayeredNeuralNetwork(Layers);
            network.Randomize(random);
            return network;
        }

        public static LayeredNeuralNetwork PreTrained(Random random)
        {
            var network = new LayeredNeuralNetworkWithBackpropagation(Layers);
            network.Randomize(random);
            var dataset = network.OutputNumber == 1 ? Dataset1 : Dataset50;
            for (var iteration = 0; iteration < Iterations; iteration++)
            {
                network.Train(dataset.Inputs, dataset.Outputs, LearningRate, MaxBlockSize);
            }
            return NeuralNetworkConverter.BackpropToEvolution(network);
        }
    }
}
