using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradeUiIcon : MonoBehaviour
{
    public string upgradeName = "Empty";
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amt;
    [SerializeField] private int amount;

    private void Start()
    {
        icon = GetComponent<Image>();
        amt = GetComponentInChildren<TMP_Text>();

    }
    
    public void SetItem(Upgrade item)
    {
        icon.sprite = item.Icon;
        icon.color = Color.white;

        upgradeName = item.Name;
    }
    
    public void AddOne()
    {
        amount++;
        amt.SetText(amount.ToString());
    }
}
