using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Pawn
{
    [Header("Character")]

    [Header("Movement")]
    public float turnSpeed = 10f;

    [Header("Animation")]
    public Animator animator;
}
