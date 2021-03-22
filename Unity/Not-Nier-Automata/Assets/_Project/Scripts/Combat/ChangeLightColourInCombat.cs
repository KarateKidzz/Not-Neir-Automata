using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightColourInCombat : MonoBehaviour
{
    public Light controlledLight;

    public WeaponUser weaponUser;

    public Color firingColor;

    Color startColor;

    private void Start()
    {
        startColor = controlledLight.color;   
    }


    // Fixed update so it's less often
    private void FixedUpdate()
    {
        if (weaponUser)
        {
            if(weaponUser.IsFiring)
            {
                controlledLight.color = firingColor;
            }
            else
            {
                controlledLight.color = startColor;
            }
        }
    }
}
