using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Animator animator;


    private void Start()
    {
        
    }

    private void OnMouseDown()
    {
        animator.SetTrigger("Presser");
        Debug.Log("je po me");
    }
}


    
