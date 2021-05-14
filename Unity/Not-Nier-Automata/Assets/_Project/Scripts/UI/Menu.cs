using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Menu : MonoBehaviour
{
    [EventRef]
    public string clickSound;

    [EventRef]
    public string hoverSound;

    public void LoadLevelOne()
    {
        GameManager.Instance.LevelLoader.LoadScene("Level01");
    }

    public void LoadExplore()
    {
        GameManager.Instance.LevelLoader.LoadScene("ExploreDemo");
    }

    public void LoadCombat()
    {
        GameManager.Instance.LevelLoader.LoadScene("CombatDemo");
    }

    public void LoadCollision()
    {
        GameManager.Instance.LevelLoader.LoadScene("ProgrammerSound");
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }

    public void PlayClickSound()
    {
        if (!string.IsNullOrEmpty(clickSound))
        {
            RuntimeManager.PlayOneShot(clickSound);
        }
    }

    public void PlayHoverSound()
    {
        if (!string.IsNullOrEmpty(hoverSound))
        {
            RuntimeManager.PlayOneShot(hoverSound);
        }
    }
}
