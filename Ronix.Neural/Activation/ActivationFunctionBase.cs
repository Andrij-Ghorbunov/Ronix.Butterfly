namespace Ronix.Neural.Activation
{
    public abstract class ActivationFunctionBase
    {
        public string Name { get; }
        public string Description { get; }
        public abstract double Value(double arg);
        public abstract double Derivative(double arg);
        /// <summary>
        /// <c>True</c> for functions whose values lie between -1 and +1 (or between 0 and 1);
        /// <c>false</c> for functions which can have arbitrarily high values.
        /// </summary>
        public virtual bool IsBounded => true;

        protected ActivationFunctionBase(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
