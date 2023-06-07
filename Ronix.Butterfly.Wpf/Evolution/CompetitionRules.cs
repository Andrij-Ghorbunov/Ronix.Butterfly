using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Evolution
{
    public class CompetitionRules
    {
        public int ScoreForDraw { get; set; }

        public int ScoreForWin { get; set; }

        public int WinnerScoreFactor { get; set; }

        public int WinnerDevelopmentFactor { get; set; }

        public bool WinnerDevelopmentAffectsScore { get; set; }

        public int LoserScoreFactor { get; set; }

        public int LoserDevelopmentFactor { get; set; }

        public bool LoserDevelopmentAffectsScore { get; set; }

        public int BalanceModifier { get; set; }

        public void GetScores(int score, out int white, out int black)
        {
            if (score < 0)
            {
                GetScores(-score, out var black1, out var white1);
                black = black1 + BalanceModifier;
                white = white1;
                return;
            }
            if (score == 0)
            {
                white = ScoreForDraw;
                black = ScoreForDraw;
                return;
            }
            var scoreDiff = (score + 50) / 100;
            var devDiff = score - 100 * scoreDiff;
            var winner = ScoreForWin + scoreDiff * WinnerScoreFactor;
            if (scoreDiff == 0 || WinnerDevelopmentAffectsScore)
                winner += WinnerScoreFactor * devDiff;
            var loser = -scoreDiff * LoserScoreFactor;
            if (scoreDiff == 0 || LoserDevelopmentAffectsScore)
                loser -= LoserScoreFactor * devDiff;

            white = winner;
            black = loser;
        }
    }
}
