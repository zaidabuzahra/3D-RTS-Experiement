using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public virtual void Selected()
    {
        Debug.Log("hello");
        TellUI();
    }

    public virtual void TellUI()
    {

    }
}