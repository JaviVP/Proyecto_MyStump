using UnityEngine;

public abstract class HazardOLD
{
    private int id;
    private string name = "";
    private string description = "";
    private string lore;
    private Sprite image;
    private int duration;
    //private int applyTurn;


   
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public string Lore { get => lore; set => lore = value; }
    public int Id { get => id; set => id = value; }
    public int Duration { get => duration; set => duration = value; }

    public abstract void Apply();
}
