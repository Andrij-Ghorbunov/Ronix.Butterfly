namespace Ronix.Butterfly.Wpf.Evolution
{
    public class NamedTournamentScore
    {
        public string Name { get; }
        public string[] Score { get; }

        public NamedTournamentScore(string name, string[] scores)
        {
            Name = name;
            Score = scores;
        }
    }
}
