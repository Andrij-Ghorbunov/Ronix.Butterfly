using System;

namespace Ronix.Framework
{
    /// <summary>
    /// Usage: using (new FlagSetter(v => MyProperty = v)) ...
    /// <para>
    /// Sets the flag whose setter is passed to the constructor to true, and assures it will be set back to false when the scope of using is over.
    /// </para>
    /// </summary>
    public class FlagSetter : IDisposable
    {
        private readonly Action<bool> _setter;
        private readonly bool _inverted;

        public FlagSetter(Action<bool> setter, bool inverted = false)
        {
            _setter = setter;
            _inverted = inverted;
            setter(!_inverted);
        }

        public void Dispose()
        {
            _setter(_inverted);
        }
    }
}
