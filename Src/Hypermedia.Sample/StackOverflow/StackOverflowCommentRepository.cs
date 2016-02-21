using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.StackOverflow
{
    public class StackOverflowCommentRepository : StackOverflowRepository<Comment>, ICommentRepository
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="comments">The list of comments available for the repository.</param>
        public StackOverflowCommentRepository(IEnumerable<Comment> comments) : base(comments) { }

        /// <summary>
        /// Returns a comment repository from the given XML file.
        /// </summary>
        /// <param name="file">The name of the file that contains the comments.</param>
        /// <returns>The comment repository populated from the file.</returns>
        public static StackOverflowCommentRepository FromXml(string file)
        {
            var document = new XmlDocument();
            document.Load(file);

            return new StackOverflowCommentRepository(FromXml(document));
        }

        /// <summary>
        /// Returns a sequence of comments from the XML document.
        /// </summary>
        /// <param name="document">The XML document to return the comments from.</param>
        /// <returns>The list of comments that were created from the document.</returns>
        static IEnumerable<Comment> FromXml(XmlDocument document)
        {
            return document.SelectNodes("//comments/row").OfType<XmlNode>().Select(FromXml);
        }

        /// <summary>
        /// Returns a comment from an XML node.
        /// </summary>
        /// <param name="node">The node to return the comment from.</param>
        /// <returns>The comment that was created from the node.</returns>
        static Comment FromXml(XmlNode node)
        {
            return new Comment
            {
                Id = node.GetInt32("Id"),
                PostId = node.GetInt32("PostId"),
                UserId = node.GetInt32("UserId"),
                Score = node.GetInt32("Score"),
                CreationDate = node.GetDateTimeOffset("CreationDate"),
                Text = node.GetString("Text")
            };
        }

        /// <summary>
        /// Gets the list of comments that are assigned to the given post id.
        /// </summary>
        /// <param name="postIds">The IDs of the post to return the comments for.</param>
        /// <returns>The list of comments that are assigned to the given list of post ids.</returns>
        public IReadOnlyList<Comment> GetByPostId(IReadOnlyList<int> postIds)
        {
            return Dictionary.Values.Where(post => postIds.Contains(post.PostId)).ToList();
        }
    }
}
