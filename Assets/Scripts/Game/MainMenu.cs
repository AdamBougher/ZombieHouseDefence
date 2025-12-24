using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Play()
    {

        StartCoroutine(SceneLoading());
        return;

        IEnumerator SceneLoading()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Field");
        
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            AsyncOperation asyncUnLoad = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            while (!asyncUnLoad.isDone)
            {
                yield return null;
            }
        }
    }
    
    public void Quit()
    {
        Application.Quit();
    }
    
}
