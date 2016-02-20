using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.StackOverflow
{
    public sealed class StackOverflowUserRepository : StackOverflowRepository<User>, IUserRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="users">The list of users for the repository.</param>
        public StackOverflowUserRepository(IEnumerable<User> users) : base(users) { }

        /// <summary>
        /// Returns a user repository from the given XML file.
        /// </summary>
        /// <param name="file">The name of the file that contains the users.</param>
        /// <returns>The user repository populated from the file.</returns>
        public static StackOverflowUserRepository FromXml(string file)
        {
            var document = new XmlDocument();
            document.Load(file);

            return new StackOverflowUserRepository(FromXml(document));
        }

        /// <summary>
        /// Returns a sequence of users from the XML document.
        /// </summary>
        /// <param name="document">The XML document to return the users from.</param>
        /// <returns>The list of users that were created from the document.</returns>
        static IEnumerable<User> FromXml(XmlDocument document)
        {
            return document.SelectNodes("//users/row").OfType<XmlNode>().Select(FromXml);
        }

        /// <summary>
        /// Returns a user from an XML node..
        /// </summary>
        /// <param name="node">The node to return the user from.</param>
        /// <returns>The user that was created from the node.</returns>
        static User FromXml(XmlNode node)
        {
            return new User
            {
                Id = node.GetInt32("Id"),
                DisplayName = node.GetString("DisplayName"),
                Reputation = node.GetInt32("Reputation"),
                UpVotes = node.GetInt32("UpVotes"),
                DownVotes = node.GetInt32("DownVotes"),
                ProfileImageUrl = node.GetString("ProfileImageUrl"),
                CreationDate = node.GetDateTimeOffset("CreationDate")
            };
        }
    }
}
