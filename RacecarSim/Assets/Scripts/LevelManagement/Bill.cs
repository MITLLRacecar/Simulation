using UnityEngine;

public class Bill : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The maximum speed allowed by Bill. Any car exceeding this speed will be hit.
    /// </summary>
    [SerializeField]
    private float MaxSpeed = 0.5f;
    #endregion

    #region Constants
    private const float thrust = 100000000;
    #endregion

    private Racecar[] cars;

    private GameObject target = null;

    private Rigidbody rBody;

    private void Awake()
    {
        this.rBody = this.GetComponent<Rigidbody>();

        // Find all players in the level
        GameObject[] carObjects = GameObject.FindGameObjectsWithTag("Player");
        this.cars = new Racecar[carObjects.Length];
        for (int i = 0; i < carObjects.Length; i++)
        {
            this.cars[i] = carObjects[i].GetComponent<Racecar>();
        }
    }

    private void Update()
    {
        if (this.target == null)
        {
            foreach (Racecar car in this.cars)
            {
                if (car.Physics.LinearVelocity.magnitude > this.MaxSpeed)
                {
                    this.target = car.gameObject;
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (this.target != null)
        {
            this.transform.LookAt(this.target.transform.position);
            this.rBody.AddRelativeForce(0, 0, Bill.thrust * Time.fixedDeltaTime); 
        }
    }
}
