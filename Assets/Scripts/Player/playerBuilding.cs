using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class playerBuilding : MonoBehaviour {
    public bool buildModeEnabled = false;
    public Grid grid; // Reference to the Grid object
    public Sprite Tool;
    public RuleTile fenceTile;
    public GameObject turret;
    
    public int currentItem = 0;

    public SpriteRenderer currentPlacement;
    private Camera _mainCamera;
    public void SetArms() {
        GetComponent<SpriteRenderer>().sprite = Tool;
        currentPlacement.sprite = fenceTile.m_DefaultSprite;
    }

    private void Start() {
        _mainCamera = Camera.main;
    }

    private void Update() {
        if (!buildModeEnabled) {
            if (currentPlacement != null && currentPlacement.gameObject.activeSelf) {
                currentPlacement.gameObject.SetActive(false);
            }
            return;
        }

        if (currentPlacement != null && !currentPlacement.gameObject.activeSelf) {
            currentPlacement.gameObject.SetActive(true);
        }

        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the z position is 0

        // Snap the mouse position to the nearest grid point
        Vector3Int cellPosition = grid.WorldToCell(mousePosition);
        Vector3 snappedPosition = grid.GetCellCenterWorld(cellPosition);

        currentPlacement.transform.position = snappedPosition;

        // Snap the rotation to the nearest 90 degrees
        float angle = Mathf.Round(currentPlacement.transform.rotation.eulerAngles.z / 90f) * 90f;
        currentPlacement.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void OnFire(InputAction.CallbackContext context){
        if (!buildModeEnabled) return;
        
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the z position is 0

        // Snap the mouse position to the nearest grid point
        var cellPosition = grid.WorldToCell(mousePosition);
        var snappedPosition = grid.GetCellCenterWorld(cellPosition);

        // Create a new tile at the snapped position
        var tilemap = grid.GetComponentInChildren<Tilemap>();
        tilemap.SetTile(cellPosition, fenceTile);
    }
    
    private void OnSwapBuild(InputValue value) {
        Debug.Log(value.Get<float>() + "Was pressed");
    }
    
}
