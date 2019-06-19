using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Xml;

public class Player_control_script : MonoBehaviour{

    public Camera _Camera;
    public GameObject _Canvas,BrickPrefab;
    

    //public static Brick_intreraction_script Bricks { get; private set; }
    //public static Resources Resources { get; private set; }

    private Dropdown _BuildingSelectionDD { get { return Links.Interface.StructurePlacingPanelBuildingSelect; } }
    private Ray _CameraRay;
    private RaycastHit _Hit;
    public bool _IsHit = false, _PopupMode = false,_BuildingMode = false;
    private GameObject _MetaBuilding, _CurrentUnit, _TargetUnit, _ActorUnit, _CurrentObject, _ZoneStartPoint, _ZoneEndPoint,ZonePointer;
    private GameObject PickedObject
    { 
        get
        {
            return _PickedObject;
        }
        set
        {
            if (value != null)
            {
                value.BroadcastMessage("Pick");
            }
            if (_PickedObject != null)
            {
                _PickedObject.BroadcastMessage("UnPick");
            }
            _PickedObject = value;

        }
    }
    private GameObject _PickedObject;
    private Vector3 _Normal;
    private ushort _ActionID = 0;
    private int mask = 1 << 10;
    private PhantomConstruction _PhantomBuilding;
    private byte _ZoneType = 1;
    private byte _StorageType;
    private string scr = "PlayerControlScript", scrm = "MouseClicks";

	void Start () {
        _MetaBuilding = transform.GetChild(3).gameObject;
        _CurrentObject = null;
        //PickedObject = null;
        _CurrentUnit = null;
        _TargetUnit = null;
        //SwitchToMainMenuMode();
        //UpdateBuildingsList();
        mask = ~mask;
	}

	void Update () {
        if (!_PopupMode)
        {
            if (_CurrentObject != null)
            {
                Highlighter.UnHighLight(_CurrentObject);
                _CurrentObject = null;
            }
            if (_CurrentUnit != null)
            {
                Highlighter.UnHighLight(_CurrentUnit);
                _CurrentUnit = null;
            }
            if (Input.GetKeyDown("z"))
                Settings.ZoneRod.SetActive(true);
            if (Input.GetKeyUp("z") && _ZoneStartPoint == null)
                Settings.ZoneRod.SetActive(false);
            if (!EventSystem.current.IsPointerOverGameObject())//Изучить. Непонятно что это, но работает
            {
                _CameraRay = _Camera.ScreenPointToRay(Input.mousePosition); //создаем луч, идущий из камеры через координаты мышки
                _IsHit = Physics.Raycast(_CameraRay, out _Hit, 1000,mask);
                if (_IsHit)
                {
                    if (_Hit.transform.tag == "Brick")
                    {
                        if ((_BuildingMode) && (_Hit.transform.gameObject != _CurrentObject))
                        {
                            _PhantomBuilding.Hide();
                            _PhantomBuilding.Show(_Hit.transform.position);
                        }
                        _CurrentObject = _Hit.transform.gameObject;
                        Highlighter.HighLight(_CurrentObject);
                        if (Settings.ZoneRod.activeInHierarchy)
                            Settings.ZoneRod.transform.SetParent(_CurrentObject.transform,false);
                    }
                    if (_Hit.transform.tag == "Unit")
                    {
                        _CurrentUnit = _Hit.transform.gameObject;
                        Highlighter.HighLight(_CurrentUnit);
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (PickedObject != null)
                        Highlighter.UnPick(PickedObject);
                    if (_IsHit)
                    {
                        if (_Hit.transform.tag == "Brick" && _BuildingMode)
                        {
                            new ProcessManager.Building(_PhantomBuilding);
                            //Building _b = new Building(_BuildingSelectionDD.value, _PhantomBuilding.Rotation, _PhantomBuilding.MasterObject.transform.position);
                            //_PhantomBuilding.Destroy();
                            //Brick_intreraction_script.AddBuilding(_b);
                            _PhantomBuilding = null;
                            _BuildingMode = false;
                        }
                        else
                        {
                            if (Settings.ZoneRod.activeInHierarchy)//В режиме выбора зоны
                            {
                                if (_ZoneStartPoint == null)//Стартовая точка не задана
                                {
                                    _ZoneStartPoint = Instantiate(Settings.ZoneRod);
                                    _ZoneStartPoint.transform.SetParent(_CurrentObject.transform,false);
                                }
                                else //Стартовая точка задана
                                {
                                    //_ZoneEndPoint = Settings.ZoneRod; Пока написана херня для теста
                                    _ZoneEndPoint = Instantiate(Settings.ZoneRod);
                                    _ZoneEndPoint.transform.SetParent(_CurrentObject.transform,false);
                                    Settings.ZoneRod.SetActive(false);
                                    Links.Interface.SwitchToZoneMenu(0);
                                }
                            }
                            PickedObject = _Hit.transform.gameObject;
                            PickedObject.BroadcastMessage("Click");
                            Highlighter.Pick(PickedObject);
                            _Normal = _Hit.normal;
                            Log.Notice(scr + scrm, "Picked Object : " + PickedObject.name);
                        }
                    }
                    else
                    {
                        PickedObject = null;
                        Settings.ZoneRod.SetActive(false);
                        Destroy(_ZoneStartPoint);
                    }
                }
            }
            if (Input.GetKeyDown("a"))
            {
                if (PickedObject != null && PickedObject.tag == "Unit")
                {
                    if (_ActorUnit != null)
                        Highlighter.UnPick(_ActorUnit);
                    _ActorUnit = PickedObject;
                    PickedObject = null;
                    Highlighter.BrightPick(_ActorUnit);
                }
                else
                {
                    if (_ActorUnit != null)
                        Highlighter.UnPick(_ActorUnit);
                    _ActorUnit = null;
                }
            }
            if (Input.GetKeyDown("r"))
                if (_PhantomBuilding != null)
                    _PhantomBuilding.Rotate();
            if (Input.GetMouseButton(1) && _ActorUnit != null)
            {
                Log.Notice(scr,"Walking");
                if ((_IsHit) && (_Hit.normal == Vector3.up))
                {
                    //if (Mathf.Round(_Hit.point.x) < 0)
                        _Normal.x = Mathf.RoundToInt(_Hit.point.x);
                    //else
                    //    _Normal.x = Mathf.Ceil(_Hit.point.x);
                    //if (Mathf.Round(_Hit.point.z) < 0)
                        _Normal.z = Mathf.RoundToInt(_Hit.point.z);
                    //else
                    //    _Normal.z = Mathf.Ceil(_Hit.point.z);
                    _Normal.y = _Hit.point.y;
                    Log.Notice(scr,"Waybuilder call");
                    _ActorUnit.GetComponent<RouteBuilder>().Build(_Normal);
                }
            }
            if (Input.GetMouseButton(1))//Massive reject
            {
                _BuildingMode = false;
                if (_PhantomBuilding != null)
                {
                    _PhantomBuilding.Destroy();
                    _PhantomBuilding = null;
                }
                CancelZoneCreation();
            }
            if (PickedObject != null && PickedObject.tag == "Unit" && Input.GetKey("l"))
            {
                PickedObject.GetComponent<RouteBuilder>().LogRoute();
            }
            if (Input.GetKey("o"))
            {
                if (!(PickedObject == null || PickedObject.tag == "Unit"))
                {
                    CreateUnit(PickedObject.transform.position);
                    Highlighter.UnPick(PickedObject);
                    PickedObject = null;
                }
                else
                    Log.Notice(scr,"There is no picked cube");
            }
        }
        else // Popup is open
        {
        }
    }
    private void SwitchToZoneCreateMode()
    {
        _PopupMode = true;
        for (int i = 0; i < _Canvas.transform.childCount; i++)
        {
            if (_Canvas.transform.GetChild(i).tag != "ZoneMenu")
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (_Canvas.transform.GetChild(i).tag == "ZoneMenu")
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    public void SwtichToMenuMode(string _m)
    {
        _PopupMode = true;
        for (int i = 0; i < _Canvas.transform.childCount; i++)
        {
            if (_Canvas.transform.GetChild(i).tag != _m)
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (_Canvas.transform.GetChild(i).tag == _m)
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        switch (_m)
        {
            case "ProcessCreateMenu":
                break;
            default:
                Debug.LogError("Unknown menu called. It will be ignored");
                SwitchToMainMenuMode();
                break;
        }
    }
    public void SwitchToMainMenuMode()
    {
        UpdateBuildingsList();
        _PopupMode = false;
        for (int i = 0; i < _Canvas.transform.childCount; i++)
        {
            if (_Canvas.transform.GetChild(i).tag == "MainMenu")
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(true);
            }
            if (_Canvas.transform.GetChild(i).tag != "MainMenu")
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        _Camera.GetComponent<camera_move>()._CameraCanMove = true;
    }
    public void CreateUnit(Vector3 _Coordinates)
    {
        if (_CurrentUnit != null)
            Highlighter.UnHighLight(_CurrentUnit);
        _Coordinates.y += 1.5f;
        _CurrentUnit = Instantiate(Settings.UnitPrefab, _Coordinates, Quaternion.identity);
        _CurrentUnit.transform.SetParent(transform.GetChild(0));
    }
    public void DoTheAction()
    {
        if (_ActionID != 0)
        {
            //_ActorUnit.GetComponent<Unit>().DoTheAction(_ActionID,_PickedObject);
            _ActionID = 0;
        }
        else
            Log.Notice(scr,"Please, select the action");
    }
    public void DestroyButtonClick()
    {
        if (PickedObject != null)
        {
            Log.Notice(scr,"Destroy button clicked");
            Highlighter.UnPick(PickedObject);
            Map.RemoveBrick(PickedObject.transform.position);
            PickedObject = null;
        }
    }
    public void CreateButtonClick()
    {
        if (PickedObject != null)
        {
            Log.Notice(scr,"Create button clicked");
            //ushort r;
            //r = (ushort)Random.Range(0, Settings.ResorucesForWorldBuilding.Count-1);
            //Log.Notice(scr,"Creating brick with parameters: [" + PickedObject.transform.position + "]. Material: " + r);
            Highlighter.UnPick(PickedObject);
            //_Landskape.GetComponent<Brick_intreraction_script>().AddBrick(Mathf.RoundToInt((_PickedObject.transform.position + _Normal).x + Settings.WorldLength + 0.5f), Mathf.RoundToInt((_PickedObject.transform.position + _Normal).y + Settings.WorldHeight + 0.5f), Mathf.RoundToInt((_PickedObject.transform.position + _Normal).z + Settings.WorldWidth + 0.5f), Random.Range(0, 2));
            Map.AddBrick(PickedObject.transform.position + _Normal, Resources.ResourceLibrary.FindAll(x => x.IsResourceForWorldBuilding)[Random.Range(0, Resources.ResourceLibrary.FindAll(x => x.IsResourceForWorldBuilding).Count)]);
            PickedObject = null;
        }
    }
    public void ActionIDEnter(string _s)
    {
        _ActionID = ushort.Parse(_s);
    }

    //Zones
    public void ZoneTypeSelect(Dropdown _d)
    {
        _ZoneType = (byte)(_d.value + 1);
        Log.Notice(scr,"Zone type selected" + (_d.value + 1));
    }
    public void StorageTypeSelect(Dropdown _d)
    {
        _StorageType = (byte)_d.value;
    }
    public void CreateZone()
    {
        switch (Links.Interface.ZoneTypeSelect.value)
        {
            case 0:
                ZoneController.CreateZone(_ZoneStartPoint.transform.position, _ZoneEndPoint.transform.position, 1, _StorageType);
                break;
            case 1:
                ZoneController.CreateZone(_ZoneStartPoint.transform.position, _ZoneEndPoint.transform.position, 4, _StorageType);
                break;
            case 2:
                ZoneController.CreateZone(_ZoneStartPoint.transform.position, _ZoneEndPoint.transform.position, 5, _StorageType);
                break;
        }
        CancelZoneCreation();
    }
    public void CancelZoneCreation()
    {
        if (_ZoneEndPoint != null)
        {
            Destroy(_ZoneEndPoint);
            Links.Interface.SwitchToMainMenu();
        }
        if (_ZoneStartPoint != null)
            Destroy(_ZoneStartPoint);
        if (PickedObject != null)
        {
            Highlighter.UnPick(PickedObject);
            PickedObject = null;
        }
        Settings.ZoneRod.SetActive(false);
    }
    // Buildings
    public void SwitchToBuildingCreationMode()
    {
        _PopupMode = true;
        for (int i = 0; i < _Canvas.transform.childCount; i++)
        {
            if (_Canvas.transform.GetChild(i).tag != "BuildingsMenu")
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (_Canvas.transform.GetChild(i).tag == "BuildingsMenu")
            {
                _Canvas.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        _Camera.GetComponent<camera_move>()._CameraCanMove = false;
        _MetaBuilding.GetComponent<Building_Creator_Interface>().BuildingModeOn(_Camera.transform.position + 6 * Vector3.Normalize(_Camera.transform.forward));
    }
    public void UpdateBuildingsList()
    {
        _BuildingSelectionDD.options.Clear();
        XmlDocument _BuildingsRepository = new XmlDocument();
        _BuildingsRepository.Load("C:/GD/You and world/v.0.0.1/First/Configs/Buildings.xml");
        foreach (XmlNode _xnode in _BuildingsRepository.DocumentElement.ChildNodes)
        {
            _BuildingSelectionDD.options.Add(new Dropdown.OptionData(_xnode.Attributes.Item(0).InnerText));
            _BuildingSelectionDD.value = 1;
            _BuildingSelectionDD.value = 0;
        }
    }
    public void ConstructPhantomBuilding()
    {
        //_PhantomBuildingCoordinates.Clear();
        if (_PhantomBuilding != null)
        {
            _PhantomBuilding.Destroy();
        }
        _BuildingMode = true;
        XmlDocument _BuildingsRepository = new XmlDocument();
        _BuildingsRepository.Load(Settings.BuildingsStoragePath);
        //_PhantomBuilding = new PhantomBuilding(_BuildingsRepository.DocumentElement.ChildNodes[_BuildingSelectionDD.value]);
        _PhantomBuilding = new PhantomConstruction(_BuildingsRepository.DocumentElement.ChildNodes[_BuildingSelectionDD.value]);

    }
    public void DeleteBuilding()
    {
        XmlDocument _BuildingsRepository = new XmlDocument();
        _BuildingsRepository.Load("C:/GD/You and world/v.0.0.1/First/Configs/Buildings.xml");
        _BuildingsRepository.DocumentElement.RemoveChild(_BuildingsRepository.DocumentElement.ChildNodes.Item(_BuildingSelectionDD.value));
        _BuildingsRepository.Save("C:/GD/You and world/v.0.0.1/First/Configs/Buildings.xml");
        UpdateBuildingsList();
        _BuildingSelectionDD.value = 0;
    }
}
// Полезная херня про линии
//GameObject _lineObject = new GameObject();
//LineRenderer _line = _lineObject.AddComponent<LineRenderer>();
//_line.SetPosition(0, _CameraRay.origin);
//_line.SetPosition(1, _CameraRay.direction);
//_line.SetWidth(0.05f, 0.05f);
//Destroy(_lineObject, 0.5f);