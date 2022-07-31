using Godot;
using System;

namespace ExtensionMethods
{
    /// <summary>
    /// Extension methods for the Node class.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Gets all of the node's children that are of the specified type
        /// </summary>
        /// <param name="node"> The object that this method is being called for. </param>
        /// <typeparam name="T"> The type to look for. </typeparam>
        /// <returns> Returns an array of references to node's children. </returns>
        public static Godot.Collections.Array GetChildrenOfType<T>(this Node node) where T : Node
        {
            Godot.Collections.Array toReturn = new Godot.Collections.Array();
            foreach (Node child in node.GetChildren())
            {
                if (child is T)
                {
                    toReturn.Add(child);
                }
            }
            return toReturn;
        }

        public static int dummy(this Node node)
        {
            return 2;
        }
    }
}