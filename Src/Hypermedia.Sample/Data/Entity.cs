using System;

namespace Hypermedia.Sample.Data
{
    public abstract class Entity
    {
        /// <summary>
        /// Gets or sets the ID of the resource.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The creation date and time of the entity.
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }
    }
}
