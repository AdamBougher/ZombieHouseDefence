using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [BoxGroup("TextElements")]
    public TMP_Text ammo, clock, kills,level, hp, jumbotron;
    [BoxGroup("ObjectElements")]
    public GameObject menu, levelUpMenu, itemUI;
    public Image xpBar;
    public UpgradeChoice[] levelUpOption;
    private readonly List<Image> _uiItemImages = new();
    public UIItemDisplay imagePanel;
    public static UserInterface UI { get; private set; }
    public delegate void UILoaded();
    public static event UILoaded OnLoaded;
    private int _index;

    private const int JumbotronOnScreenTime = 3;
    private float _jumbotronFadeAmt = 0.1f;

    private void Awake()
    {
        UI = this;
        
        clock.SetText("00:00");
        StartCoroutine(UpdateUI());

        foreach (var img in itemUI.transform.GetComponentsInChildren<Image>())
        {
            _uiItemImages.Add(img);
        }

        OnLoaded?.Invoke();

        StartCoroutine(Tutorial());
    }

    private IEnumerator Tutorial()
    {
        Jumbotron("Use W,A,S and D to move around");
        yield return new WaitForSeconds(6.5f);
        Jumbotron("Aim with the Mouse!");
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
            UpdateClock(GameManager.Time.ToString());
        }

    }

    public void UpdateXpBar(float xpAmt)
    {
        xpBar.fillAmount = xpAmt;
    }

    public void DisplayKills()
    {
        kills.SetText(Enemy.EnemiesKilled.ToString());
    }

    public void Updatelevel(string lvl)
    {
        level.SetText("Lvl: "+ lvl);
    }

    public void UpdateHp(int amt)
    {
        hp.SetText(amt.ToString());
    }
    
    public void AddItemToUI(Upgrade option)
    {
        _uiItemImages[_index].sprite = option.Icon;
        _index++;
        
    }

    public void ReturnToMenu()
    {
        StartCoroutine(SceneLoading());
        return;

        IEnumerator SceneLoading()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            AsyncOperation asyncUnLoad = SceneManager.UnloadSceneAsync("MainMenu");

            while (!asyncUnLoad.isDone)
            {
                yield return null;
            }
        }
    }

    public void Jumbotron(string text)
    {
        var color = jumbotron.color;
        
        jumbotron.SetText(text);
        
        StartCoroutine(FadeIn());
        return;
        
        IEnumerator FadeIn()
        {
            while (jumbotron.alpha < 1)
            {
                yield return new WaitForSeconds(0.1f);
                jumbotron.color = new (color.r,color.g,color.b, color.a += _jumbotronFadeAmt);
            }

            yield return new WaitForSeconds(JumbotronOnScreenTime);

            while (jumbotron.alpha > 0)
            {
                yield return new WaitForSeconds(0.1f);
                jumbotron.color = new (color.r,color.g,color.b, color.a -= _jumbotronFadeAmt);
            }
        }
    }
    
}
