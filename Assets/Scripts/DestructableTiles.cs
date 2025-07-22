using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

/// <summary>
/// this class is used to create destructable tile, it has a HP value and a cost value
/// it should keep track of a certan tile on a tilemap and change the tile when the HP reaches certain threshholds
/// it should also have a sound that plays when it takes damage
/// it should also have a sound that plays when it is destroyed
/// </summary>
public class DestructableTiles : MonoBehaviour
{
    public float HP;

    public int cost;
    
    public TileBase tile;

    public Color[] color;

    public AudioClip[] damaged;

    
    /// <summary>
    /// in the start method it should place the tile on the tilemap
    /// it should also set the color of the tile based on the HP value
    /// </summary>
    private void Start() {
        
    }

    /// <summary>
    /// this damages the hp of the tile
    /// </summary>
    /// <param name="dmg">amount to remove from hp</param>
    public void Damage(int dmg){
        HP -= dmg;
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().clip = damaged[Random.Range(0,damaged.Length)];

        //ColorCheck();

        if(HP <= 0){
            Destroy(this.gameObject);
        }
    }
    
    /// <summary>
    /// this method changes the color of the tile based on the HP
    /// </summary>
    private void ColorCheck(){
        if(HP <= HP*0.5){
            GetComponent<SpriteRenderer>().color = color[0];
        }

        if(HP <= HP*0.25){
            GetComponent<SpriteRenderer>().color = color[1];
        }
    }

    
    
}
