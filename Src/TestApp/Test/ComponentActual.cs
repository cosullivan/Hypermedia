using System;

namespace TestApp.Test
{
    public sealed class ComponentActual : BaseEntity
    {
        /// <summary>
        /// The ID of the actual component.
        /// </summary>
        public long ComponentId { get; set; }

        /// <summary>
        /// The rail component.
        /// </summary>
        public RailComponent Component { get; set; }

        /// <summary>
        /// The ID of the train that the actual is assigned to.
        /// </summary>
        public long TrainId { get; set; }

        /// <summary>
        /// The train journey that the component actual is assigned to.
        /// </summary>
        public TrainJourney TrainJourney { get; set; }

        /// <summary>
        /// The starting time of the component actual.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// The ending time of the component actual.
        /// </summary>
        public DateTimeOffset EndTime { get; set; }
    }
}