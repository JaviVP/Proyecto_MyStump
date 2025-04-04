using UnityEngine;

[System.Serializable]
public class UnitPlacement
{
    public Vector2Int position;  // Where the unit is placed
    public UnitType unitType;  // Enum to select unit type
}

public enum UnitType { None, Runner, Terraformer, Punchulina, Base }  // Unit types
