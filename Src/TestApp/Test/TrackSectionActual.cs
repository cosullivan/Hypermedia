using System;

namespace TestApp.Test
{
    public sealed class TrackSectionActual : BaseEntity
    {
        /// <summary>
        /// The ID of the section.
        /// </summary>
        public long TrackSectionId { get; set; }

        /// <summary>
        /// The track section.
        /// </summary>
        public TrackSection TrackSection { get; set; }

        /// <summary>
        /// The ID of the train that the section is assigned to.
        /// </summary>
        public long TrainJourneyId { get; set; }

        /// <summary>
        /// The train journey that the section actual is assigned to.
        /// </summary>
        public TrainJourney TrainJourney { get; set; }

        /// <summary>
        /// The starting time of the section actual.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// The ending time of the section actual.
        /// </summary>
        public DateTimeOffset EndTime { get; set; }
    }
}