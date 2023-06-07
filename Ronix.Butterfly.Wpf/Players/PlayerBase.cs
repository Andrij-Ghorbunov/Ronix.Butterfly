using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Players
{
    public abstract class PlayerBase
    {
        public virtual bool NeedsFileBrowsing => false;

        public virtual bool ContributesToDataset => false;

        public abstract string Name { get; }

        public virtual string Description => Name;

        public virtual int? Prompt(ButterflyGame game)
        {
            return null;
        }

        public virtual void Load(string path)
        {
        }

        public abstract PlayerBase Copy();
    }
}
