using UnityEngine;

public class KeyPoint : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// Whether this key point is the finish line (true) or a checkpoint (false)
    /// </summary>
    [SerializeField]
    bool isFinishLine = true;

    /// <summary>
    /// The index of the checkpoint. Not used for finish line.
    /// </summary>
    [SerializeField]
    int index = 0;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        Racecar car = other.attachedRigidbody.GetComponent<Racecar>();
        if (car != null)
        {
            if (this.isFinishLine)
            {
                LevelManager.HandleFinish(car.Index);
            }
            else
            {
                LevelManager.HandleCheckpoint(this.index, car.Index, this.transform.position, this.transform.rotation.eulerAngles);
            }
        }
    }
}
