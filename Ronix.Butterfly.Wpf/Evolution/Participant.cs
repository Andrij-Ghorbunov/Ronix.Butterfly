using Ronix.Neural;

namespace Ronix.Butterfly.Wpf.Evolution
{
    public class Participant
    {
        public LayeredNeuralNetwork Network { get; }

        public int Score { get; set; }

        public string ScoreAgainstRandom { get; set; }

        public string FileName { get; set; }

        public Participant(LayeredNeuralNetwork network)
        {
            Network = network;
            Score = 0;
        }
    }
}
