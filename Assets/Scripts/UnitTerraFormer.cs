using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UnitTerraFormer : Unit
{
    private HexGrid hexGrid;
    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void OnSelected()
    {
        List<HexTile> tiles = hexGrid.GetTilesWithinRange(this.AxialCoords, 1);
        if (tiles.Count > 0)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                //Pruebas
                tiles[i].tileRenderer.material.color = Color.yellow;
            }

        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }

}
