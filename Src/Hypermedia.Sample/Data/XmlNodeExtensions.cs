using System;
using System.Xml;

namespace Hypermedia.Sample.Data
{
    public static class XmlNodeExtensions
    {
        /// <summary>
        /// Gets a string value from the given node's attribute.
        /// </summary>
        /// <param name="node">The node to return the value from.</param>
        /// <param name="name">The name of the attribute to return the value for.</param>
        /// <param name="defaultValue">The default value to return in the case that the attribute doesnt exist.</param>
        /// <returns>The value from the given attribute, or the default value if one doesnt exist.</returns>
        public static string GetString(this XmlNode node, string name, string defaultValue = null)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));   
            }

            if (node.Attributes[name] == null)
            {
                return defaultValue;
            }

            return node.Attributes[name].InnerText;
        }

        /// <summary>
        /// Gets an integer value from the given node's attribute.
        /// </summary>
        /// <param name="node">The node to return the value from.</param>
        /// <param name="name">The name of the attribute to return the value for.</param>
        /// <param name="defaultValue">The default value to return in the case that the attribute doesnt exist.</param>
        /// <returns>The value from the given attribute, or the default value if one doesnt exist.</returns>
        public static int GetInt32(this XmlNode node, string name, int defaultValue = 0)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Attributes[name] == null)
            {
                return defaultValue;
            }

            return Int32.Parse(node.Attributes[name].InnerText);
        }

        /// <summary>
        /// Gets an integer value from the given node's attribute.
        /// </summary>
        /// <param name="node">The node to return the value from.</param>
        /// <param name="name">The name of the attribute to return the value for.</param>
        /// <param name="defaultValue">The default value to return in the case that the attribute doesnt exist.</param>
        /// <returns>The value from the given attribute, or the default value if one doesnt exist.</returns>
        public static DateTimeOffset GetDateTimeOffset(this XmlNode node, string name, DateTimeOffset defaultValue = default(DateTimeOffset))
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Attributes[name] == null)
            {
                return defaultValue;
            }

            return DateTimeOffset.Parse(node.Attributes[name].InnerText);
        }
    }
}
