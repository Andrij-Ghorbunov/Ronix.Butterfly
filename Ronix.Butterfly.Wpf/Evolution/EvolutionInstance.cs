using Ronix.Butterfly.Wpf.Helpers;
using Ronix.Butterfly.Wpf.Views;
using Ronix.Neural;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Ronix.Butterfly.Wpf.Evolution
{
    /// <summary>
    /// Represents a group of neural networks competing against each other to win the Butterfly game.
    /// </summary>
    public class EvolutionInstance
    {
        #region Fields and events

        /// <summary>
        /// Common source of randomness for the genetic mutations.
        /// </summary>
        private static Random Random = new();

        /// <summary>
        /// The list of current participants.
        /// </summary>
        public List<Participant> Participants { get; private set; } = new List<Participant>();

        /// <summary>
        /// Fires each time the round player reports progress.
        /// </summary>
        public event Action<double> ProgressReport;

        /// <summary>
        /// Rules of determining player scores in multiple games.
        /// </summary>
        public Func<CompetitionRules> Rules;

        private CompetitionRules _rules;

        #endregion Fields and events

        #region Macroparameters

        /// <summary>
        /// How many participants to generate upon initialization.
        /// </summary>
        public int ParticipantCount = 30;
        
        /// <summary>
        /// How many top participants in each round are replicated with mutations to replace the same number of bottom participants which leave the tournament.
        /// </summary>
        public int TopCut = 5;
        
        /// <summary>
        /// How many additional bottom participants are replaced with newly generated networks.
        /// </summary>
        public int FreshBlood = 2;
        
        /// <summary>
        /// How many mutations to introduce in each offspring of a champion.
        /// </summary>
        public int NumberOfMutations = 1;

        /// <summary>
        /// Absolute amplitude of a single mutation in an offspring of a champion.
        /// </summary>
        public double AmplitudeOfMutations = 0.1;

        /// <summary>
        /// A way to create new participants for the fresh blood.
        /// </summary>
        public Func<Random, LayeredNeuralNetwork> ParticipantGenerator = Generators.Default;

        #endregion Macroparameters

        #region Methods

        /// <summary>
        /// Create initial list of participants, using the generator.
        /// </summary>
        public void Init()
        {
            Participants.Clear();
            for (var index = 0; index < ParticipantCount; index++)
            {
                Participants.Add(new Participant(ParticipantGenerator(Random)));
            }
        }

        /// <summary>
        /// Play a full round, then kill losers and replicate champions.
        /// </summary>
        public void Generation()
        {
            PlayFullRound();
            KillAndReplicate();
            if (SaveAfterRound) Save();
        }

        /// <summary>
        /// Play two games between each pair of participants (one for white and one for black pieces).
        /// </summary>
        public void PlayFullRound()
        {
            var count = Participants.Count;
            var totalNumberOfIterationsInRound = count * (count - 1) / 2; // Gauss sum 1+2+3+...+(count-1)
            var progressStep = 1d / totalNumberOfIterationsInRound; // used for reporting progress
            var progressStepCount = 0;
            _rules = Rules();
            for (var x = 0; x < count; x++)
            {
                for (var y = x + 1; y < count; y++)
                {
                    Play(Participants[x], Participants[y]);
                    Play(Participants[y], Participants[x]);
                    progressStepCount++;
                    ReportRoundProgress(progressStepCount * progressStep);
                }
            }
        }

        /// <summary>
        /// Remove bottom participants, replicate top participants, and add fresh blood.
        /// </summary>
        public void KillAndReplicate()
        {
            var rangedList = Participants.OrderBy(it => it.Score).Skip(TopCut + FreshBlood).ToList();
            rangedList.Reverse();
            var champions = rangedList.Take(TopCut).ToList();
            foreach (var participant in rangedList)
            {
                participant.Network.SurvivedGenerations++;
                participant.Network.TotalGenerationNumber++;
            }
            var middleParticipants = rangedList.Except(champions).ToList();
            foreach (var champion in champions)
            {
                var offspring = champion.Network.Offspring(Random, NumberOfMutations, AmplitudeOfMutations);
                offspring.LastEvolutionHistory = Append(offspring.LastEvolutionHistory, "*");
                rangedList.Add(new Participant(offspring));
                champion.Network.LastEvolutionHistory = Append(champion.Network.LastEvolutionHistory, "+");
            }
            foreach (var participant in middleParticipants)
            {
                participant.Network.LastEvolutionHistory = Append(participant.Network.LastEvolutionHistory, "=");
            }
            for (var i = 0; i < FreshBlood; i++)
            {
                var newNetwork = ParticipantGenerator(Random);
                rangedList.Add(new Participant(newNetwork));
            }
            SaveScores();
            foreach (var participant in rangedList)
            {
                participant.Score = 0;
            }
            Participants = rangedList;
        }

        private string Append(string source, string suffix, int max = 10)
        {
            var r = source + suffix;
            if (r.Length <= max) return r;
            return r.Substring(r.Length - max);
        }

        private void Play(Participant p1, Participant p2)
        {
            var score = NeuralHelper.ContestTwoNetworks(p1.Network, p2.Network);
            _rules.GetScores(score, out var p1score, out var p2score);
            p1.Score += p1score;
            p2.Score += p2score;
        }

        private void ReportRoundProgress(double progress)
        {
            ProgressReport?.Invoke(progress);
        }

        #endregion Methods

        #region Constructor

        public EvolutionInstance()
        {
            Stats = new List<StatisticsItem>();
        }

        #endregion Constructor

        #region Statistics

        public List<StatisticsItem> Stats { get; private set; }

        public void SaveScores()
        {
            Stats = Participants.Select(it => new StatisticsItem
            {
                Score = it.Score,
                GenerationNumber = it.Network.GenerationNumber,
                TotalGenerationNumber = it.Network.TotalGenerationNumber,
                SurvivedGenerations = it.Network.SurvivedGenerations,
                Layers = it.Network.LayerConfiguration,
                LastEvolutionHistory = it.Network.LastEvolutionHistory,
                FileName = it.FileName,
                Comments = it.Network.Comments,
                ScoreAgainstRandom = it.ScoreAgainstRandom
            }).OrderByDescending(it => it.Score).ToList();
        }

        #endregion Statistics

        #region Save and load

        public bool SaveAfterRound = true;

        public string DirectoryPath;

        public void Save()
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            var index = 1;
            var names = new List<string>();
            foreach (var participant in Participants)
            {
                var filename = $"network-{index++}.nn";
                names.Add(filename);
                using var file = new FileStream(Path.Combine(DirectoryPath, filename), FileMode.Create);
                participant.Network.Save(file);
                participant.FileName = filename;
                file.Close();
            }
            // clean up possible previous files
            foreach (var path in Directory.EnumerateFiles(DirectoryPath, "*.nn").ToList())
            {
                if (!names.Contains(Path.GetFileName(path)))
                {
                    File.Delete(path);
                }
            }
        }

        public void Load()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            var files = Directory.EnumerateFiles(DirectoryPath, "*.nn").ToList();
            if (files.Count == 0)
            {
                Init();
                Save();
                MessageBox.Show($"Generated {ParticipantCount} new networks to start with!");
                return;
            }
            Participants = files.Select(it => new Participant(LayeredNeuralNetwork.LoadFromFile(it)) { FileName = Path.GetFileName(it) }).ToList();
            if (files.Count != ParticipantCount)
            {
                if (files.Count > ParticipantCount)
                {
                    MessageBox.Show($"There are more .nn files in the folder ({files.Count}) than the expected number of participants in the tournament ({ParticipantCount}); tournament extended to accomodate all of them");
                    ParticipantCount = files.Count;
                }
                else
                {
                    MessageBox.Show($"There are less .nn files in the folder ({files.Count}) than the expected number of participants in the tournament ({ParticipantCount}); missing participants autogenerated");
                    var missing = ParticipantCount - files.Count;
                    for (var i = 0; i < missing; i++)
                    {
                        Participants.Add(new Participant(ParticipantGenerator(Random)));
                    }
                }
            }
            SaveScores();
        }

        #endregion Save and load
    }
}
