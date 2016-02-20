using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.StackOverflow
{
    public class StackOverflowPostRepository : StackOverflowRepository<Post>, IPostRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="posts">The list of posts available for the repository.</param>
        public StackOverflowPostRepository(IEnumerable<Post> posts) : base(posts) { }

        /// <summary>
        /// Returns a post repository from the given XML file.
        /// </summary>
        /// <param name="file">The name of the file that contains the posts.</param>
        /// <returns>The post repository populated from the file.</returns>
        public static StackOverflowPostRepository FromXml(string file)
        {
            var document = new XmlDocument();
            document.Load(file);

            return new StackOverflowPostRepository(FromXml(document));
        }

        /// <summary>
        /// Returns a sequence of posts from the XML document.
        /// </summary>
        /// <param name="document">The XML document to return the posts from.</param>
        /// <returns>The list of posts that were created from the document.</returns>
        static IEnumerable<Post> FromXml(XmlDocument document)
        {
            return document.SelectNodes("//posts/row").OfType<XmlNode>().Select(FromXml);
        }

        /// <summary>
        /// Returns a post from an XML node.
        /// </summary>
        /// <param name="node">The node to return the post from.</param>
        /// <returns>The post that was created from the node.</returns>
        static Post FromXml(XmlNode node)
        {
            return new Post
            {
                Id = node.GetInt32("Id"),
                PostTypeId = node.GetInt32("PostTypeId"),
                Body = node.GetString("Body"),
                OwnerUserId = node.GetInt32("OwnerUserId"),
                Score = node.GetInt32("Score"),
                ViewCount = node.GetInt32("ViewCount"),
                CommentCount = node.GetInt32("CommentCount"),
                FavoriteCount = node.GetInt32("FavoriteCount"),
                CreationDate = node.GetDateTimeOffset("CreationDate")
            };
        }

        /// <summary>
        /// Gets the list of posts that are assigned to the given user.
        /// </summary>
        /// <param name="userId">The ID of the user to return the posts for.</param>
        /// <returns>The list of posts that are assigned the given IDs.</returns>
        public IReadOnlyList<Post> GetByOwnerUserId(int userId)
        {
            return Dictionary.Values.Where(post => post.OwnerUserId == userId).ToList();
        }
    }
}
