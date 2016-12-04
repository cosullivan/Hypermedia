namespace Hypermedia.Sample.Data
{
    public class Comment : Entity
    {
        /// <summary>
        /// Gets or sets the ID of the post that the comment is assigned to.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user that made the comment.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the score for the comment.
        /// </summary>
        public int Score { get; set; }
        
        /// <summary>
        /// Gets or sets the text for the comment.
        /// </summary>
        public string Text { get; set; }
    }
}
