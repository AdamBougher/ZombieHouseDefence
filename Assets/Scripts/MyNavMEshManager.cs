using NavMeshPlus.Components;
using UnityEngine;

public class MyNavMEshManager : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface _surface2D;

    private void Start()
    {
        _surface2D = GetComponent<NavMeshSurface>();
    }
    
}
