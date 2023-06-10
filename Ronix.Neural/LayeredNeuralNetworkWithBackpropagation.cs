using Ronix.Neural.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Neural
{
    public class LayeredNeuralNetworkWithBackpropagation : LayeredNeuralNetwork
    {
        public bool IsClassificator { get; set; }

        public string FileName { get; set; }

        protected readonly double[][] DeltaSignal;

        protected readonly double[][,] ConnectomeCorrection;

        protected readonly double[][] BiasCorrection;

        protected readonly double[][] ActivationDerivative;

        protected readonly double[][] Hypothesis;

        private readonly double[] LastDeltaSignal;

        public LayeredNeuralNetworkWithBackpropagation(params LayerOfNeurons[] layers): base(layers)
        {
            DeltaSignal = new double[Layers.Length][];
            for (var layer = 0; layer < Layers.Length; layer++)
            {
                DeltaSignal[layer] = new double[Layers[layer].NumberOfNeurons];
            }
            ConnectomeCorrection = new double[LayerCount][,];
            BiasCorrection = new double[LayerCount][];
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                ConnectomeCorrection[layer] = new double[Layers[layer].NumberOfNeurons, Layers[nextLayer].NumberOfNeurons];
                BiasCorrection[layer] = new double[Layers[nextLayer].NumberOfNeurons];
            }
            ActivationDerivative = new double[Layers.Length][];
            for (var layer = 1; layer < Layers.Length; layer++) // first layer won't have that
            {
                ActivationDerivative[layer] = new double[Layers[layer].NumberOfNeurons];
            }
            Hypothesis = new double[Layers.Length][];
            for (var layer = 1; layer < Layers.Length; layer++) // first layer won't have that
            {
                Hypothesis[layer] = new double[Layers[layer].NumberOfNeurons];
            }
            LastDeltaSignal = DeltaSignal[^1]; // arr[^1] means 'last item of array arr'
        }

        protected void ResetCorrections()
        {
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                var connectomePart = ConnectomeCorrection[layer];
                var neuronCount = Layers[nextLayer].NumberOfNeurons;
                var synapseCount = Layers[layer].NumberOfNeurons;
                var bias = BiasCorrection[layer];
                for (var neuron = 0; neuron < neuronCount; neuron++)
                {
                    for (var synapse = 0; synapse < synapseCount; synapse++)
                    {
                        connectomePart[synapse, neuron] = 0;
                    }
                    bias[neuron] = 0;
                }
            }
        }

        protected void ResetDeltas()
        {
            for (var layer = 0; layer < LayerCount; layer++)
            {
                var delta = DeltaSignal[layer];
                var numberOfNeurons = delta.Length;
                for (var neuron = 0; neuron < numberOfNeurons; neuron++)
                {
                    delta[neuron] = 0;
                }
            }
        }

        protected override double[] PassSignal()
        {
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                var connectomePart = Connectome[layer];
                var neuronCount = Layers[nextLayer].NumberOfNeurons;
                var activationFunction = Layers[nextLayer].ActivationFunction;
                var synapseCount = Layers[layer].NumberOfNeurons;
                var previousSignal = Signal[layer];
                var nextSignal = Signal[nextLayer];
                var nextHypothesis = Hypothesis[nextLayer];
                var nextDerivative = ActivationDerivative[nextLayer];
                var bias = Bias[layer];
                for (var neuron = 0; neuron < neuronCount; neuron++)
                {
                    var sum = bias[neuron];
                    for (var synapse = 0; synapse < synapseCount; synapse++)
                    {
                        sum += previousSignal[synapse] * connectomePart[synapse, neuron];
                    }

                    nextHypothesis[neuron] = sum;
                    nextSignal[neuron] = activationFunction.Value(sum);
                    nextDerivative[neuron] = activationFunction.Derivative(sum);
                }
            }
            return Signal[LayerCount];
        }


        /// <summary>
        /// Trains this neural network based on the training dataset using backpropagation.
        /// </summary>
        /// <param name="inputs">Array of input data. The first index is the number of dataset item, the second index is the number of the input layer neuron.</param>
        /// <param name="outputs">Array of output data. The first index is the number of dataset item, the second index is the number of the output layer neuron.</param>
        /// <param name="learningRate">Learning rate - a number between 0 and 1. The default is 0.01.</param>
        /// <param name="maxBatchSize">Maximum number of samples to feed at once before applying correction. Null for single-chunk.</param>
        public TrainingStatistics Train(double[,] inputs, double[,] outputs, double learningRate = 0.01, int? maxBatchSize = null)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (outputs == null) throw new ArgumentNullException(nameof(outputs));
            var datasetSize = inputs.GetLength(0);
            if (datasetSize != outputs.GetLength(0)) throw new ArgumentException($"First dimension of input and output arrays must be equal - it is the size of the dataset. Got {inputs.GetLength(0)} and {outputs.GetLength(0)}.");
            if (inputs.GetLength(1) != InputNumber) throw new ArgumentException($"Second dimension of input expected to be the number of input neurons ({InputNumber}), but got {inputs.GetLength(1)}", nameof(inputs));
            if (outputs.GetLength(1) != OutputNumber) throw new ArgumentException($"Second dimension of output expected to be the number of output neurons ({OutputNumber}), but got {outputs.GetLength(1)}", nameof(outputs));

            // initialize all connectome/bias corrections to 0
            ResetCorrections();

            ResetStats();

            if (maxBatchSize.HasValue)
            {
                var size = maxBatchSize.Value;
                if (size <= 0) throw new ArgumentOutOfRangeException(nameof(maxBatchSize), "Batch size must be positive");
                var indices = Randomizer.Shuffle(datasetSize, new Random());
                var itemsLeft = datasetSize;
                var skip = 0;
                while (itemsLeft > 0)
                {
                    var chunkIndices = indices.Skip(skip).Take(size).ToArray();
                    skip += size;
                    itemsLeft -= chunkIndices.Length;
                    TrainPart(inputs, outputs, learningRate, chunkIndices);
                }
            }
            else
            {
                var indices = Enumerable.Range(0, datasetSize).ToArray();
                TrainPart(inputs, outputs, learningRate, indices);
            }
            return CollectStats();
        }

        protected void TrainPart(double[,] inputs, double[,] outputs, double learningRate, int[] itemIndices)
        {
            var batchSize = itemIndices.Length;

            // for each sample in training dataset
            for (var sampleIndex = 0; sampleIndex < batchSize; sampleIndex++)
            {
                TrainSample(inputs, outputs, itemIndices[sampleIndex]);
            }

            // 2 comes from the derivative of quadratic C which we didn't take into account in the calculations yet:
            // C = (a - desired)^2 = delta^2; dC/da = 2 * delta
            // Batch size is needed to calculate the average values - since now we only had sum of all values, not average
            ApplyCorrections(2 * learningRate / batchSize);
        }

        private void TrainSample(double[,] inputs, double[,] outputs, int datasetSampleIndex)
        {
            // put input of the item to the first layer and calculate all other layers
            for (var inputNeuronIndex = 0; inputNeuronIndex < InputNumber; inputNeuronIndex++)
            {
                FirstSignal[inputNeuronIndex] = inputs[datasetSampleIndex, inputNeuronIndex];
            }
            PassSignal();

            ResetDeltas();

            var meanSquareDev = 0d;

            // fill deltas between actual and desired values for the output layer
            for (var outputNeuronIndex = 0; outputNeuronIndex < OutputNumber; outputNeuronIndex++)
            {
                var output = outputs[datasetSampleIndex, outputNeuronIndex];
                var outputDelta = output - LastSignal[outputNeuronIndex];
                meanSquareDev += outputDelta * outputDelta;
                LastDeltaSignal[outputNeuronIndex] = outputDelta;
            }

            _cumulativeDev += meanSquareDev;
            _sampleCount++;

            // gather statistics - check if the classificator gives the correct answer on this data item
            if (IsClassificator)
            {
                var maxSignal = double.NegativeInfinity;
                var maxSignalIndex = -1;
                var maxDesiredSignal = double.NegativeInfinity;
                var maxDesiredSignalIndex = -1;
                for (var outputNeuronIndex = 0; outputNeuronIndex < OutputNumber; outputNeuronIndex++)
                {
                    var desiredSignal = outputs[datasetSampleIndex, outputNeuronIndex];
                    var receivedSignal = LastSignal[outputNeuronIndex];
                    if (desiredSignal > maxDesiredSignal)
                    {
                        maxDesiredSignal = desiredSignal;
                        maxDesiredSignalIndex = outputNeuronIndex;
                    }
                    if (receivedSignal > maxSignal)
                    {
                        maxSignal = receivedSignal;
                        maxSignalIndex = outputNeuronIndex;
                    }
                }
                if (maxSignalIndex == maxDesiredSignalIndex)
                {
                    _correctGuesses++;
                }
                else
                {
                    _incorrectGuesses++;
                }
            }

            // for each layer pair from last till first
            for (int layerIndex = LayerCount - 1, nextLayerIndex = LayerCount; layerIndex >= 0; layerIndex--, nextLayerIndex--)
            {
                var numberOfFrontNeurons = Layers[nextLayerIndex].NumberOfNeurons;
                var numberOfBackNeurons = Layers[layerIndex].NumberOfNeurons;
                var deltas = DeltaSignal[nextLayerIndex];
                var connectome = Connectome[layerIndex];
                var connectomeCorrections = ConnectomeCorrection[layerIndex];
                var biasCorrections = BiasCorrection[layerIndex];
                var backSignals = Signal[layerIndex];
                var backDeltas = DeltaSignal[layerIndex];
                var derivatives = ActivationDerivative[nextLayerIndex];
                var backSignalScale = Layers[nextLayerIndex].ActivationFunction.IsBounded
                    ? (1d / (numberOfBackNeurons + 1)) // bounded functions - normalize by simply the number of neurons
                    : (1 / (backSignals.Sum(x => x * x) + 1)); // unbounded functions - normalize by square norm of all signals
                                                        // + 1 is the bias - basically a fixed signal
                var frontSignalScale = 1d / numberOfFrontNeurons;

                // for each neuron in the front layer
                for (var frontNeuronIndex = 0; frontNeuronIndex < numberOfFrontNeurons; frontNeuronIndex++)
                {
                    //var currentSignal = frontSignal[frontNeuronIndex];
                    var currentActivationDerivative = derivatives[frontNeuronIndex];

                    // we need to alter this neuron's signal proportionally to this value (e. g. increase if this value is positive)
                    var activatedDelta = deltas[frontNeuronIndex];

                    // the steeper the activation function here, the more important the change
                    var delta = activatedDelta * currentActivationDerivative;

                    // for each synapse leading to this neuron
                    for (var backNeuronIndex = 0; backNeuronIndex < numberOfBackNeurons; backNeuronIndex++)
                    {
                        var backSignal = backSignals[backNeuronIndex];
                        var weight = connectome[backNeuronIndex, frontNeuronIndex];

                        // the correction must also be proportional to the signal of the back neuron
                        connectomeCorrections[backNeuronIndex, frontNeuronIndex] += delta * backSignal * backSignalScale;

                        // also calculate desiderata for the back layer
                        backDeltas[backNeuronIndex] += delta * weight;
                    }

                    // and for the neuron's bias - here signal is basically 1
                    biasCorrections[frontNeuronIndex] += delta * backSignalScale;
                }

                // desired changes for the back layer should be average - we calculated sum
                for (var backNeuronIndex = 0; backNeuronIndex < numberOfBackNeurons; backNeuronIndex++)
                {
                    backDeltas[backNeuronIndex] *= frontSignalScale;
                }
            }
        }

        protected void ApplyCorrections(double learningRate)
        {
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                var connectomePart = Connectome[layer];
                var connectomeCorrectionPart = ConnectomeCorrection[layer];
                var neuronCount = Layers[nextLayer].NumberOfNeurons;
                var synapseCount = Layers[layer].NumberOfNeurons;
                var bias = Bias[layer];
                var biasCorrection = BiasCorrection[layer];
                for (var neuron = 0; neuron < neuronCount; neuron++)
                {
                    double correction;
                    for (var synapse = 0; synapse < synapseCount; synapse++)
                    {
                        correction = connectomeCorrectionPart[synapse, neuron];
                        if (double.IsNaN(correction))
                            throw new Exception("Bad training");
                        connectomePart[synapse, neuron] += learningRate * correction;
                        connectomeCorrectionPart[synapse, neuron] = 0;
                    }
                    correction = biasCorrection[neuron];
                    if (double.IsNaN(correction))
                        throw new Exception("Bad training");
                    bias[neuron] += learningRate * correction;
                    biasCorrection[neuron] = 0;
                }
            }
        }
        public static new LayeredNeuralNetworkWithBackpropagation LoadFromFile(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            var r = LoadFromFile(stream);
            r.FileName = Path.GetFileName(path);
            return r;
        }

        public static new LayeredNeuralNetworkWithBackpropagation LoadFromFile(Stream stream)
        {
            using var br = new BinaryReader(stream);
            var firstByte = br.ReadByte();
            var secondByte = br.ReadByte();
            if (firstByte != 78 || secondByte != 78)
                throw new Exception("This file does not contain a neural network!");
            var layerCount = br.ReadInt32();
            var layers = new LayerOfNeurons[layerCount + 1];
            for (var layer = 0; layer <= layerCount; layer++)
            {
                var numberOfNeurons = br.ReadInt32();
                var actFuncName = br.ReadString();
                var actFunc = Activation.ActivationFunctions.GetByName(actFuncName);
                if (actFunc == null)
                    throw new Exception("Activation function with name " + actFuncName + " not found!");
                layers[layer] = new LayerOfNeurons(numberOfNeurons, actFunc);
            }
            var network = new LayeredNeuralNetworkWithBackpropagation(layers);
            for (int prevLayer = 0, nextLayer = 1; prevLayer < layerCount; prevLayer++, nextLayer++)
            {
                var synapseCount = layers[prevLayer].NumberOfNeurons;
                var neuronCount = layers[nextLayer].NumberOfNeurons;
                var connectomePart = network.Connectome[prevLayer];
                var biasPart = network.Bias[prevLayer];
                for (var neuron = 0; neuron < neuronCount; neuron++)
                {
                    for (var synapse = 0; synapse < synapseCount; synapse++)
                    {
                        connectomePart[synapse, neuron] = br.ReadDouble();
                    }
                    biasPart[neuron] = br.ReadDouble();
                }
            }
            network.GenerationNumber = br.ReadInt32();
            network.TotalGenerationNumber = br.ReadInt32();
            network.SurvivedGenerations = br.ReadInt32();
            network.LastEvolutionHistory = br.ReadString();
            network.Comments = br.ReadString();
            return network;
        }

        #region Statistics

        private int _sampleCount;
        private int _correctGuesses;
        private int _incorrectGuesses;
        private double _cumulativeDev;

        private void ResetStats()
        {
            _sampleCount = 0;
            _cumulativeDev = 0;
            _correctGuesses = 0;
            _incorrectGuesses = 0;
        }

        private TrainingStatistics CollectStats()
        {
            double? rate = null;
            if (IsClassificator)
                rate = (double)_correctGuesses / (_correctGuesses + _incorrectGuesses);
            return new TrainingStatistics { RateOfCorrectGuesses = rate, MeanSquareDeviation = _cumulativeDev / _sampleCount / OutputNumber };
        }

        #endregion Statistics
    }
}
