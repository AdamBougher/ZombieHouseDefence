using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeChoice : MonoBehaviour
{

    private Upgrade _upgrade;


    public void Setup(Upgrade item)
    {
        _upgrade = item;

        //setup options
        transform.Find("icon").GetComponent<Image>().sprite = _upgrade.Icon;
        GetComponentInChildren<TMP_Text>().SetText(_upgrade.Name);
        GetComponent<Button>().onClick.AddListener(ChooseOption);
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(ChooseOption);
    }

    private void ChooseOption()
    {
        FindAnyObjectByType<GameManager>().GetComponent<GameManager>().LevelUp(_upgrade);
    }
}
