using UnityEngine;

public class Structure : MonoBehaviour
{
    public Vector2Int size;
    public Vector2Int bottomLeft;

    public void SetLayer(int baseLayer)
    {
        GetComponent<SpriteRenderer>().sortingOrder = baseLayer;

        int childIndex = 0;
        foreach (Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().sortingOrder = baseLayer + childIndex * 4;
            childIndex++;
        }
    }
}
