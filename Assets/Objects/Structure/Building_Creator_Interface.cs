using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using System.Xml;

public class Building_Creator_Interface : MonoBehaviour {

    private Dropdown _ResourceSelectDD { get { return Links.Interface.StructureCreationPanelResourceSelect; } }
    private Dropdown _WorkbrenchSelectDD { get { return Links.Interface.StructureCreationPanelWorkbenchSelect; } }
    //public Camera _Camera;
    public float _ScrollSpeed,_RotateSpeed,_AngleLimit,_TransformSpeed;
    public bool _SetDefaults;
    public string _BuildingName;

    private bool _BuildModeOff = true, _IsHit=false;
    private GameObject _BasicObject;
    private float _VerticalDeltaPos,_HorisontalDeltaPos;
    private Vector3 _Center,_RightForCameraDirection, _LocalCoordinates, _NewBrickPosition, _PreviousMousePosition, _Normal;
    private int _mask = 1 << 9;
    private RaycastHit _Hit;
    private List<GameObject> _BuildingElements = new List<GameObject>();
    private XmlDocument _BuildingsRepository = new XmlDocument();
    private string _RepositoryPass;
    private PhantomConstruction _PhantomWorkbench;
    private List<PhantomConstruction> _BuildingWorkbenches = new List<PhantomConstruction>();

    void Start()
    {
        
    }
    public void SetParams()
    {
        _ResourceSelectDD.options.Clear();
        _WorkbrenchSelectDD.options.Clear();
        if (_SetDefaults)
        {
            _ScrollSpeed = 3f;
            _RotateSpeed = 0.1f;
            _AngleLimit = 75;
            _TransformSpeed = 0.015f;
        }
        foreach (Resource _r in Resources.ResourceLibrary.FindAll(x=>x.MayBeUsedInStructures))
        {
            _ResourceSelectDD.options.Add(new Dropdown.OptionData(_r.Name, _r.Sprite));
            _ResourceSelectDD.value = 1;
            _ResourceSelectDD.value = 0;
            Debug.Log("Resource " + _r.Name + " added to select");
        }
        foreach (WorkbenchTemplate _t in Workbenches.Templates)
        {
            _WorkbrenchSelectDD.options.Add(new Dropdown.OptionData(_t.Name));
            _PhantomWorkbench = null;
        }
    }

    void Update()
    {
        if (!_BuildModeOff)
        {
            //Camera moving rules
            if (Input.mouseScrollDelta.y < 0 && _BasicObject.transform.localScale.x > 0.1f)
            {
                _BasicObject.transform.localScale += Vector3.one * (_ScrollSpeed * Input.mouseScrollDelta.y * Time.deltaTime); // когда крутишь колесиком, надо чуть чуть смещать основной блок к центру. (или отдалять)
                _BasicObject.transform.position -= Vector3.Normalize(_Center - _BasicObject.transform.position) * (_ScrollSpeed * Input.mouseScrollDelta.y * Time.deltaTime);
            }
            if (Input.mouseScrollDelta.y > 0 && _BasicObject.transform.localScale.x < 25f)
            {
                _BasicObject.transform.localScale += Vector3.one * (_ScrollSpeed * Input.mouseScrollDelta.y * Time.deltaTime);
                _BasicObject.transform.position -= Vector3.Normalize(_Center - _BasicObject.transform.position) * (_ScrollSpeed * Input.mouseScrollDelta.y * Time.deltaTime);
            }
            if (Input.GetMouseButtonDown(1))
            {
                _PreviousMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(1))
            {
                _HorisontalDeltaPos = (_PreviousMousePosition.x - Input.mousePosition.x);
                _VerticalDeltaPos = -(_PreviousMousePosition.y - Input.mousePosition.y);
                _BasicObject.transform.RotateAround(_BasicObject.transform.up, _HorisontalDeltaPos * _RotateSpeed / 32); // нужно чтоб фигура вращалась вокруг центра, а не вокруг основного блока. А для этого нужно перемещать основной блок немножко в сторону
                _BasicObject.transform.RotateAround(_RightForCameraDirection, _VerticalDeltaPos * _RotateSpeed / 32);
                _PreviousMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonDown(2))
            {
                _PreviousMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(2))
            {
                _VerticalDeltaPos = _PreviousMousePosition.y - Input.mousePosition.y;
                _BasicObject.transform.Translate(Vector3.up * _VerticalDeltaPos * -1 * _TransformSpeed);
                _HorisontalDeltaPos = _PreviousMousePosition.x - Input.mousePosition.x;
                _BasicObject.transform.position += _RightForCameraDirection * _HorisontalDeltaPos * -1 * _TransformSpeed;
                _PreviousMousePosition = Input.mousePosition;
            }

            //User inputs reaction
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (_IsHit)
                    Highlighter.UnHighLight(_Hit.transform.gameObject);
                _IsHit = Physics.Raycast(Links.MainCamera.ScreenPointToRay(Input.mousePosition), out _Hit, 200, _mask);
                if (_IsHit)
                {
                    /*if(_Hit.transform.tag == "Brick")*/ Highlighter.HighLight(_Hit.transform.gameObject);
                    //Debug.Log("Dont touch my " + _Hit.transform.name + "!!!1!11odinodin");
                    if (Input.GetKey("r"))
                        {
                            _Hit.transform.gameObject.GetComponent<Renderer>().material = Resources.ResourceLibrary[_ResourceSelectDD.value].Material;
                        }
                    if (Input.GetMouseButtonDown(0)&&_Hit.transform.tag == "Brick")
                    {
                        if (_PhantomWorkbench != null)
                        {
                            Highlighter.UnHighLight(_Hit.transform.gameObject);
                            _IsHit = false;
                            _PhantomWorkbench.UnPhantomize();
                            _BuildingWorkbenches.Add(_PhantomWorkbench);
                            Debug.Log("Create workbench, master local position is:" + _PhantomWorkbench.MasterObject.transform.localPosition);
                            ClearASpaceForWorkbench(_BuildingWorkbenches.Count - 1);
                            _PhantomWorkbench = null;
                        }
                        else
                        {
                            _Normal = _Hit.normal;
                            _NewBrickPosition = _Hit.transform.position + _Normal * _BasicObject.transform.lossyScale.x;
                            _BuildingElements.Add(Instantiate(Settings.BrickPrefab, _NewBrickPosition, _Hit.transform.rotation));
                            _BuildingElements[_BuildingElements.Count - 1].transform.localScale = _BasicObject.transform.localScale;
                            _BuildingElements[_BuildingElements.Count - 1].layer = 9;
                            _BuildingElements[_BuildingElements.Count - 1].transform.SetParent(_BasicObject.transform, true);
                            _BuildingElements[_BuildingElements.Count - 1].GetComponent<Renderer>().material = Resources.ResourceLibrary.Find(x => x.Name == _ResourceSelectDD.options[_ResourceSelectDD.value].text).Material;
                        }
                    }
                    if (_PhantomWorkbench != null)
                    {
                        _PhantomWorkbench.Hide();
                        _PhantomWorkbench.Show(_Hit.transform.localPosition);
                        if (Input.GetKeyDown("r"))
                        {
                            _PhantomWorkbench.Rotate();
                        }
                    }
                    if (Input.GetKeyUp("d"))
                    {
                        _IsHit = false;
                        switch (_Hit.transform.tag)
                        {                                
                            case "Brick":
                                _BuildingElements.Remove(_Hit.transform.gameObject);
                                Destroy(_Hit.transform.gameObject);
                                break;
                            case "WorkbenchBrick":
                                _BuildingWorkbenches.RemoveAll(x => x.MasterObject == _Hit.transform.parent.gameObject);
                                Destroy(_Hit.transform.parent.gameObject);
                            break;
                            default:
                                break;
                        }
                        if (_PhantomWorkbench != null) _PhantomWorkbench.Hide();
                    }
                }
                else
                    if (_PhantomWorkbench != null) _PhantomWorkbench.Hide();
                if (Input.GetMouseButtonDown(1))
                {
                    if (_PhantomWorkbench != null)
                    {
                        _PhantomWorkbench.Destroy();
                        _PhantomWorkbench = null;
                    }
                }
            }
        }
    }
    public void BuildingModeOn(Vector3 _Coordinates)
    {
        //SetParams();
        Links.MainCamera.GetComponent<camera_move>()._CameraCanMove = false;
        _Center = _Coordinates;
        _Normal = _Coordinates - Links.MainCamera.transform.position;
        _RightForCameraDirection = new Vector3(_Normal.z,0f,-_Normal.x);
        _RightForCameraDirection = Vector3.ClampMagnitude(_RightForCameraDirection,1);
        Debug.Log("Right to camera direction is " + _RightForCameraDirection);
        _BuildModeOff = false;
        _BasicObject = new GameObject();
        _BasicObject.transform.position = _Coordinates;
        _WorkbrenchSelectDD.value = 0;
        _WorkbrenchSelectDD.value = 1;
        _PhantomWorkbench.Destroy();
        _PhantomWorkbench = null;
        _BuildingElements.Add(Instantiate(Settings.BrickPrefab, _Coordinates, Quaternion.identity));
        Highlighter.Pick(_BuildingElements[0]);
        _BuildingElements[0].transform.SetParent(_BasicObject.transform, true);
        _BuildingElements[0].layer = 9;
        _BuildingElements[0].GetComponent<Renderer>().material = Resources.ResourceLibrary.Find(x => x.Name == _ResourceSelectDD.options[_ResourceSelectDD.value].text).Material;
    }
    public void BuildingModeOff()
    {
        Links.MainCamera.GetComponent<camera_move>()._CameraCanMove = true;
        Links.PlayerControl._PopupMode = true;
        _BuildModeOff = true;
        foreach (GameObject _obj in _BuildingElements)
        {
            if (_obj != null)
                Destroy(_obj);
        }
        Destroy(_BasicObject);
        _BasicObject = null;
        _BuildingWorkbenches.Clear();
        _BuildingElements.Clear();
        _BuildingName = null;
        Links.Interface.SwitchToMainMenu();
    }
    public void SaveTheBuilding()
    {
        if (_BuildingName == "")
        {
            Debug.LogError("Enter building name");
            return;
        }
        _BuildingsRepository.Load("C:/GD/You and world/v.0.0.1/First/Configs/Buildings.xml");
        XmlNode _Root = _BuildingsRepository.DocumentElement;
        XmlElement _Building = _BuildingsRepository.CreateElement("Building");
        _Root.AppendChild(_Building);
        _Building.SetAttribute("Name",_BuildingName);
        XmlElement _Version = _BuildingsRepository.CreateElement("Version");
        _Building.AppendChild(_Version);

        //Сохраняем станки
        XmlElement _Workbenches, _Workbench, _WBType, _WBRotation, _wbx, _wby, _wbz, _WBCoordinates;
        _Workbenches = _BuildingsRepository.CreateElement("Workbenches");
        foreach (PhantomConstruction _wb in _BuildingWorkbenches)
        {
            // Инициализация всех элементов
            _Workbench = _BuildingsRepository.CreateElement("Workbench");
            _WBType = _BuildingsRepository.CreateElement("Type");
            _WBRotation = _BuildingsRepository.CreateElement("Rotation");
            _WBCoordinates = _BuildingsRepository.CreateElement("RelativeCoordinates");
            _wbx = _BuildingsRepository.CreateElement("x");
            _wby = _BuildingsRepository.CreateElement("y");
            _wbz = _BuildingsRepository.CreateElement("z");

            // Присвоение элементам значений
            _WBType.InnerText = _wb.Type.ToString();
            _WBRotation.InnerText = _wb.Rotation.ToString();
            _wbx.InnerText = Mathf.RoundToInt(_wb.MasterObject.transform.localPosition.x).ToString();
            _wby.InnerText = Mathf.RoundToInt(_wb.MasterObject.transform.localPosition.y).ToString();
            _wbz.InnerText = Mathf.RoundToInt(_wb.MasterObject.transform.localPosition.z).ToString();
            Debug.Log("Master object coodrinates: " + _wb.MasterObject.transform.position + ", Master object local coordinates: " + _wb.MasterObject.transform.localPosition);

            // Построение иерархии
            _WBCoordinates.AppendChild(_wbx);
            _WBCoordinates.AppendChild(_wby);
            _WBCoordinates.AppendChild(_wbz);
            _Workbench.AppendChild(_WBCoordinates);
            _Workbench.AppendChild(_WBType);
            _Workbench.AppendChild(_WBRotation);
            _Workbenches.AppendChild(_Workbench);

            // Очистка обработанных объектов
            _BuildingElements.Remove(_wb.MasterObject);
            Destroy(_wb.MasterObject);
        }
        _BuildingWorkbenches.Clear();
        _Version.AppendChild(_Workbenches);

        //Сохраняем кирпичи
        XmlElement _Bricks,_Brick,_x,_y,_z,_RelativeCoordinates,_ResourceID;
        _Bricks = _BuildingsRepository.CreateElement("Bricks");
        foreach (GameObject _be in _BuildingElements)
        {
            _Brick = _BuildingsRepository.CreateElement("Brick");
            _RelativeCoordinates = _BuildingsRepository.CreateElement("RelativeCoordinates");
            _x = _BuildingsRepository.CreateElement("x");
            _y = _BuildingsRepository.CreateElement("y");
            _z = _BuildingsRepository.CreateElement("z");
            _RelativeCoordinates.AppendChild(_x);
            _RelativeCoordinates.AppendChild(_y);
            _RelativeCoordinates.AppendChild(_z);
            _x.InnerText = Mathf.RoundToInt(_be.transform.localPosition.x).ToString();
            _y.InnerText = Mathf.RoundToInt(_be.transform.localPosition.y).ToString();
            _z.InnerText = Mathf.RoundToInt(_be.transform.localPosition.z).ToString();
            _ResourceID = _BuildingsRepository.CreateElement("ResourceID");
            _ResourceID.InnerText = Resources.ResourceLibrary.Find(x => x.Material.mainTexture == _be.GetComponent<Renderer>().material.mainTexture).Type.ToString();
            _Brick.AppendChild(_RelativeCoordinates);
            _Brick.AppendChild(_ResourceID);
            _Bricks.AppendChild(_Brick);
        }
        _BuildingElements.Clear();
        _Version.AppendChild(_Bricks);
        _BuildingsRepository.Save("C:/GD/You and world/v.0.0.1/First/Configs/Buildings.xml");
        Debug.Log(_BuildingName + " saved");
        BuildingModeOff();
    }
    public void BuldingNameEnter(string _bn)
    {
        _BuildingName = _bn;
    }
    public void WorkbenchSelect(int _wbnum)
    {
        Debug.Log(_wbnum);
        _PhantomWorkbench = new PhantomConstruction(Workbenches.Templates[_wbnum]);
        _PhantomWorkbench.MasterObject.transform.SetParent(_BasicObject.transform,false);
    }

    private void ClearASpaceForWorkbench(int WorkBenchNum)
    {
        List<GameObject> SuperfluousBlocks = new List<GameObject>();
        foreach (GameObject _Bb in _BuildingElements)
            foreach (GameObject _Wbb in _BuildingWorkbenches[WorkBenchNum].Elements)
                if ((_Bb.transform.position == _Wbb.transform.position)/* && (!_Wbb._Basic)*/)
                    SuperfluousBlocks.Add(_Bb);
        //var SuperfluousBlocks = from _Brick in _BuildingElements
        //                        from _WbBrick in _BuildingWorkbenches[WorkBenchNum]._Elements
        //                        where (_Brick.transform.localPosition == _WbBrick.Element.transform.localPosition + _BuildingWorkbenches[WorkBenchNum]._MasterObject.transform.localPosition)
        //                        && (!_WbBrick._Basic)&&(_Brick != _BasicObject)
        //                        select _Brick;
        foreach (GameObject _go in SuperfluousBlocks)
        {
            _BuildingElements.Remove(_go);
            Destroy(_go);
        }
    }
}