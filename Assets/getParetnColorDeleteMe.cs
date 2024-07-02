using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class getParetnColorDeleteMe : MonoBehaviour
{
    private MeshRenderer mRenderer;

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        mRenderer.material.color = transform.parent.GetComponent<MeshRenderer>().material.color;
    }
}
