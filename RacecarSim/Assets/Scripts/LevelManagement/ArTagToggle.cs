using UnityEngine;

public class ArTagToggle : MonoBehaviour
{
    #region Constants
    private static readonly Color[] colors = new Color[] { Color.white, Color.red, new Color(0, 0.5f, 1), Color.green };
    #endregion

    private Renderer tagRenderer;

    private int colorIndex = 0;

    private int rotation = 0;

    private void Awake()
    {
        this.tagRenderer = this.GetComponentInChildren<Renderer>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rotation = (rotation - 90) % 360;
            this.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }

        if (Input.GetMouseButtonUp(1))
        {
            this.colorIndex++;
            this.tagRenderer.material.SetColor("_Color", colors[colorIndex % colors.Length]);
        }
    }
}
