using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class Door : MonoBehaviour, IHittable
{
    [SerializeField]
    private NavMeshSurface _surface2D;
    
    [SerializeField] 
    private int hp = 10;

    [SerializeField]
    private bool isOpen;

    private bool navmeshUpdateScheduled;
    private const float NavMeshDebounceDelay = 0.2f;

    private void OnEnable()
    {
        // NavMeshSurface should be assigned in Inspector
        if (_surface2D == null)
        {
            _surface2D = FindFirstObjectByType<NavMeshSurface>();
            if (_surface2D == null)
                Debug.LogError("Door: NavMeshSurface not found or assigned.");
        }
    }

    private void OnDisable()
    {
        // Clean up on destruction
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
        
        ScheduleNavMeshUpdate();
    }

    private void ScheduleNavMeshUpdate()
    {
        if (navmeshUpdateScheduled || _surface2D == null) return;
        navmeshUpdateScheduled = true;
        StartCoroutine(DebouncedNavMeshUpdate());
    }

    private IEnumerator DebouncedNavMeshUpdate()
    {
        yield return new WaitForSeconds(NavMeshDebounceDelay);
        if (_surface2D != null)
            _surface2D.UpdateNavMesh(_surface2D.navMeshData);
        navmeshUpdateScheduled = false;
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
        
        ScheduleNavMeshUpdate();
        Destroy(gameObject);
    }

    public void Fix(int amt)
    {
        hp += amt;
    }
}
