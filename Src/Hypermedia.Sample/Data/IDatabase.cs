namespace Hypermedia.Sample.Data
{
    public interface IDatabase
    {
        /// <summary>
        /// Gets an instance of the users repository.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Gets an instance of the posts repository.
        /// </summary>
        IPostRepository Posts { get; }
    }
}
