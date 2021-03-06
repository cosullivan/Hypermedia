﻿using Hypermedia.Sample.Data;
using Hypermedia.Sample.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Hypermedia.Sample.AspNetCore.Controllers.Comments
{
    [Route("v1/comments")]
    public sealed class CreateCommentController : ResourceController<Comment, CommentResource>
    {
        /// <summary>
        /// Creates a comment.
        /// </summary>
        /// <returns>The HTTP action result that represents the result of the action.</returns>
        [HttpOptions, HttpPost, FormatFilter]
        public IActionResult Execute([FromBody] CommentResource comment)
        {
            comment.Id = 123;

            return Created("v1/comments/123", comment);
        }
    }
}