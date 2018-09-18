using System;

namespace Hypermedia
{
    internal sealed class DelegatingDisposable : IDisposable
    {
        readonly Action _delegate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delegate">The delegate to call when being disposed.</param>
        internal DelegatingDisposable(Action @delegate)
        {
            _delegate = @delegate;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _delegate();
        }
    }
}