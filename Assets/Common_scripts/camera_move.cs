using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_move : MonoBehaviour {

    private string scr = "CameraMoveScript";
    private Transform cam_position;
    public bool DefaultSettings,_CameraCanMove,DebugMode;
    public float Transform_speed,ScrollSpeed,RotateSpeed,AngleLimit,RollbackSpeed;
    private float UpDeltaPos,RightDeltaPos;
    private Vector3 PreviousMousePosition,PreviousCameraPosition;
    private static float MaxTopShift = 10f;
    private bool isTriggered;

    //Рекомендованные настройки:
    //Transform_speed=0.02;
    //Scroll speed = 15;
    //Rotate Speed = 0.3;
    //Angle limit = 75;
    //RollbackSpeed = 0.5f;
	// Use this for initialization
	void Start () {
        cam_position = GetComponent<Transform>();
        transform.position = Vector3.up * (Settings.MapHeight - 1);
        Log.Notice(scr,"Started at:" + Time.time);
        if (DefaultSettings)
        {
            Transform_speed = 0.02f;
            ScrollSpeed = 15;
            RotateSpeed = 0.3f;
            AngleLimit = 75;
            RollbackSpeed = 0.5f;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (_CameraCanMove)
        {
            if (PreviousCameraPosition != transform.position)
                PreviousCameraPosition = transform.position;
            if (Input.GetMouseButtonDown(2))
            {
                PreviousMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                UpDeltaPos = PreviousMousePosition.y - Input.mousePosition.y;
                cam_position.Translate(Vector3.up * UpDeltaPos * Transform_speed);
                RightDeltaPos = PreviousMousePosition.x - Input.mousePosition.x;
                cam_position.Translate(Vector3.right * RightDeltaPos * Transform_speed);
                PreviousMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonDown(1))
            {
                PreviousMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(1))
            {
                UpDeltaPos = -(PreviousMousePosition.x - Input.mousePosition.x);
                RightDeltaPos = (PreviousMousePosition.y - Input.mousePosition.y);
                cam_position.rotation = Quaternion.Euler(new Vector3(cam_position.rotation.eulerAngles.x, cam_position.rotation.eulerAngles.y + UpDeltaPos * RotateSpeed, cam_position.rotation.eulerAngles.z));
                if ((cam_position.rotation.eulerAngles.x + RightDeltaPos * RotateSpeed < AngleLimit) || (360 - cam_position.rotation.eulerAngles.x - RightDeltaPos * RotateSpeed < AngleLimit))
                    cam_position.rotation = Quaternion.Euler(new Vector3(cam_position.rotation.eulerAngles.x + RightDeltaPos * RotateSpeed, cam_position.rotation.eulerAngles.y, cam_position.rotation.eulerAngles.z));
                PreviousMousePosition = Input.mousePosition;
            }
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                cam_position.Translate(Vector3.forward * ScrollSpeed * Input.mouseScrollDelta.y * Time.deltaTime);
            }
            if (!CamIntoBoarders())
            {
                Links.Interface.ShowExpandButton(transform.position);
                transform.position = PreviousCameraPosition;
            }
        }
	}
    void OnTriggerEnter(Collider C)
    {
        if (!DebugMode)
        {
            //_CameraCanMove = false;
            //isTriggered = true;
            //StartCoroutine(CamRollBack(PreviousCameraPosition));
            transform.position = PreviousCameraPosition;
        }
    }
    //void OnTriggerExit(Collider C)
    //{
    //    isTriggered = false;
    //}
    //IEnumerator CamRollBack(Vector3 TargetPosition)
    //{
    //    Vector3 InitialNormal = TargetPosition-transform.position;
    //    Vector3.Normalize(InitialNormal);
    //    Log.Notice(scr, "Start rolling back. Current position: " + transform.position + ", target: " + TargetPosition + " Normal: " + InitialNormal);
    //    while (isTriggered)
    //    {
    //        transform.Translate(InitialNormal * -1 * RollbackSpeed * Time.deltaTime);
    //        yield return new WaitForEndOfFrame();
    //    }
    //    Log.Notice(scr, "Rolled back. Current position: " + transform.position);
    //    _CameraCanMove = true;
    //}
    private bool CamIntoBoarders()
    {
        if (DebugMode)
            return true;
        if (transform.position.y > Settings.MapHeight + MaxTopShift)
            return false;
        if (!Map.CoordinatesIntoTheMap(transform.position))
            return false;
        return true;
    }
}