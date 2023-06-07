using Ronix.Butterfly.Wpf.Helpers;
using Ronix.Neural;
using System.IO;

namespace Ronix.Butterfly.Wpf.Players
{
    public class NeuralPlayer : PlayerBase
    {
        public override bool NeedsFileBrowsing => true;

        public override string Name => "Neural network";

        public override string Description => FileName;

        public LayeredNeuralNetwork Network { get; set; }

        public string FileName { get; set; } = "";

        public override void Load(string path)
        {
            Network = LayeredNeuralNetwork.LoadFromFile(path);
            FileName = Path.GetFileNameWithoutExtension(path);
        }

        public override PlayerBase Copy()
        {
            return new NeuralPlayer { Network = Network, FileName = FileName };
        }

        public override int? Prompt(ButterflyGame game)
        {
            return NeuralHelper.Move(game, Network);
        }
    }
}
