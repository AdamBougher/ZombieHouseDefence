using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.Tilemaps;

public class playerBuilding : MonoBehaviour {
    public Grid grid; // Reference to the Grid object
    public Sprite Tool;
    public RuleTile fenceTile;
    public GameObject turret;
    public AudioClip buildSound;
    
    
    public SpriteRenderer currentPlacement;
    private Camera _mainCamera;
    private AudioSource _audioSource;
    private NavMeshSurface _surface2D;
    
    public void SetArms() {
        GetComponent<SpriteRenderer>().sprite = Tool;
        currentPlacement.sprite = fenceTile.m_DefaultSprite;
    }

    private void Start() {
        _mainCamera = Camera.main;
        _audioSource = GetComponentInParent<AudioSource>();
        _surface2D = FindObjectOfType<NavMeshSurface>();
    }

    private void Update() {

        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the z position is 0

        // Snap the mouse position to the nearest grid point
        var cellPosition = grid.WorldToCell(mousePosition);
        var snappedPosition = grid.GetCellCenterWorld(cellPosition);

        currentPlacement.transform.position = snappedPosition;

        // Snap the rotation to the nearest 90 degrees
        var angle = Mathf.Round(currentPlacement.transform.rotation.eulerAngles.z / 90f) * 90f;
        currentPlacement.transform.rotation = Quaternion.Euler(0, 0, angle);
        
    }

    public void Place() {
        Debug.Log("Build!");
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the z position is 0

        // Snap the mouse position to the nearest grid point
        var cellPosition = grid.WorldToCell(mousePosition);
        var snappedPosition = grid.GetCellCenterWorld(cellPosition);

        // Create a new tile at the snapped position
        var tilemap = grid.GetComponentInChildren<Tilemap>();
        tilemap.SetTile(cellPosition, fenceTile);
        _audioSource.PlaySound(buildSound);
        _surface2D.UpdateNavMesh(_surface2D.navMeshData);
    }
}