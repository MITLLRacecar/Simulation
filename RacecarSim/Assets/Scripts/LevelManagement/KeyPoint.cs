using System;
using UnityEngine;

/// <summary>
/// A key point in a race such as a checkpoint or finish line.
/// </summary>
public class KeyPoint : MonoBehaviour, IComparable<KeyPoint>
{
    #region Set in Unity Editor
    /// <summary>
    /// The type of key point (start, checkpoint, etc.).
    /// </summary>
    [SerializeField]
    private KeyPointType type;

    /// <summary>
    /// The index of the checkpoint. Not used for finish line.
    /// </summary>
    [SerializeField]
    private int checkpointIndex = 0;
    #endregion

    #region Public Interface
    /// <summary>
    /// The types of key points in a race.
    /// </summary>
    public enum KeyPointType
    {
        Start,
        Checkpoint,
        Finish
    }

    /// <summary>
    /// The type of key point (start, checkpoint, etc.).
    /// </summary>
    public KeyPointType Type { get { return this.type; } }

    public int CompareTo(KeyPoint other)
    {
        // Ordered as start, checkpoints (by index order), finish
        if (this.Type != other.Type)
        {
            return this.Type - other.Type;
        }
        return this.checkpointIndex - other.checkpointIndex;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        Racecar car = other.attachedRigidbody.GetComponent<Racecar>();
        if (car != null)
        {
            switch (this.Type)
            {
                case KeyPointType.Finish:
                    LevelManager.HandleFinish(car.Index);
                    break;

                case KeyPointType.Checkpoint:
                    LevelManager.HandleCheckpoint(car.Index, this.checkpointIndex);
                    break;
            }
        }
    }
}
