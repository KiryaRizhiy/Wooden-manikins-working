using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Links : MonoBehaviour {

	// 1,2,3,4
    public Camera _Camera;
    public Canvas _Canvas;

    //public static world_building WorldBuilder;
    //public static Resources Resources;
    //public static Brick_intreraction_script Bricks;
    public static ProcessManager Processes;
    public static Building_Creator_Interface Builder;
    public static LogSettings LSettings;
    public static UI Interface;
    public static Canvas MainCanvas;
    public static Camera MainCamera;
    public static Player_control_script PlayerControl;

    private string scr = "Links";

	void Start () {
        SetLinks();
	}
    private void SetLinks()
    {
        //WorldBuilder = GetComponent<world_building>();
        //Resources = transform.GetChild(2).gameObject.GetComponent<Resources>();
        //Bricks = transform.GetChild(1).GetComponent<Brick_intreraction_script>();
        Processes = transform.GetChild(5).GetComponent<ProcessManager>();
        Builder = transform.GetChild(3).GetComponent<Building_Creator_Interface>();
        LSettings = GetComponent<LogSettings>();
        Interface = GetComponent<UI>();
        PlayerControl = GetComponent<Player_control_script>();
        MainCamera = _Camera;
        MainCanvas = _Canvas;
        

        Log.LinkFields();
        Log.Notice(scr, "Links set, settings initializer called");
        GetComponent<Settings>().SetParams();
        
        Builder.SetParams();
    }
}