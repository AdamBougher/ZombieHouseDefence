using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDisplay : MonoBehaviour
{
    [ShowInInspector]
    public Queue<Image> Images = new();
    
    public Dictionary<string, Tuple<Image,TMP_Text>> Upgrades = new();

    private void Start()
    {
        foreach(Transform child in transform)
        {
            if(child.TryGetComponent<Image>(out Image outImg))
            {
                Images.Enqueue(outImg);
            }
        }
    }

    
    public void AddToUI(Upgrade upgrade, int amt)
    {
        //first check to see if item is displayed
        if (Upgrades.TryGetValue(upgrade.Name, out var tile))
        {
            tile.Item2.text = amt.ToString();
            return;
        }
        
        
        //if not then add it to the list
        var item = Images.Dequeue();

        item.sprite = upgrade.Icon;
        item.color = Color.white; 
        
        Upgrades.Add(upgrade.Name, new(item, item.GetComponentInChildren<TMP_Text>()));
    }
}
