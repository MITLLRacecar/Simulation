using UnityEngine;

/// <summary>
/// Generates trains as series of chunks.
/// </summary>
public class TrainEntrance : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The train chunk prefab.
    /// </summary>
    [SerializeField]
    private GameObject train;

    /// <summary>
    /// The number of chunks in a train.
    /// </summary>
    [SerializeField]
    private int numChunks = 3;

    /// <summary>
    /// The speed at which the train travels.
    /// </summary>
    [SerializeField]
    private float speed = 2;

    /// <summary>
    /// The number of empty chunk-lengths to leave between trains.
    /// </summary>
    [SerializeField]
    private int numEmptyChunks = 3;
    #endregion
    
    /// <summary>
    /// The number of seconds until we handle the next chunk.
    /// </summary>
    private float counter = 0;

    /// <summary>
    /// The number of remaining chunks to wait in the current cycle.
    /// </summary>
    private int remainingChunks = 0;

    /// <summary>
    /// True if we are spawning train chunks in this cycle.
    /// </summary>
    private bool isSpawning = false;

    void Update()
    {
        this.counter -= Time.deltaTime;
        if (this.counter <= 0)
        {
            if (isSpawning)
            {
                Rigidbody rbody = GameObject.Instantiate(this.train, this.transform.position, this.transform.rotation).GetComponent<Rigidbody>();
                rbody.velocity = this.transform.forward * this.speed;
            }

            this.remainingChunks--;
            if (this.remainingChunks <= 0)
            {
                this.isSpawning = !this.isSpawning;
                this.remainingChunks = this.isSpawning ? this.numChunks : this.numEmptyChunks;
            }

            this.counter = Train.Length / this.speed;
        }
    }
}
