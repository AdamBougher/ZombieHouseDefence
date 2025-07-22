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

    public void SetToolSprite()
    {
        GetComponent<SpriteRenderer>().sprite = toolSprite;
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
        currentItemIndex += (int)direction;

        // Wrap around the item index
        if (currentItemIndex < 0)
            currentItemIndex = totalItems - 1;
        else if (currentItemIndex >= totalItems)
            currentItemIndex = 0;

        // Update the ghost sprite based on the selected item
        switch (currentItemIndex)
        {
            case 0:
                UpdateGhostSprite(fenceTile.m_DefaultSprite);
                break;
            case 1:
                UpdateGhostSprite(turretPrefab.GetComponent<SpriteRenderer>().sprite);
                break;
        }
    }

    public void Place()
    {
        switch (currentItemIndex)
        {
            case 0:
                BuildFence();
                break;
            case 1:
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