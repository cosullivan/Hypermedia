using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Hypermedia.Sample.WebApi
{
    public sealed class MethodConstrainingRouteProvider : DefaultDirectRouteProvider
    {
        /// <summary>
        /// Gets direct routes for the given controller descriptor and action descriptors based 
        /// on <see cref="T:System.Web.Http.Routing.IDirectRouteFactory"/> attributes.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <param name="actionDescriptors">The action descriptors for all actions.</param>
        /// <param name="constraintResolver">The constraint resolver.</param>
        /// <returns>A set of route entries.</returns>
        public override IReadOnlyList<RouteEntry> GetDirectRoutes(
            HttpControllerDescriptor controllerDescriptor,
            IReadOnlyList<HttpActionDescriptor> actionDescriptors,
            IInlineConstraintResolver constraintResolver)
        {
            var routes = base.GetDirectRoutes(controllerDescriptor, actionDescriptors, constraintResolver);

            foreach (var route in routes)
            {
                var methods = GetSupportedMethods(route.Route).ToArray();

                route.Route.Constraints.Add("httpMethod", new HttpMethodConstraint(methods));
            }

            return routes;
        }

        /// <summary>
        /// Gets the support methods that are defined on the route.
        /// </summary>
        /// <param name="route">The route to return the supported methods from.</param>
        /// <returns>The list of supported methods that are defined on the route.</returns>
        static IEnumerable<HttpMethod> GetSupportedMethods(IHttpRoute route)
        {
            if (route.DataTokens.ContainsKey("actions") == false)
            {
                return Enumerable.Empty<HttpMethod>();
            }

            var actions = (HttpActionDescriptor[])route.DataTokens["actions"];

            return actions.SelectMany(a => a.SupportedHttpMethods.OfType<HttpMethod>()).Distinct();
        }
    }
}
