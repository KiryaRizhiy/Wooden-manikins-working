using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Structure {

    ////Fields
    public ushort id { get; private set; }
    public string TypeName { get; private set; }
    public byte Version { get; private set; }
    //public List<GameObject> Bricks { get; private set; }
    public List<Workbench> BWorkbenches { get; private set; }
    public GameObject MasterObject { get; private set; }
    public byte Rotation { get; private set; }
    public StructureState State;

    //Constructor
    public Structure(PhantomConstruction Prototype)
    {
        //Consturction
        //Bricks = new List<GameObject>();
        BWorkbenches = new List<Workbench>();
        //XmlDocument _BuildingXMLDescription = new XmlDocument();
        //XmlNodeList _CoordinatesDescription;
        //Vector3 _Coordinates;
        MasterObject = new GameObject();

        //Defenition
        State = StructureState.New;
        id = Structures.BuildingID;
        Version = 0;
        Rotation = Prototype.Rotation;
        TypeName = Prototype.TypeName;

        //Processing
        MasterObject.name = TypeName + ":" + id.ToString();
        MasterObject.transform.SetParent(Map.GetBrick(Prototype.MasterObject.transform.position).gameObject.transform.parent);
        MasterObject.transform.position = Prototype.MasterObject.transform.position;
    }

    //Constructor
    //public Building(int BuildingNubmer, byte Rotate, Vector3 StartPoint)
    //{
    //    //Consturction
    //    Bricks = new List<GameObject>();
    //    BWorkbenches = new List<Workbench>();
    //    XmlDocument _BuildingXMLDescription = new XmlDocument();
    //    XmlNodeList _CoordinatesDescription;
    //    Vector3 _Coordinates;
    //    MasterObject = new GameObject();

    //    //Defenition
    //    _BuildingXMLDescription.Load(Settings.BuildingsStoragePath);
    //    id = Buildings.BuildingID;
    //    Version = 0;
    //    Rotation = Rotate;
    //    TypeName = _BuildingXMLDescription.DocumentElement.ChildNodes[BuildingNubmer].Attributes[0].InnerText;
    //    XmlNodeList _Bricks = _BuildingXMLDescription.DocumentElement.ChildNodes[BuildingNubmer].ChildNodes[Version].ChildNodes[1/*Bricks*/].ChildNodes;
    //    XmlNodeList _Workbenches = _BuildingXMLDescription.DocumentElement.ChildNodes[BuildingNubmer].ChildNodes[Version].ChildNodes[0/*Workbenches*/].ChildNodes;

    //    //Processing
    //    MasterObject.name = TypeName + ":" + id.ToString();
    //    MasterObject.transform.SetParent(Links.LSettings.gameObject.transform);
    //    MasterObject.transform.position = StartPoint;
    //    foreach (XmlNode _bd in _Bricks)
    //    {
    //        _CoordinatesDescription = _bd.ChildNodes[0].ChildNodes;
    //        _Coordinates = new Vector3(float.Parse(_CoordinatesDescription[0].InnerText), float.Parse(_CoordinatesDescription[1].InnerText), float.Parse(_CoordinatesDescription[2].InnerText));
    //        for (byte i = 0; i < Rotation; i++)
    //            _Coordinates = Functions.RightVector(_Coordinates);
    //        ushort _ResID = ushort.Parse(_bd.ChildNodes.Item(1).InnerText);
    //        GameObject _Element = Links.Bricks.AddBrick(Functions.ObjectToWorldCoordinates(StartPoint + _Coordinates), _ResID, true);
    //        _Element.transform.SetParent(MasterObject.transform, false);
    //        _Element.transform.localPosition = _Coordinates;
    //        _Element.GetComponent<MeshRenderer>().material = Player_control_script.Resources.GetResource(_ResID).Material;
    //        _Element.tag = "BuildingBrick";
    //    }
    //    foreach (XmlNode _wb in _Workbenches)
    //    {
    //        _CoordinatesDescription = _wb.ChildNodes[0].ChildNodes;
    //        _Coordinates = new Vector3(float.Parse(_CoordinatesDescription[0].InnerText), float.Parse(_CoordinatesDescription[1].InnerText), float.Parse(_CoordinatesDescription[2].InnerText));
    //        for (byte i = 0; i < Rotation; i++)
    //            _Coordinates = Functions.RightVector(_Coordinates);
    //        ushort _Template = ushort.Parse(_wb.ChildNodes.Item(1).InnerText);
    //        byte _rotation = byte.Parse(_wb.ChildNodes.Item(2).InnerText);
    //        BWorkbenches.Add(new Workbench(_Coordinates + StartPoint, _Template, (byte)(_rotation + Rotation), this));
    //    }
    //}

    //Public methods
    public bool HasAWorkbench(ushort WBType)
    {
        if (BWorkbenches.Find(x => x.Type == WBType) != null)
            return true;
        else
            return false;
    }
}
public class StructureState
{
    public static StructureState New { get { return _new; } }
    public static StructureState Building { get { return _building; } }
    public static StructureState Active { get { return _active; } }

    private static StructureState _new = new StructureState(0);
    private static StructureState _building = new StructureState(1);
    private static StructureState _active = new StructureState(2);

    private int _statusId;

    private StructureState(int _i)
    {
        _statusId = _i;
    }
}