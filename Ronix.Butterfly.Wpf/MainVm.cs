using Ronix.Butterfly.Wpf.Helpers;
using Ronix.Butterfly.Wpf.Views;
using Ronix.Framework.Mvvm;

namespace Ronix.Butterfly.Wpf
{
    public class MainVm: ViewModelBase
    {
        public GameVm Game { get; }
        public CompetitionRulesVm CompetitionRules { get; }
        public EvolutionVm Evolution { get; }
        public NamedTournamentVm NamedTournament { get; }
        public EditorVm Editor { get; }
        public DatasetsVm Datasets { get; }
        public TrainingVm Training { get; }
        public NoiseVm Noise { get; }

        public AppSettings Settings { get; }

        public MainVm()
        {
            Game = new GameVm();
            CompetitionRules = new CompetitionRulesVm();
            Evolution = new EvolutionVm();
            NamedTournament = new NamedTournamentVm();
            Editor = new EditorVm();
            Datasets = new DatasetsVm();
            Training = new TrainingVm();
            Noise = new NoiseVm();

            Datasets.SetGame(Game);
            Evolution.SetRules(CompetitionRules.GetModel);
            NamedTournament.SetRules(CompetitionRules.GetModel);
            Training.Evolution = Evolution;
            NeuralHelper.Noise = Noise;

            Settings = AppSettings.Load();
            Game.ApplySettings(Settings);
            Evolution.ApplySettings(Settings);
            NamedTournament.ApplySettings(Settings);
            Editor.ApplySettings(Settings);
            Datasets.ApplySettings(Settings);
            Training.ApplySettings(Settings);
        }
    }
}
