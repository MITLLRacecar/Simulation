using UnityEngine;

public class ArTagToggle : MonoBehaviour
{
    #region Constants
    private static readonly Color[] colors = new Color[] { Color.white, Color.red, new Color(0, 0.5f, 1), Color.green };

    private const float scrollScale = 10;
    #endregion

    #region Set in Unity Editor
    [SerializeField]
    private Material[] tags;

    [SerializeField]
    private int tagIndex = 0;
    #endregion

    private Renderer outline;

    private Renderer tag;

    private int colorIndex = 0;

    private bool selected = false;

    private void Awake()
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        this.outline = renderers[0];
        this.tag = renderers[1];
    }

    private void Start()
    {
        this.tag.material = tags[tagIndex];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            this.selected = Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject;
        }

        if (this.selected)
        {
            if (Input.GetMouseButtonDown(1))
            {
                this.colorIndex++;
                this.outline.material.SetColor("_Color", colors[colorIndex % colors.Length]);
            }

            this.transform.Rotate(0, 0, ArTagToggle.scrollScale * Input.mouseScrollDelta[1]);

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                this.tagIndex += tags.Length - 1;
                this.tagIndex %= tags.Length;
                this.tag.material = tags[tagIndex];
            }

            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                this.tagIndex++;
                this.tagIndex %= tags.Length;
                this.tag.material = tags[tagIndex];
            }
        }
    }
}
