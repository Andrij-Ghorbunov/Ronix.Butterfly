namespace Ronix.Neural
{
    /// <summary>
    /// A feed-forward neural network organized in layers with neuron biases.
    /// </summary>
    public class LayeredNeuralNetwork
    {
        public readonly int LayerCount;

        public readonly double[][,] Connectome;

        public readonly double[][] Bias;

        public readonly double[][] Signal;

        public readonly double[] FirstSignal, LastSignal;

        public readonly int InputNumber, OutputNumber;

        public readonly LayerOfNeurons[] Layers;

        /// <summary>
        /// Shows how many total generations this instance survived.
        /// </summary>
        public int TotalGenerationNumber { get; set; }

        /// <summary>
        /// Shows how many mutations this instance had since its initialization.
        /// </summary>
        public int GenerationNumber { get; set; }

        /// <summary>
        /// Shows how many last generations this instance survived without mutations.
        /// </summary>
        public int SurvivedGenerations { get; set; }

        /// <summary>
        /// = - survived, + - survived and gave offspring, * - mutated; history of last 10 rounds
        /// </summary>
        public string LastEvolutionHistory { get; set; }

        public string Comments { get; set; }

        public string LayerConfiguration { get; }

        /// <summary>
        /// Constructs a feed-forward neural network organized in layers.
        /// <para>
        /// Hypothesis function is biased linear combination (b + w1*x1 + w2*x2 + ... + wN*xN).
        /// </para>
        /// </summary>
        /// <param name="layers">Number of neurons in each layer. The first number is the size of input, and the last one is the size of output.</param>
        /// <exception cref="ArgumentException"></exception>
        public LayeredNeuralNetwork(params LayerOfNeurons[] layers)
        {
            if (layers.Length < 2) throw new ArgumentException("There should be at least two layers - input and output", nameof(layers));
            LayerCount = layers.Length - 1;
            Layers = layers.ToArray();
            Connectome = new double[LayerCount][,];
            Bias = new double[LayerCount][];
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                Connectome[layer] = new double[Layers[layer].NumberOfNeurons, Layers[nextLayer].NumberOfNeurons];
                Bias[layer] = new double[Layers[nextLayer].NumberOfNeurons];
            }
            Signal = new double[Layers.Length][];
            for (var layer = 0; layer < Layers.Length; layer++)
            {
                Signal[layer] = new double[Layers[layer].NumberOfNeurons];
            }
            InputNumber = Layers[0].NumberOfNeurons;
            OutputNumber = Layers[^1].NumberOfNeurons; // arr[^1] means 'last item of array arr'
            FirstSignal = Signal[0];
            LastSignal = Signal[^1];
            LastEvolutionHistory = string.Empty;
            Comments = string.Empty;

            LayerConfiguration = string.Join("-", Layers.Select(it => it.NumberOfNeurons.ToString()));
        }

        /// <summary>
        /// Constructs a copy of the layered neural network.
        /// </summary>
        /// <param name="copyFrom">Source network to copy from.</param>
        protected LayeredNeuralNetwork(LayeredNeuralNetwork copyFrom)
        {
            LayerCount = copyFrom.LayerCount;
            Layers = copyFrom.Layers.ToArray();
            Connectome = new double[LayerCount][,];
            Bias = new double[LayerCount][];
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                var synapses = Layers[layer].NumberOfNeurons;
                var neurons = Layers[nextLayer].NumberOfNeurons;
                Connectome[layer] = new double[synapses, neurons];
                Bias[layer] = new double[neurons];
                Array.Copy(copyFrom.Connectome[layer], Connectome[layer], synapses * neurons);
                Array.Copy(copyFrom.Bias[layer], Bias[layer], neurons);
            }
            Signal = new double[Layers.Length][];
            for (var layer = 0; layer < Layers.Length; layer++)
            {
                Signal[layer] = new double[Layers[layer].NumberOfNeurons];
            }
            FirstSignal = Signal[0];
            LastSignal = Signal[^1]; // arr[^1] means 'last item of array arr'
            InputNumber = copyFrom.InputNumber;
            OutputNumber = copyFrom.OutputNumber;
            GenerationNumber = copyFrom.GenerationNumber;
            TotalGenerationNumber = copyFrom.TotalGenerationNumber;
            LastEvolutionHistory = copyFrom.LastEvolutionHistory;
            Comments = copyFrom.Comments;
            LayerConfiguration = copyFrom.LayerConfiguration;
        }

        /// <summary>
        /// Constructs an output based on the provided input signal.
        /// </summary>
        /// <param name="input">The input signal.</param>
        /// <returns>The output signal.</returns>
        /// <exception cref="ArgumentException"></exception>
        public double[] ComputeOutput(double[] input)
        {
            if (input.Length != Layers[0].NumberOfNeurons) throw new ArgumentException("Length of input should correspond to first layer size", nameof(input));
            input.CopyTo(Signal[0], 0);
            return PassSignal();
        }

        protected virtual double[] PassSignal()
        {
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                var connectomePart = Connectome[layer];
                var neuronCount = Layers[nextLayer].NumberOfNeurons;
                var activationFunction = Layers[nextLayer].ActivationFunction;
                var synapseCount = Layers[layer].NumberOfNeurons;
                var previousSignal = Signal[layer];
                var nextSignal = Signal[nextLayer];
                var bias = Bias[layer];
                for (var neuron = 0; neuron < neuronCount; neuron++)
                {
                    var sum = bias[neuron];
                    for (var synapse = 0; synapse < synapseCount; synapse++)
                    {
                        sum += previousSignal[synapse] * connectomePart[synapse, neuron];
                    }
                    nextSignal[neuron] = activationFunction.Value(sum);
                }
            }
            return Signal[LayerCount];
        }

        /// <summary>
        /// Fills the network's connectome and biases with default values (1s and 0s respectively).
        /// </summary>
        public void Reset()
        {
            GenerationNumber = 0;
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                var connectomePart = Connectome[layer];
                var neuronCount = Layers[nextLayer].NumberOfNeurons;
                var synapseCount = Layers[layer].NumberOfNeurons;
                var bias = Bias[layer];
                for (var neuron = 0; neuron < neuronCount; neuron++)
                {
                    for (var synapse = 0; synapse < synapseCount; synapse++)
                    {
                        connectomePart[synapse, neuron] = 1;
                    }
                    bias[neuron] = 0;
                }
            }
        }

        /// <summary>
        /// Fills the network's connectome and biases with random values.
        /// </summary>
        /// <param name="random">Entropy source.</param>
        /// <param name="central"><c>True</c> for weights between -1.0 and +1.0 cap-distributed; <c>false</c> for weights between 0.0 and +1.0.</param>
        /// <param name="biasCentral"><c>True</c> for biases between -1.0 and +1.0 cap-distributed; <c>false</c> for biases between 0.0 and +1.0.</param>
        public void Randomize(Random random, bool central = true, bool biasCentral = true)
        {
            GenerationNumber = 0;
            for (int layer = 0, nextLayer = 1; layer < LayerCount; layer++, nextLayer++)
            {
                var connectomePart = Connectome[layer];
                var neuronCount = Layers[nextLayer];
                var synapseCount = Layers[layer];
                var bias = Bias[layer];
                for (var neuron = 0; neuron < neuronCount.NumberOfNeurons; neuron++)
                {
                    for (var synapse = 0; synapse < synapseCount.NumberOfNeurons; synapse++)
                    {
                        connectomePart[synapse, neuron] = central ?  (random.NextDouble() - random.NextDouble()) : random.NextDouble();
                    }
                    bias[neuron] = biasCentral ? (random.NextDouble() - random.NextDouble()) : random.NextDouble();
                }
            }
        }

        /// <summary>
        /// Randomly changes some of the weights and biases.
        /// </summary>
        /// <param name="random">Entropy source.</param>
        /// <param name="numberOfMutations">Number of mutations to inflict.</param>
        /// <param name="amplitudeOfMutations">Maximum amplitude of each mutation.</param>
        public void Mutate(Random random, int numberOfMutations, double amplitudeOfMutations)
        {
            for (var mutation = 0; mutation < numberOfMutations; mutation++)
            {
                var layer = random.Next(LayerCount);
                var synapse = random.Next(Layers[layer].NumberOfNeurons + 1) - 1;
                var isBias = synapse < 0;
                var neuron = random.Next(Layers[layer + 1].NumberOfNeurons);
                if (isBias)
                {
                    Bias[layer][neuron] += (random.NextDouble() - random.NextDouble()) * amplitudeOfMutations;
                }
                else
                {
                    var connectomePart = Connectome[layer];
                    connectomePart[synapse, neuron] += (random.NextDouble() - random.NextDouble()) * amplitudeOfMutations;
                }
            }
        }

        /// <summary>
        /// Produces a slightly altered copy of the current network (for genetic algorithms).
        /// </summary>
        /// <param name="random">Entropy source.</param>
        /// <param name="numberOfMutations">Number of mutations to inflict.</param>
        /// <param name="amplitudeOfMutations">Maximum amplitude of each mutation.</param>
        /// <returns>An offspring network.</returns>
        public LayeredNeuralNetwork Offspring(Random random, int numberOfMutations, double amplitudeOfMutations)
        {
            var copy = new LayeredNeuralNetwork(this);
            copy.Mutate(random, numberOfMutations, amplitudeOfMutations);
            copy.GenerationNumber = GenerationNumber + 1;
            copy.SurvivedGenerations = 0;
            return copy;
        }

        public void Save(string path)
        {
            using var file = new FileStream(path, FileMode.Create);
            Save(file);
        }

        public void Save(Stream stream)
        {
            using var bw = new BinaryWriter(stream);
            bw.Write((byte)78); // NN - magical symbols (Neural Network)
            bw.Write((byte)78);
            bw.Write(LayerCount);
            for (var layer = 0; layer <= LayerCount; layer++)
            {
                bw.Write(Layers[layer].NumberOfNeurons);
                bw.Write(Layers[layer].ActivationFunction.Name);
            }
            for (int prevLayer = 0, nextLayer = 1; prevLayer < LayerCount; prevLayer++, nextLayer++)
            {
                var synapseCount = Layers[prevLayer].NumberOfNeurons;
                var neuronCount = Layers[nextLayer].NumberOfNeurons;
                var connectomePart = Connectome[prevLayer];
                var biasPart = Bias[prevLayer];
                for (var neuron = 0; neuron < neuronCount; neuron++)
                {
                    for (var synapse = 0; synapse < synapseCount; synapse++)
                    {
                        bw.Write(connectomePart[synapse, neuron]);
                    }
                    bw.Write(biasPart[neuron]);
                }
            }
            bw.Write(GenerationNumber);
            bw.Write(TotalGenerationNumber);
            bw.Write(SurvivedGenerations);
            bw.Write(LastEvolutionHistory ?? string.Empty);
            bw.Write(Comments ?? string.Empty);
        }
        public static LayeredNeuralNetwork LoadFromFile(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            return LoadFromFile(stream);
        }

        public static LayeredNeuralNetwork LoadFromFile(Stream stream)
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
            var network = new LayeredNeuralNetwork(layers);
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
    }
}
