using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNoise : MonoBehaviour
{
    public Vector3 speed = Vector3.one;
    public Vector3 amount = Vector3.one;

    public float randomRange = 1;

    float randomX, randomY, randomZ;

    private void Start()
    {
        randomX = Random.Range(-randomRange, randomRange);
        randomY = Random.Range(-randomRange, randomRange);
        randomZ = Random.Range(-randomRange, randomRange);
    }

    public Vector3 GetMovementNoise()
    {
        float x = Mathf.Sin(Time.time * speed.x + randomX) * amount.x;
        float y = Mathf.Sin(Time.time * speed.y + randomY) * amount.y;
        float z = Mathf.Sin(Time.time * speed.z + randomZ) * amount.z;

        return new Vector3(x, y, z);
    }
}
