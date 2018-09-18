using System;
using System.Collections.Generic;

namespace Hypermedia.Json
{
    internal static class StackExtensions
    {
        /// <summary>
        /// Add the item to the stack and return a disposable that will remove it.
        /// </summary>
        /// <param name="stack">The stack to add the items to.</param>
        /// <param name="value">The value to add to the stack.</param>
        /// <returns>A disposable that will pop the item from the stack.</returns>
        internal static IDisposable Visit(this Stack<object> stack, object value)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            stack.Push(value);

            return new DelegatingDisposable(()=> stack.Pop());
        }
    }
}