using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDisplay : MonoBehaviour
{
    [ShowInInspector]
    public Queue<Image> Images = new();


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


    public void AddToUI(Sprite icon)
    {
        Image item = Images.Dequeue();

        item.sprite = icon;
        item.color = Color.white;
    }
}
