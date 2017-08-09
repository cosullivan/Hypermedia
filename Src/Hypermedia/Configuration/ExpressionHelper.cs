using System;
using System.Linq.Expressions;

namespace Hypermedia.Configuration
{
    internal static class ExpressionHelper
    {
        /// <summary>
        /// Gets the name of the member that is defined in the expression tree.
        /// </summary>
        /// <param name="expression">The expression that defines a member access expression.</param>
        /// <returns>The name of the member that is referenced in the member access expression.</returns>
        internal static string GetMemberNameFromExpression(Expression expression)
        {
            if (expression is MemberExpression)
            {
                return ((MemberExpression)expression).Member.Name;
            }

            if (expression is UnaryExpression)
            {
                return GetMemberNameFromExpression(((UnaryExpression)expression).Operand);
            }

            if (expression is LambdaExpression)
            {
                return GetMemberNameFromExpression(((LambdaExpression)expression).Body);
            }

            throw new ArgumentException("The expression must be a member access expression.");
        }
    }
}