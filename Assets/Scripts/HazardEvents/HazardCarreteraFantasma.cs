using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HazardCarreteraFantasma : HazardOLD
{
    /*Efecto: Hasta 4 casillas al azar de la zona interior se convierten en territorio del jugador con menos casillas pintadas.
    (Si hay menos de 4 casillas pintadas en la zona exterior y este evento se lanza mirará el número de casillas pintadas actuales e irá borrando casillas[Mínimo de casillas pintadas = 1, Máximo de casillas pintadas = 4]) 
        Casos: 
            2 casillas jugador → 2 casillas neutrales 
            1 casillas jugador → 3 casillas neutrales 
            3 casillas jugador → 1 casillas neutrales

    Inspiración: Carreteras como la Transamazónica han facilitado la colonización y deforestación, favoreciendo a los que ya tienen poder.
    */

    public override void Apply()
    {
        int affectedTiles = 4;
       // int affectedTurns = 1; //No se usa de momento lo comento


        //Debug.Log(" 4 casillas del que más tiene se vuelve a las ");
        //Interno
        List<HexTile> innerList = GameManager.Instance.HexGrid.GetTilesWithinRange(new Vector2Int(0,0), 0, 2);
        Debug.Log("Cantidad: "+innerList.Count);
        innerList = RamdomList(innerList);
        int countAntsTiles = GameManager.Instance.HexGrid.GetCountStateTiles(HexState.Ants);
        int countTermitesTiles = GameManager.Instance.HexGrid.GetCountStateTiles(HexState.Termites);

        Debug.Log(countTermitesTiles + "--- " + countAntsTiles);

        List<HexTile> antList= new List<HexTile>();
        List<HexTile> termitesList = new List<HexTile>();
        List<HexTile> neutralList = new List<HexTile>();
        if (innerList != null && innerList.Count > 0)
        {
            foreach (HexTile tile in innerList)
            {
                if (tile != null)
                {
                    if (tile.state == HexState.Ants)
                    {
                        antList.Add(tile);
                        
                    }
                    else if (tile.state == HexState.Termites)
                    {
                       termitesList.Add(tile);  
                    }
                    else if (tile.state == HexState.Neutral)
                    {
                        neutralList.Add(tile);
                    }
                    // tile.ChangeColor(Color.blue);
                }
            }
            antList = RamdomList(antList);
            termitesList=RamdomList(termitesList);
            neutralList = RamdomList(neutralList);
            if (countAntsTiles> countTermitesTiles)
            {
                Debug.Log("ANT>TERM");
                //There are more ants
                int res = antList.Count - affectedTiles;
                if (res > 0)
                {
                    
                    //Unordered
                    for (int i = 0; i < antList.Count; i++)
                    {
                        if (antList[i] != null)
                        {
                            antList[i].state = HexState.Termites;
                            antList[i].ChangeColor(Color.green);
                        }
                    }
                }
                else
                {
                   
                    //Unordered
                    for (int i = 0; i < antList.Count; i++)
                    {
                        if (antList[i]!=null)
                        {
                            antList[i].state = HexState.Termites;
                            antList[i].ChangeColor(Color.green);
                        }
                    }
                    for (int i = 0; i < Mathf.Abs(res); i++)
                    {
                        //Unordered
                        if (neutralList[i]!=null)
                        {
                            neutralList[i].state = HexState.Termites;
                            neutralList[i].ChangeColor(Color.green);
                        }
                    }
                }

            }
            else if (countAntsTiles < countTermitesTiles)
            {
                Debug.Log("ANT<TERM");
                //There are more termites
                //Termites
                int res = termitesList.Count - affectedTiles;
                if (res > 0)
                {
                    //Unordered
                    for (int i = 0; i < affectedTiles; i++)
                    {
                        if (termitesList[i]!=null)
                        {
                            termitesList[i].state = HexState.Ants;
                            termitesList[i].ChangeColor(Color.red);
                        }
                        

                    }
                }
                else
                {
                    //Unordered
                    for (int i = 0; i < termitesList.Count; i++)
                    {
                        if (termitesList[i] != null)
                        {
                            termitesList[i].state = HexState.Ants;
                            termitesList[i].ChangeColor(Color.red);
                        }
                    }
                    for (int i = 0; i < Mathf.Abs(res); i++)
                    {
                        //Unordered
                        if (neutralList[i] != null)
                        {
                            neutralList[i].state = HexState.Ants;
                            neutralList[i].ChangeColor(Color.red);
                        }

                    }
                }
            }
            else
            {
                Debug.Log("ANT==TERM");
                //Equals
                //Ants
                int res = antList.Count - affectedTiles;
                if (res>0)
                {
                    //Unordered
                    for (int i = 0; i< antList.Count; i++)
                    {
                        if (antList[i] != null)
                        {
                            antList[i].state = HexState.Termites;
                            antList[i].ChangeColor(Color.green);
                        }
                    }
                }
                else
                {
                    //Unordered
                    for (int i = 0; i < antList.Count; i++)
                    {
                        if (antList[i]!=null)
                        {
                            antList[i].state = HexState.Termites;
                            antList[i].ChangeColor(Color.green);
                        }
                    }
                    //Unordered
                    for (int i = 0; i < Mathf.Abs(res); i++)
                    {
                         //Unordered
                        if (neutralList[i]!=null)
                        {
                            neutralList[i].state = HexState.Termites;
                            neutralList[i].ChangeColor(Color.green);
                        }
                    }
                }

                //Termites
                res = termitesList.Count - affectedTiles;
                if (res > 0)
                {
                    //Unordered
                    for (int i = 0; i < termitesList.Count; i++)
                    {
                        if (termitesList[i] != null)
                        {
                            termitesList[i].state = HexState.Ants;
                            termitesList[i].ChangeColor(Color.red);
                        }
                    }
                }
                else
                {
                    //Unordered
                    for (int i = 0; i < termitesList.Count; i++)
                    {
                        if (termitesList[i] != null)
                        {
                            termitesList[i].state = HexState.Ants;
                            termitesList[i].ChangeColor(Color.red);
                        }
                    }
                    //Unordered
                    for (int i = 0; i < Mathf.Abs(res); i++)
                    {
                        if (neutralList[i]!=null)
                        {
                            neutralList[i].state = HexState.Ants;
                            neutralList[i].ChangeColor(Color.red);
                        }
                        
                    }
                }

            }
        }
        else
        {
            Debug.Log("Lista vacia");
        }

    }

    public List<HexTile> RamdomList(List<HexTile> list)
    {
        System.Random rng = new System.Random();
        
        List<HexTile> suffled = list.OrderBy(h => rng.Next()).ToList();
        return suffled;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
