using Ronix.Butterfly.Wpf.Evolution;
using Ronix.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Views
{
    public class CompetitionRulesVm: ViewModelBase
    {
        private int _scoreForDraw = 1;

        public int ScoreForDraw
        {
            get => _scoreForDraw;
            set => SetValue(ref _scoreForDraw, value);
        }

        private int _scoreForWin = 3;

        public int ScoreForWin
        {
            get => _scoreForWin;
            set => SetValue(ref _scoreForWin, value);
        }

        private int _winnerScoreFactor = 0;

        public int WinnerScoreFactor
        {
            get => _winnerScoreFactor;
            set => SetValue(ref _winnerScoreFactor, value);
        }

        private int _winnerDevelopmentFactor = 0;

        public int WinnerDevelopmentFactor
        {
            get => _winnerDevelopmentFactor;
            set => SetValue(ref _winnerDevelopmentFactor, value);
        }

        private bool _winnerDevelopmentAffectsScore = false;

        public bool WinnerDevelopmentAffectsScore
        {
            get => _winnerDevelopmentAffectsScore;
            set => SetValue(ref _winnerDevelopmentAffectsScore, value);
        }


        private int _loserScoreFactor = 0;

        public int LoserScoreFactor
        {
            get => _loserScoreFactor;
            set => SetValue(ref _loserScoreFactor, value);
        }

        private int _loserDevelopmentFactor = 0;

        public int LoserDevelopmentFactor
        {
            get => _loserDevelopmentFactor;
            set => SetValue(ref _loserDevelopmentFactor, value);
        }

        private bool _loserDevelopmentAffectsScore = false;

        public bool LoserDevelopmentAffectsScore
        {
            get => _loserDevelopmentAffectsScore;
            set => SetValue(ref _loserDevelopmentAffectsScore, value);
        }

        private int _balanceModifier = 0;

        public int BalanceModifier
        {
            get => _balanceModifier;
            set => SetValue(ref _balanceModifier, value);
        }

        public CompetitionRules GetModel()
        {
            return new CompetitionRules
            {
                ScoreForDraw = ScoreForDraw,
                ScoreForWin = ScoreForWin,
                WinnerScoreFactor = WinnerScoreFactor,
                WinnerDevelopmentFactor = WinnerDevelopmentFactor,
                WinnerDevelopmentAffectsScore = WinnerDevelopmentAffectsScore,
                LoserScoreFactor = LoserScoreFactor,
                LoserDevelopmentFactor = LoserDevelopmentFactor,
                LoserDevelopmentAffectsScore = LoserDevelopmentAffectsScore,
                BalanceModifier = BalanceModifier,
            };
        }

        public void GetScores(int score, out int white, out int black)
        {
            if (score < 0)
            {
                GetScores(-score, out var black1, out var white1);
                black = black1 + _balanceModifier;
                white = white1;
                return;
            }
            if (score == 0)
            {
                white = _scoreForDraw;
                black = _scoreForDraw;
                return;
            }
            var scoreDiff = (score + 50) / 100;
            var devDiff = score - 100 * scoreDiff;
            var winner = _scoreForWin + scoreDiff * _winnerScoreFactor;
            if (scoreDiff == 0 || WinnerDevelopmentAffectsScore)
                winner += _winnerScoreFactor * devDiff;
            var loser = -scoreDiff * _loserScoreFactor;
            if (scoreDiff == 0 || LoserDevelopmentAffectsScore)
                loser -= _loserScoreFactor * devDiff;

            white = winner;
            black = loser;
        }
    }
}
