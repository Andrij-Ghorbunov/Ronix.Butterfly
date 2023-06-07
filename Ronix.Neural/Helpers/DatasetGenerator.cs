namespace Ronix.Neural.Helpers
{
    public static class DatasetGenerator
    {
        public static void Generate(int datasetSize, int inputSize, int outputSize, Func<double[], double[]> groundTruth,
            Random random, int min, int max, out double[,] inputs, out double[,] outputs)
        {
            inputs = new double[inputSize, datasetSize];
            outputs = new double[outputSize, datasetSize];
            var input = new double[inputSize];
            var diff = max - min;
            for (var itemNumber = 0; itemNumber < datasetSize; itemNumber++)
            {
                for (var inputNeuron = 0; inputNeuron < inputSize; inputNeuron++)
                {
                    var data = min + diff * random.NextDouble();
                    inputs[inputNeuron, itemNumber] = data;
                    input[inputNeuron] = data;
                }
                var output = groundTruth(input);
                for (var outputNeuron = 0; outputNeuron < outputSize; outputNeuron++)
                {
                    outputs[outputNeuron, itemNumber] = output[outputNeuron];
                }
            }
        }
    }
}
