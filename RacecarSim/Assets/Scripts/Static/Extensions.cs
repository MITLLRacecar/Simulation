using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The extension methods for this project.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Similar to GetComponentsInChildren, except objects are returned in transform sort order and we only search through first-level children.
    /// </summary>
    /// <typeparam name="T">The type to search for in our children.</typeparam>
    /// <param name="monoBehaviour">this</param>
    /// <returns>An array of all T objects in the first-level children of this gameobject in transfrom sort order.</returns>
    public static T[] GetComponentsInChildrenOrdered<T>(this MonoBehaviour monoBehaviour)
    {
        List<T> components = new List<T>();
        foreach (Transform child in monoBehaviour.transform)
        {
            T component = child.GetComponent<T>();
            if (component != null)
            {
                components.Add(component);
            }
        }
        return components.ToArray();
    }
}
