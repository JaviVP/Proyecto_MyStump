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
        List<HexTile> tilesSelected = null; //
        List<HexTile> tilesCheck = null; //
        tilesSelected = new List<HexTile>();
        tilesCheck = new List<HexTile>();
        tilesCheck.Add(hexGrid.GetHexTile(this.AxialCoords));
        int contador = 100;
        while (tilesCheck.Count > 0 && contador >0)
        {

            List<HexTile> tiles = hexGrid.GetTilesWithinRange(tilesCheck[0].axialCoords, 1);
            tilesCheck.RemoveAt(0);
            contador--;
            if (tiles.Count > 0)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    if (contador==99 && tiles[i].state == HexState.Neutral)
                    {
                        if (!tilesSelected.Contains(tiles[i]))
                        {
                            tilesSelected.Add(tiles[i]);
                        }
                      
                    }

                    if (tiles[i].state == HexState.Termites && hexGrid.GetUnitInTile(tiles[i].axialCoords) == null)
                    {

                        //tiles[i].HighlightTile(Color.yellow);
                        if (!tilesCheck.Contains(tiles[i]))
                        {
                            tilesCheck.Add(tiles[i]);
                        }
                        if (!tilesSelected.Contains(tiles[i]))
                        {
                            tilesSelected.Add(tiles[i]);
                        }






                    }
                    else
                    {
                        tiles[i].HighlightTile(Color.magenta);
                    }

                }

            }
        }
        if (tilesSelected.Count > 0)
        {
            for (int i = 0; i < tilesSelected.Count; i++)
            {
                tilesSelected[i].HighlightTile(Color.yellow);
            }
        }





        
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }

}
