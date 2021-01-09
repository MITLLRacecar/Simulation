using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The extension methods for this project.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Similar to GetComponentsInChildren, except objects are returned in transform sort order.
    /// </summary>
    /// <typeparam name="T">The type to search for in our children.</typeparam>
    /// <param name="monoBehaviour">this</param>
    /// <returns>An array of all T components in all children (and grandchildren, etc.) of this gameobject in transfrom sort order.</returns>
    public static T[] GetComponentsInChildrenOrdered<T>(this MonoBehaviour monoBehaviour)
    {
        return GetComponentsInChildrenOrdered<T>(monoBehaviour.transform).ToArray();
    }

    /// <summary>
    /// Helper function for the extension GetComponentsInChildrenOrdered method.
    /// </summary>
    /// <typeparam name="T">The type to search for in the transform's children.</typeparam>
    /// <param name="transform">The parent transform in which to search.</param>
    /// <returns>A list of all T objects in the all children (and grandchildren, etc.) of this transform in transfrom sort order.</returns>
    private static List<T> GetComponentsInChildrenOrdered<T>(Transform transform)
    {
        List<T> components = new List<T>();
        foreach (Transform child in transform)
        {
            T component = child.GetComponent<T>();
            if (component != null)
            {
                components.Add(component);
            }

            // Recursively search the child's children
            if (child.transform.childCount > 0)
            {
                components.AddRange(GetComponentsInChildrenOrdered<T>(child.transform));
            }
        }
        return components;
    }
}
