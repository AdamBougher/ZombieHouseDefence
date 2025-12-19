using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.Tilemaps;

public class PlayerBuilding : MonoBehaviour
{
    [Header("References")]
    public Grid grid; // Reference to the Grid object
    public Sprite toolSprite;
    public AudioClip buildSound;
    public RuleTile fenceTile;
    public GameObject turretPrefab;
    public SpriteRenderer currentPlacement;

    [Header("Settings")]
    [SerializeField] private int totalItems = 2; // Total number of buildable items
    [SerializeField] private int currentItemIndex;

    private Camera mainCamera;
    private AudioSource audioSource;
    private NavMeshSurface navMeshSurface;

    private enum BuildItem { Fence, Turret }
    private BuildItem currentItem;

    [SerializeField]
    private SpriteRenderer armsSpriteRenderer;

    private void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponentInParent<AudioSource>();
        navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
    }

    private void Update()
    {
        UpdateGhostPlacement();
    }

    public void SetArms()
    {
        if (armsSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on the PlayerBuilding GameObject.");
            return;
        }

        // Use FindFirstObjectByType instead of FindObjectOfType
        FindFirstObjectByType<PlayerArmsManager>()?.SetArmsSprite(toolSprite);
        UpdateGhostSprite(fenceTile.m_DefaultSprite);
    }

    private void UpdateGhostPlacement()
    {
        // Snap the ghost to the grid
        currentPlacement.transform.position = grid.GetCellCenterWorld(GetGridPosition());

        // Snap the rotation to the nearest 90 degrees
        var angle = Mathf.Round(currentPlacement.transform.rotation.eulerAngles.z / 90f) * 90f;
        currentPlacement.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void ChangeItem(float direction)
    {
        currentItem = (BuildItem)(((int)currentItem + (int)direction + totalItems) % totalItems);
        UpdateGhostSprite(currentItem == BuildItem.Fence ? fenceTile.m_DefaultSprite : turretPrefab.GetComponent<SpriteRenderer>().sprite);
    }

    public void Place()
    {
        switch (currentItem)
        {
            case BuildItem.Fence:
                BuildFence();
                break;
            case BuildItem.Turret:
                BuildTurret();
                break;
        }

        PlayBuildSound();
        UpdateNavMesh();
    }

    private void BuildFence()
    {
        // Place a fence tile at the current grid position
        var tilemap = grid.GetComponentInChildren<Tilemap>();
        tilemap.SetTile(GetGridPosition(), fenceTile);
    }

    private void BuildTurret()
    {
        // Instantiate a turret prefab at the current grid position
        Instantiate(turretPrefab, grid.GetCellCenterWorld(GetGridPosition()), Quaternion.identity);
    }

    private void PlayBuildSound()
    {
        if (audioSource != null && buildSound != null)
            audioSource.PlayOneShot(buildSound);
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface != null)
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }

    private void UpdateGhostSprite(Sprite sprite)
    {
        if (currentPlacement != null)
            currentPlacement.sprite = sprite;
    }

    private Vector3Int GetGridPosition()
    {
        // Convert the mouse position to a grid cell position
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the z position is 0
        return grid.WorldToCell(mousePosition);
    }
}