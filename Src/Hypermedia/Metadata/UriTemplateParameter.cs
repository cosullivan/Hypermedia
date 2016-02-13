using System;

namespace Hypermedia.Metadata
{
    public sealed class UriTemplateParameter
    {
        readonly string _name;
        readonly Func<object, object> _accessor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="accessor">The accessor function to return the parameter from the instance.</param>
        public UriTemplateParameter(string name, Func<object, object> accessor)
        {
            _name = name;
            _accessor = accessor;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the accessor function that returns the parameter from the instance.
        /// </summary>
        public Func<object, object> Accessor
        {
            get { return _accessor; }
        }
    }
}
