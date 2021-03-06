﻿using System;

namespace Hypermedia.WebApi
{
    public sealed class HypermediaWebApiException : HypermediaException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message arguments.</param>
        public HypermediaWebApiException(string format, params object[] args) : base(format, args) { }
    }
}