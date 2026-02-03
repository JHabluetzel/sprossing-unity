using UnityEngine;

[CreateAssetMenu(menuName = "Wave Function Collapse/Tile")]
public class WFCTile : ScriptableObject
{
    public WFCTile[] topOptions;
    public WFCTile[] rightOptions;
    public WFCTile[] bottomOptions;
    public WFCTile[] leftOptions;
}
