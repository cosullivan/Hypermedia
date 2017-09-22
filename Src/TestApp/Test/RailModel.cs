using System.Collections.Generic;

namespace TestApp.Test
{
    public sealed class RailModel : BaseEntity
    {
        /// <summary>
        /// The name of the model.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of components assigned to this model.
        /// </summary>
        public IReadOnlyList<RailComponent> Components { get; set; }
    }
}