using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Players
{
    public class HumanPlayer : PlayerBase
    {
        public override string Name => "Human";

        public override bool ContributesToDataset => true;

        public override PlayerBase Copy()
        {
            return this;
        }
    }
}
