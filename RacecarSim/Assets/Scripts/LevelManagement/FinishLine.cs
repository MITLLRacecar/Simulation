using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    private BestTimes.Level level;

    private void Start()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<Racecar>().SetIsWinable();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Racecar player = other.attachedRigidbody.GetComponent<Racecar>();
        if (player != null)
        {
            player.HandleFinish(level);

            GameObject raceManager = GameObject.FindGameObjectWithTag("RaceManager");
            if (raceManager != null)
            {
                raceManager.GetComponent<VariableManager>().SetKeyTime(VariableManager.KeyTime.Finish, Time.time);
            }
        }
    }
}
