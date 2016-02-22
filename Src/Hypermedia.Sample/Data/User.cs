using System;

namespace Hypermedia.Sample.Data
{
    public class User : Entity
    {
        /// <summary>
        /// The users display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The reputation of the user.
        /// </summary>
        public int Reputation { get; set; }

        /// <summary>
        /// The number of up votes that the user has supplied.
        /// </summary>
        public int UpVotes { get; set; }

        /// <summary>
        /// The number of down votes that the user has supplied.
        /// </summary>
        public int DownVotes { get; set; }

        /// <summary>
        /// The URL of the users profile image.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// The date that the user was first created.
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }
    }
}
