using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightColourInCombat : Actor, IBeginPlay, IPhysicsTick
{
    public Light controlledLight;

    public WeaponUser weaponUser;

    public Color firingColor;

    Color startColor;

    public void BeginPlay()
    {
        startColor = controlledLight.color;   
    }

    // Fixed update so it's less often
    public void PhysicsTick(float DeltaTime)
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
