using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.Tilemaps;

public class playerBuilding : MonoBehaviour {
    public Grid grid; // Reference to the Grid object
    public Sprite Tool;
    public AudioClip buildSound;
    
    public RuleTile fenceTile;
    public GameObject turret;
    
    
    public SpriteRenderer currentPlacement;
    private Camera _mainCamera;
    private AudioSource _audioSource;
    private NavMeshSurface _surface2D;
    private static int amtOfItems = 2;
    [SerializeField] private int currentItem;
    public void SetArms() {
        GetComponent<SpriteRenderer>().sprite = Tool;
        setGhost(fenceTile.m_DefaultSprite);
        
    }

    private void setGhost(Sprite sprite) {
        currentPlacement.sprite = sprite;
    }

    private void Start() {
        _mainCamera = Camera.main;
        _audioSource = GetComponentInParent<AudioSource>();
        _surface2D = FindObjectOfType<NavMeshSurface>();
    }

    private void Update() { ;

        currentPlacement.transform.position = grid.GetCellCenterWorld(GetGridPosition());

        // Snap the rotation to the nearest 90 degrees
        var angle = Mathf.Round(currentPlacement.transform.rotation.eulerAngles.z / 90f) * 90f;
        currentPlacement.transform.rotation = Quaternion.Euler(0, 0, angle);
        
    }

    public void ChangeItem(float direction) {
        currentItem += (int)direction;
        
        if (currentItem < 0) {
            currentItem = amtOfItems-1;
        }else if (currentItem == amtOfItems) {
            currentItem = 0;
        }

        switch (currentItem) {
            case 0:
                setGhost(fenceTile.m_DefaultSprite);
                break;
            case 1:
                setGhost(turret.GetComponent<SpriteRenderer>().sprite);
                break;
        }
    }
    public void Place() {
        switch (currentItem) {
            case 0:
                BuildFence();
                break;
            case 1: 
                BuildTurret();
                break;
        }
        _audioSource.PlaySound(buildSound);
        _surface2D.UpdateNavMesh(_surface2D.navMeshData);
    }

    private void BuildFence() {
        // Create a new tile at the snapped position
        grid.GetComponentInChildren<Tilemap>().SetTile(GetGridPosition(), fenceTile);
    }

    private void BuildTurret() {
        Instantiate(turret, grid.GetCellCenterWorld(GetGridPosition()), Quaternion.identity);
    }

    private Vector3Int GetGridPosition() {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the z position is 0

        // Snap the mouse position to the nearest grid point
        return grid.WorldToCell(mousePosition);
    }
}