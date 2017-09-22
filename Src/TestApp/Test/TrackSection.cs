namespace TestApp.Test
{
    public sealed class TrackSection : BaseEntity
    {
        /// <summary>
        /// The name of the track section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The starting position (in KM) of the section.
        /// </summary>
        public decimal StartKm { get; set; }

        /// <summary>
        /// The ending position (in KM) of the section.
        /// </summary>
        public decimal EndKm { get; set; }

        ///// <summary>
        ///// The list of source system mapping information for the track section.
        ///// </summary>
        //public StringDictionary Mapping { get; set; }
    }
}