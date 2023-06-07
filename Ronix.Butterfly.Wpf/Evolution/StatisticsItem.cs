namespace Ronix.Butterfly.Wpf.Evolution
{
    public class StatisticsItem
    {
        public int Score { get; set; }

        public int GenerationNumber { get; set; }
        public int TotalGenerationNumber { get; set; }

        public int SurvivedGenerations { get; set; }

        public string ScoreAgainstRandom { get; set; }
        public string Layers { get; set; }
        public string LastEvolutionHistory { get; set; }
        public string FileName { get; set; }
        public string Comments { get; set; }
    }
}
