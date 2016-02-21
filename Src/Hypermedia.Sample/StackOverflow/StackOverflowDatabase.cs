using System.IO;
using Hypermedia.Sample.Data;

namespace Hypermedia.Sample.StackOverflow
{
    public sealed class StackOverflowDatabase : IDatabase
    {
        readonly string _folder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="folder">The base folder for the XML dumps.</param>
        public StackOverflowDatabase(string folder)
        {
            _folder = folder;
        }

        /// <summary>
        /// Gets an instance of the users repository.
        /// </summary>
        public IUserRepository Users
        {
            get { return StackOverflowUserRepository.FromXml(Path.Combine(_folder, "users.xml")); }
        }

        /// <summary>
        /// Gets an instance of the posts repository.
        /// </summary>
        public IPostRepository Posts
        {
            get {  return StackOverflowPostRepository.FromXml(Path.Combine(_folder, "posts.xml")); }
        }

        /// <summary>
        /// Gets an instance of the comments repository.
        /// </summary>
        public ICommentRepository Comments
        {
            get { return StackOverflowCommentRepository.FromXml(Path.Combine(_folder, "comments.xml")); }
        }
    }
}
