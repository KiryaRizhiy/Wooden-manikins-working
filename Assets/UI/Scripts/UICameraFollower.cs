using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraFollower : MonoBehaviour
{

    void Start()
    {
        transform.rotation = Links.MainCamera.transform.rotation;
    }

    void Update()
    {
        transform.rotation = Links.MainCamera.transform.rotation;
    }
}