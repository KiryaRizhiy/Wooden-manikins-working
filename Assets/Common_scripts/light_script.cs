using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light_script : MonoBehaviour
{
    
    private string scr = "LightScript";
    public float turnspeed = 0.3f;
    private Transform Obj_pos;

    // Use this for initialization
    void Start()
    {
        Obj_pos = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Obj_pos.Translate(Vector3.right * turnspeed * Time.deltaTime);
        Obj_pos.LookAt(new Vector3(0f,0f,0f));
    }
}

