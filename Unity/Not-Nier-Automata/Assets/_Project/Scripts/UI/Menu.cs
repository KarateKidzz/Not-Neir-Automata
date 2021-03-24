using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void LoadExplore()
    {
        GameManager.Instance.LevelLoader.LoadScene("ExploreDemo");
    }

    public void LoadCombat()
    {
        GameManager.Instance.LevelLoader.LoadScene("CombatDemo");
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }
}
