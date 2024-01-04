using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDisplay : MonoBehaviour
{
    [ShowInInspector]
    public Queue<Image> images = new();


    private void Start()
    {
        foreach(Transform child in transform)
        {
            if(child.TryGetComponent<Image>(out Image outImg))
            {
                images.Enqueue(outImg);
            }
        }
    }


    public void AddToUI(Sprite icon)
    {
        Image item = images.Dequeue();

        item.sprite = icon;
        item.color = Color.white;
    }
}
