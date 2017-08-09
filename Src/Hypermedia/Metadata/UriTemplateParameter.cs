using System;

namespace Hypermedia.Metadata
{
    public sealed class UriTemplateParameter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="accessor">The accessor function to return the parameter from the instance.</param>
        public UriTemplateParameter(string name, Func<object, object> accessor)
        {
            Name = name;
            Accessor = accessor;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the accessor function that returns the parameter from the instance.
        /// </summary>
        public Func<object, object> Accessor { get; }
    }
}