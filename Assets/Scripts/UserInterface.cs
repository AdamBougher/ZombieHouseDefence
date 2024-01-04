using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    [BoxGroup("TextElements")]
    public TMP_Text ammo, clock, kills,level;
    [BoxGroup("ObjectElements")]
    public GameObject menu, levelUpMenu, itemUI;

    public Image xpBar;

    public UpgradeChoice[] levelUpOption;

    private readonly List<Image> uiItemicons = new();

    public UIItemDisplay imagePannel;

    private static UserInterface _instance;
    public static UserInterface UI
    {
        get {

            if (_instance == null)
            {
                _instance = FindObjectOfType<UserInterface>();
            }

            return _instance;
        }
    }



    private int index = 0;

    void Start()
    {

        clock.SetText("00:00");
        StartCoroutine(UpdateUI());

        foreach (Image img in itemUI.transform.GetComponentsInChildren<Image>())
        {
            uiItemicons.Add(img);
        }
    }

    public void UpdateAmmoDisplays(string str) 
    {
        ammo.SetText(str);
    }

    private void UpdateClock(string time)
    {
        clock.SetText(time);
    }

    private IEnumerator UpdateUI()
    {
        while (!GameManager.GameOver)
        {
            yield return new WaitForSeconds(0.5f);
            UpdateClock(GameManager.time.ToString());
        }

    }

    public void UpdateXpBar(float xpAmt)
    {
        xpBar.fillAmount = xpAmt;
    }

    public void DisplayKills()
    {
        kills.SetText(Enemy.EnemysKilled.ToString());
    }

    public void Updatelevel(string lvl)
    {
        level.SetText("Lvl: "+ lvl);
    }

    public void AddItemToUI(Upgrade option)
    {
        uiItemicons[index].sprite = option.Icon;
        index++;
        
    }

}
