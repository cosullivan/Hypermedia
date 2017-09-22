using System.Collections.Generic;

namespace TestApp.Test
{
    public sealed class TrainJourney : BaseEntity
    {
        /// <summary>
        /// The train id.
        /// </summary>
        public long TrainId { get; set; }

        /// <summary>
        /// The component actuals for the train journey.
        /// </summary>
        public IReadOnlyList<ComponentActual> ComponentActuals { get; set; }

        /// <summary>
        /// The list of rail section actuals.
        /// </summary>
        public IReadOnlyList<TrackSectionActual> SectionActuals { get; set; }
    }
}