using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIItemDisplay : MonoBehaviour
{
    
    [SerializeField] private List<UpgradeUiIcon> icons = new();
    
    
    private IEnumerator Start()
    {
        // Now all children have initialized
        icons = new List<UpgradeUiIcon>(GetComponentsInChildren<UpgradeUiIcon>());
        while (icons == null)
        {
            // Wait for one frame
            yield return null;
            // Now all children have initialized
            icons = new List<UpgradeUiIcon>(GetComponentsInChildren<UpgradeUiIcon>());
        }

    }
    
    public void AddToUI(Upgrade upgrade)
    {
        var icon = FindUpgradeUiIconByName(upgrade.Name);
        
        //initiaial search
        if (icon.upgradeName != "Empty")
        {
            icon.AddOne();
            return;
        }
        
        icon.SetItem(upgrade);
    }

    private UpgradeUiIcon FindUpgradeUiIconByName(string name)
    {
        return icons.Find(icon => icon.upgradeName == name) 
            ? icons.Find(icon => icon.upgradeName == name) 
            : icons.Find(icon => icon.upgradeName == "Empty");
    }
    
}