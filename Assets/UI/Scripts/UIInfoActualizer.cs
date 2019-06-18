using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UIInfoActualizer : MonoBehaviour {

    UIInfoActualizerState State = UIInfoActualizerState.New;
    
    ObjectMethod _m;

	
	// Update is called once per frame
	void Update () {
        if (State == UIInfoActualizerState.Active)
        {
            _m.Invoke();            
        }
	}

    public void UnPause()
    {
        if (State == UIInfoActualizerState.Paused)
            State = UIInfoActualizerState.Active;
    }
    public void Pause()
    {
        if (State == UIInfoActualizerState.Active)
            State = UIInfoActualizerState.Paused;
    }
    public void Initialize(ObjectMethod Method,bool StartFromPause = false)
    {
        _m = Method;
        if (StartFromPause)
            State = UIInfoActualizerState.Paused;
        else
            State = UIInfoActualizerState.Active;
    }
}
enum UIInfoActualizerState {New,Paused,Active };
public delegate void ObjectMethod();