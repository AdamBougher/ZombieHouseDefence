using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class UpgradeChoice : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text textComponent;
    public Button buttonComponent;
    private static GameManager  Gm => FindObjectOfType<GameManager>();
    
    [RuntimeInitializeOnLoadMethod]
    private void Awake()
    {
        iconImage = transform.Find("icon").GetComponent<Image>();
        textComponent = GetComponentInChildren<TMP_Text>();
        buttonComponent = GetComponent<Button>();
    }

    private IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(0.02f);
        buttonComponent.interactable = true;
        
    }

    private void OnEnable()
    {
        StartCoroutine(EnableButtons());
    }

    private void OnDisable()
    {
        buttonComponent.interactable = false;
    }

    public void SetUpgrade(Upgrade upgrade, Player player)
    {
        iconImage.sprite = upgrade.Icon;
        textComponent.text = upgrade.Name;
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(() => upgrade.ApplyUpgrade(player));
        buttonComponent.onClick.AddListener(() => GameManager.UserInterface.imagePanel.AddToUI(upgrade));
        
        buttonComponent.onClick.AddListener(() => GameManager.EndLevelUp());
    }
    
}