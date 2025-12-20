using UnityEngine;
using NavMeshPlus.Components;
using UnityEngine.Tilemaps;
using System.Collections;

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
    [SerializeField] private NavMeshSurface navMeshSurface;
    private Tilemap tilemap;
    private SpriteRenderer turretSpriteRenderer;
    private bool navmeshUpdateScheduled;

    private enum BuildItem { Fence, Turret }
    private BuildItem currentItem;

    [SerializeField] private PlayerArmsManager armsManager;

    private void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponentInParent<AudioSource>();
        tilemap = grid != null ? grid.GetComponentInChildren<Tilemap>() : null;
        if (turretPrefab != null)
            turretSpriteRenderer = turretPrefab.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (currentPlacement != null && currentPlacement.gameObject.activeSelf && grid != null && mainCamera != null)
        {
            UpdateGhostPlacement();
        }
    }

    public void SetArms()
    {
        armsManager?.SetArmsSprite(toolSprite);
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
        var turretSprite = turretSpriteRenderer != null ? turretSpriteRenderer.sprite : null;
        UpdateGhostSprite(currentItem == BuildItem.Fence ? fenceTile.m_DefaultSprite : turretSprite);
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
        ScheduleNavMeshUpdate();
    }

    private void BuildFence()
    {
        // Place a fence tile at the current grid position
        if (tilemap == null || fenceTile == null) return;
        tilemap.SetTile(GetGridPosition(), fenceTile);
    }

    private void BuildTurret()
    {
        // Instantiate a turret prefab at the current grid position
        if (turretPrefab == null || grid == null) return;
        Instantiate(turretPrefab, grid.GetCellCenterWorld(GetGridPosition()), Quaternion.identity);
    }

    private void PlayBuildSound()
    {
        if (audioSource != null && buildSound != null)
            audioSource.PlayOneShot(buildSound);
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface == null) return;
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }

    private void ScheduleNavMeshUpdate(float delay = 0.25f)
    {
        if (navmeshUpdateScheduled || navMeshSurface == null) return;
        navmeshUpdateScheduled = true;
        StartCoroutine(DebouncedNavmeshUpdate(delay));
    }

    private IEnumerator DebouncedNavmeshUpdate(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateNavMesh();
        navmeshUpdateScheduled = false;
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