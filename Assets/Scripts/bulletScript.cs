using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public void RemoveSelf(float time)
    {
        Destroy(this.gameObject,time);
    }
}
