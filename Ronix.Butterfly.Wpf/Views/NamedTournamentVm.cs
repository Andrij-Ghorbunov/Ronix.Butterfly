using Microsoft.WindowsAPICodePack.Dialogs;
using Ronix.Butterfly.Wpf.Evolution;
using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ronix.Butterfly.Wpf.Views
{
    public class NamedTournamentVm : ViewModelWithStopwatch
    {
        private string _lastDirectory;

        public event Action<string[]> BuildColumns;

        public ObservableCollection<NamedTournamentScore> Scores { get; }

        public ObservableCollection<NamedTournamentParticipant> Participants { get; }

        private double _roundProgress;

        public double RoundProgress
        {
            get => _roundProgress;
            set => SetValue(ref _roundProgress, value);
        }

        public NamedTournament NamedTournament { get; }

        private string _path;

        public string Path
        {
            get => _path;
            set => SetValue(ref _path, value);
        }

        private int _whiteWins;

        public int WhiteWins
        {
            get => _whiteWins;
            set => SetValue(ref _whiteWins, value);
        }

        private int _blackWins;

        public int BlackWins
        {
            get => _blackWins;
            set => SetValue(ref _blackWins, value);
        }

        public void ApplySettings(AppSettings settings)
        {
            _lastDirectory = settings.RootFolder;
        }

        private int _draws;

        public int Draws
        {
            get => _draws;
            set => SetValue(ref _draws, value);
        }

        private string _whiteBlackStats;

        public string WhiteBlackStats
        {
            get => _whiteBlackStats;
            set => SetValue(ref _whiteBlackStats, value);
        }

        public ICommand Browse { get; }
        public ICommand Load { get; }
        public ICommand Play { get; }

        public NamedTournamentVm()
        {
            Participants = new();
            Scores = new();
            NamedTournament = new();
            Browse = new Command(BrowseCommand);
            Load = new Command(LoadCommand);
            Play = new Command(PlayCommand);
            NamedTournament.ProgressReport += x => RoundProgress = x;
        }

        public void SetRules(Func<CompetitionRules> rules)
        {
            NamedTournament.Rules = rules;
        }

        private void BrowseCommand()
        {
            var dlg = new CommonOpenFileDialog { IsFolderPicker = true, Title = "Select tournament folder", InitialDirectory = _lastDirectory };
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return;
            Path = dlg.FileName;
            _lastDirectory = System.IO.Path.GetDirectoryName(Path);
            LoadCommand();
        }

        private void LoadCommand()
        {
            if (string.IsNullOrEmpty(Path))
            {
                MessageBox.Show("You didn't specify a folder!");
                return;
            }
            NamedTournament.Load(Path);
            Participants.Clear();
            foreach (var p in NamedTournament.Participants.OrderBy(it => it.FileName))
            {
                Participants.Add(new NamedTournamentParticipant
                {
                    Name = p.FileName,
                    Score = 0,
                    Place = 0
                });
            }
        }

        private async void PlayCommand()
        {
            if (NamedTournament.Participants.Count == 0)
            {
                MessageBox.Show("There are no participants! Did you load a tournament?");
                return;
            }

            StartRound();

            var scores = await Task.Factory.StartNew(NamedTournament.PlayFullRound);
            var names = scores.Select(it => it.Name).ToArray();
            var participants = NamedTournament.Participants;

            FinishRound();

            Participants.Clear();
            var index = 0;
            foreach (var p in participants.OrderByDescending(it => it.Score))
            {
                Participants.Add(new NamedTournamentParticipant
                {
                    Name = p.FileName,
                    Score = p.Score,
                    Place = ++index
                });
            }

            Scores.Clear();
            foreach (var s in scores)
            {
                Scores.Add(s);
            }

            BuildColumns?.Invoke(names);
            RoundProgress = 0;

            WhiteWins = NamedTournament.WhiteWins;
            BlackWins = NamedTournament.BlackWins;
            Draws = NamedTournament.Draws;
            WhiteBlackStats = NamedTournament.WhiteBlackStats;
        }
    }
}
