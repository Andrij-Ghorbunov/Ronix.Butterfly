using Ronix.Butterfly.Wpf.Helpers;
using Ronix.Butterfly.Wpf.Views;
using Ronix.Neural;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ronix.Butterfly.Wpf.Evolution
{
    public class NamedTournament
    {
        /// <summary>
        /// The list of current participants.
        /// </summary>
        public List<Participant> Participants { get; private set; } = new List<Participant>();

        /// <summary>
        /// Fires each time the round player reports progress.
        /// </summary>
        public event Action<double> ProgressReport;

        private int[,] _scoreTable;

        public Func<CompetitionRules> Rules;

        private CompetitionRules _rules;

        public int WhiteWins { get; private set; }
        public int BlackWins { get; private set; }
        public int Draws { get; private set; }
        public string WhiteBlackStats { get; private set; }

        private List<int> _scores;

        public NamedTournament()
        {
        }

        public void Load(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.nn").ToList();
            if (files.Count == 0)
            {
                MessageBox.Show("This folder contains no neural networks!");
                return;
            }
            Participants = files.Select(it => new Participant(LayeredNeuralNetwork.LoadFromFile(it)) { FileName = Path.GetFileName(it) }).ToList();
        }

        public List<NamedTournamentScore> PlayFullRound()
        {
            _rules = Rules();
            foreach (var p in Participants)
            {
                p.Score = 0;
            }
            WhiteWins = 0;
            BlackWins = 0;
            Draws = 0;

            var count = Participants.Count;
            _scores = new List<int>(count * (count - 1));

            _scoreTable = new int[count, count];
            var totalNumberOfIterationsInRound = count * (count - 1) / 2; // Gauss sum 1+2+3+...+(count-1)
            var progressStep = 1d / totalNumberOfIterationsInRound; // used for reporting progress
            var progressStepCount = 0;
            for (var x = 0; x < count; x++)
            {
                for (var y = x + 1; y < count; y++)
                {
                    PlaySingle(Participants[x], Participants[y], x, y);
                    progressStepCount++;
                    ReportRoundProgress(progressStepCount * progressStep);
                }
            }

            var avg = _scores.Average();
            var dev = Math.Sqrt(_scores.Select(x => (x - avg) * (x - avg)).Average());
            WhiteBlackStats = $"{avg:f3}±{dev:f3}";

            var data = new List<NamedTournamentScore>();
            for (var x = 0; x < count; x++)
            {
                var p1 = Participants[x];
                var scores = new string[count];
                scores[x] = string.Empty;
                for (var y = 0; y < count; y++)
                {
                    var p2 = Participants[y];
                    if (x == y)
                    {
                        continue;
                    }
                    scores[y] = _scoreTable[x, y].ToString();
                }
                data.Add(new NamedTournamentScore(Path.GetFileNameWithoutExtension(p1.FileName), scores));
            }
            return data;
        }

        private void PlaySingle(Participant p1, Participant p2, int x, int y)
        {
            var score1 = NeuralHelper.ContestTwoNetworks(p1.Network, p2.Network);
            var score2 = NeuralHelper.ContestTwoNetworks(p2.Network, p1.Network);

            _rules.GetScores(score1, out var p11, out var p21);
            _rules.GetScores(score2, out var p22, out var p12);

            p1.Score += p11 + p12;
            p2.Score += p21 + p22;

            if (score1 > 0)
            {
                WhiteWins++;
            }
            else if (score1 < 0)
            {
                BlackWins++;
            }
            else
            {
                Draws++;
            }
            if (score2 > 0)
            {
                WhiteWins++;
            }
            else if (score2 < 0)
            {
                BlackWins++;
            }
            else
            {
                Draws++;
            }

            _scores.Add(score1);
            _scores.Add(score2);

            _scoreTable[x, y] = score1;
            _scoreTable[y, x] = score2;
        }

        private void ReportRoundProgress(double progress)
        {
            ProgressReport?.Invoke(progress);
        }
    }
}
