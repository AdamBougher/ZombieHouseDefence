using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class Door : MonoBehaviour, IHittable
{
    private NavMeshSurface _surface2D;
    
    [SerializeField] 
    private int hp = 10;

    
    [SerializeField]
    private bool isOpen;

    private void OnEnable()
    {
        _surface2D = FindObjectOfType<NavMeshSurface>();
    }

    public void Enter()
    {
        if (isOpen)
        {
            isOpen = false;
            gameObject.transform.Rotate(0,0,90);
        }else{
            
            isOpen = true;
            gameObject.transform.Rotate(0,0,-90);
        }
        
        _surface2D.UpdateNavMesh(_surface2D.navMeshData);

    }

    public void Damage(int amt)
    {
        hp -= amt;
        
        if (hp > 0) 
            return;
        
        GetComponent<BoxCollider2D>().enabled = false;
        foreach (var boxCollider2D in gameObject.GetComponents<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
        
        _surface2D.UpdateNavMesh(_surface2D.navMeshData);


        Destroy(gameObject);
        
    }

    public void Fix(int amt)
    {
        hp += amt;
    }
}
