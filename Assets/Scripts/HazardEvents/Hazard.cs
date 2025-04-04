using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    private string name = "";
    private string description = "";
    private string lore;

   
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public string Lore { get => lore; set => lore = value; }

    public abstract void Apply();
}
