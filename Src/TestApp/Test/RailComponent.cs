using System.Collections.Generic;

namespace TestApp.Test
{
    public sealed class RailComponent : BaseEntity
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the rail model that the component is assigned to.
        /// </summary>
        public long ModelId { get; set; }

        /// <summary>
        /// The rail model that the component is assigned to.
        /// </summary>
        public RailModel Model { get; set; }

        ///// <summary>
        ///// The list of attributes for the component.
        ///// </summary>
        //public StringDictionary Attributes { get; set; }

        /// <summary>
        /// The list of track sections associated to the component.
        /// </summary>
        public IReadOnlyList<TrackSection> Sections { get; set; }
    }
}