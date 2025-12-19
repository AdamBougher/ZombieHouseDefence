using UnityEngine;

public class PlayerArmsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer armsSpriteRenderer;

    private void Awake()
    {
        if (armsSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer for arms is not assigned in PlayerArmsManager.");
        }
    }

    public void SetArmsSprite(Sprite sprite)
    {
        if (armsSpriteRenderer == null)
        {
            Debug.LogError("Cannot set arms sprite because SpriteRenderer is missing.");
            return;
        }

        armsSpriteRenderer.sprite = sprite;
    }
}
