using Ronix.Butterfly.Wpf.Evolution;
using Ronix.Butterfly.Wpf.Helpers;
using Ronix.Butterfly.Wpf.Players;
using Ronix.Framework;
using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace Ronix.Butterfly.Wpf.Views
{
    public class EvolutionVm : ViewModelWithStopwatch
    {
        private string _lastDirectory;
        
        public EvolutionInstance Evolution { get; }

        public MacroparametersVm Macroparameters { get; }
        
        public LayersConstructorVm LayersConstructor { get; }

        public ObservableCollection<StatisticsItem> Stats { get; }

        private string _path;

        public string Path
        {
            get => _path;
            set => SetValue(ref _path, value);
        }

        private bool _saveAfterEachRound = true;

        public bool SaveAfterEachRound
        {
            get => _saveAfterEachRound;
            set => SetValue(ref _saveAfterEachRound, value);
        }

        private int _saveRounds = 1;

        public int SaveRounds
        {
            get => _saveRounds;
            set => SetValue(ref _saveRounds, value);
        }

        private int _n = 5;
        
        public int N
        {
            get => _n;
            set => SetValue(ref _n, value, ValidateN);
        }

        public void ApplySettings(AppSettings settings)
        {
            _lastDirectory = settings.RootFolder;
        }

        private void ValidateN(int oldValue, int newValue)
        {
            if (newValue < 1)
                N = oldValue;
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set => SetValue(ref _isRunning, value, IsRunningChanged);
        }

        private bool _stopRequested;

        private int _roundsPlayed;

        public int RoundsPlayed
        {
            get => _roundsPlayed;
            set => SetValue(ref _roundsPlayed, value);
        }

        private double _roundProgress;

        public double RoundProgress
        {
            get => _roundProgress;
            set => SetValue(ref _roundProgress, value);
        }


        private bool _isPreTrainPrepared;

        public bool IsPreTrainPrepared
        {
            get => _isPreTrainPrepared;
            set => SetValue(ref _isPreTrainPrepared, value);
        }

        private bool _isPreTrainEnabled;

        public bool IsPreTrainEnabled
        {
            get => _isPreTrainEnabled;
            set => SetValue(ref _isPreTrainEnabled, value, IsPreTrainEnabledChanged);
        }

        private void IsPreTrainEnabledChanged(bool newValue)
        {
            Evolution.ParticipantGenerator = newValue ? Generators.PreTrained : Generators.Default;
        }

        private void IsRunningChanged()
        {
            _stop.RaiseCanExecuteChanged();
            _runOneRound.RaiseCanExecuteChanged();
            _runNRounds.RaiseCanExecuteChanged();
            _runIndefinitely.RaiseCanExecuteChanged();
            _testAgainstRandom.RaiseCanExecuteChanged();
        }

        private readonly Command _stop, _runOneRound, _runNRounds, _runIndefinitely, _testAgainstRandom, _testBalance;

        public ICommand RunOneRound => _runOneRound;

        public ICommand RunNRounds => _runNRounds;

        public ICommand RunIndefinitely => _runIndefinitely;

        public ICommand Stop => _stop;

        public ICommand TestAgainstRandom => _testAgainstRandom;

        public ICommand TestBalance => _testBalance;

        public ICommand Save { get; }

        public ICommand Load { get; }

        public ICommand Browse { get; }

        public EvolutionVm()
        {
            _runOneRound = new Command(RunOneRoundCommand, () => !IsRunning);
            _runNRounds = new Command(RunNRoundsCommand, () => !IsRunning);
            _runIndefinitely = new Command(RunIndefinitelyCommand, () => !IsRunning);
            _stop = new Command(StopCommand, () => IsRunning);
            _testAgainstRandom = new Command(TestAgainstRandomCommand, () => !IsRunning);
            _testBalance = new Command(TestBalanceCommand, () => !IsRunning);
            Save = new Command(SaveCommand);
            Load = new Command(LoadCommand);
            Browse = new Command(BrowseCommand);

            Evolution = new EvolutionInstance();
            LayersConstructor = new LayersConstructorVm();
            Macroparameters = new MacroparametersVm();

            Stats = new ObservableCollection<StatisticsItem>();
            Evolution.ProgressReport += ProgressReport;

            Macroparameters.Load(Evolution);
        }

        public void SetRules(Func<CompetitionRules> rules)
        {
            Evolution.Rules = rules;
        }

        private bool CheckLoaded()
        {
            if (!string.IsNullOrEmpty(Evolution.DirectoryPath)) return true;
            MessageBox.Show("The tournament is not loaded!");
            return false;
        }

        private async void RunOneRoundCommand()
        {
            if (!CheckLoaded()) return;
            IsRunning = true;
            BeforeRunning();
            BeforeEachRound();
            //await Task.Factory.StartNew(Evolution.Generation);
            await Evolution.GenerationAsync();
            AfterEachRound();
            IsRunning = false;
            AfterRunning();
        }

        private async void RunNRoundsCommand()
        {
            if (!CheckLoaded()) return;
            IsRunning = true;
            BeforeRunning();
            var n = N;
            await Task.Factory.StartNew(async () =>
            {
                while (n > 0 && !_stopRequested)
                {
                    BeforeEachRound();
                    await Evolution.GenerationAsync();
                    //Evolution.Generation();
                    n--;
                    AfterEachRound();
                }
            });
            IsRunning = false;
            AfterRunning();
        }

        private async void RunIndefinitelyCommand()
        {
            if (!CheckLoaded()) return;
            IsRunning = true;
            BeforeRunning();
            //await Task.Factory.StartNew(async () =>
            //{
                while (!_stopRequested)
                {
                    BeforeEachRound();
                    await Evolution.GenerationAsync();
                    AfterEachRound();
                }
            //});
            AfterRunning();
            IsRunning = false;
        }

        private void StopCommand()
        {
            _stopRequested = true;
        }

        private void SaveCommand()
        {
            if (!CheckLoaded()) return;
            Evolution.DirectoryPath = Path;
            Generators.Layers = LayersConstructor.GetLayers();
            Evolution.Save();
        }

        private void LoadCommand()
        {
            if (string.IsNullOrEmpty(Path))
            {
                MessageBox.Show("You didn't specify a folder!");
                return;
            }
            Evolution.DirectoryPath = Path;
            Generators.Layers = LayersConstructor.GetLayers();
            Macroparameters.Save(Evolution);
            Evolution.Load();
            Macroparameters.Load(Evolution);
            Evolution.SaveScores();
            UpdateStats(Evolution.Stats);
        }

        private void BrowseCommand()
        {
            var dlg = new CommonOpenFileDialog { IsFolderPicker = true, Title = "Select tournament folder", InitialDirectory = _lastDirectory };
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return;
            Path = dlg.FileName;
            _lastDirectory = System.IO.Path.GetDirectoryName(Path);
        }

        private void BeforeRunning()
        {
            Macroparameters.Save(Evolution);
            Evolution.SaveAfterRound = SaveAfterEachRound;
            Evolution.SaveAfterRoundN = SaveRounds;
            Generators.Layers = LayersConstructor.GetLayers();
            _stopRequested = false;
        }

        private void AfterRunning()
        {
            RoundProgress = 0;
            Evolution.Save();
        }

        private void BeforeEachRound()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                StartRound();
            });
        }

        private void AfterEachRound()
        {
            var stats = Evolution.Stats;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                RoundsPlayed++;
                UpdateStats(stats);
                FinishRound();
            });
        }

        private void UpdateStats(List<StatisticsItem> stats)
        {
            Stats.Clear();
            foreach (var item in stats)
            {
                Stats.Add(item);
            }
        }

        private void ProgressReport(double progress)
        {
            RoundProgress = progress;
        }

        private async void TestAgainstRandomCommand()
        {
            IsRunning = true;
            var numberOfRounds = 50;
            var random = new RandomPlayer();
            var progressStep = 1d / Evolution.Participants.Count;
            var progressStepNumber = 0;
            foreach (var participant in Evolution.Participants)
            {
                await Task.Factory.StartNew(() =>
                {
                    var score = 0;
                    for (var i = 0; i < numberOfRounds; i++)
                    {
                        score += NeuralHelper.ContestNetworkAndRandom(participant.Network, random);
                    }
                    participant.ScoreAgainstRandom = ScoreAgainstRandom(score, numberOfRounds);
                });
                progressStepNumber++;
                ProgressReport(progressStepNumber * progressStep);
            }
            Evolution.SaveScores();
            UpdateStats(Evolution.Stats);
            ProgressReport(0);
            IsRunning = false;
        }

        private async void TestBalanceCommand()
        {
            IsRunning = true;
            var numberOfRounds = 500;
            var random = new RandomPlayer();
            var progressStep = 1d / numberOfRounds;
            var progressStepNumber = 0;
            var score = 0;
            await Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < numberOfRounds; i++)
                {
                    score += Math.Sign(NeuralHelper.ContestTwoRandoms(random));
                }
                progressStepNumber++;
                ProgressReport(progressStepNumber * progressStep);
            });
            ProgressReport(0);
            IsRunning = false;

            var msg = $"White : Black {score} in {numberOfRounds}";
            MessageBox.Show(msg);
        }

        private static string ScoreAgainstRandom(int score, int numberOfRounds)
        {
            if (score == 0) return "0%";
            var sign = score > 0 ? "+" : "";
            return $"{sign}{(double)score / (2 * numberOfRounds):p0}";
        }
    }
}
