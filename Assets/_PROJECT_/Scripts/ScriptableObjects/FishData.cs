using UnityEngine;

[CreateAssetMenu(fileName = "Fish Data", menuName = "Inventory/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int baseSellPrice;
}