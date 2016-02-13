//using System;

//namespace Hypermedia.Metadata.Runtime
//{
//    public sealed class RuntimeResourceContractResolver : IResourceContractResolver
//    {
//        /// <summary>
//        /// Attempt to resolve the resource contract from a CLR type.
//        /// </summary>
//        /// <param name="type">The CLR type of the resource contract to resolve.</param>
//        /// <param name="contract">The resource contract that was associated with the given CLR type.</param>
//        /// <returns>true if the resource contract could be resolved, false if not.</returns>
//        public bool TryResolve(Type type, out IResourceContract contract)
//        {
//            contract = RuntimeResourceContract.CreateRuntimeType(type);

//            return true;
//        }

//        /// <summary>
//        /// Attempt to resolve the resource contract from a resource type name.
//        /// </summary>
//        /// <param name="name">The resource type name of the resource contract to resolve.</param>
//        /// <param name="contract">The resource contract that was associated with the given resource type name.</param>
//        /// <returns>true if the resource contract could be resolved, false if not.</returns>
//        public bool TryResolve(string name, out IResourceContract contract)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
