using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PhantomConstruction {

    //Fields
    public GameObject MasterObject { get; private set; }
    public List<GameObject> Elements { get; private set; }//Нахер не нужен, удалить
    public List<PhantomConstruction> ChildConstructions { get; private set; }
    //public string Tag { get; private set; }
    public byte Rotation { get; private set; }
    //public ushort ConstructionType { get; private set; }
    public string TypeName { get; private set; }
    public ushort Type { get; private set; }

    private string scr = "PhantomConstruction";

    //Public methods
    public void Show(Vector3 Coordinates)
    {
        MasterObject.transform.localPosition = Coordinates;
        MasterObject.SetActive(true);
    }
    public void Hide()
    {
        MasterObject.SetActive(false);
    }
    public void Rotate(byte R = 1)
    {
        MasterObject.transform.localEulerAngles += 90 * Vector3.up*R;
        Rotation += R;
        Log.Notice(scr,"Rotate the " + MasterObject.name + " in " + R + ". Resulting rotation is: " + ((byte)(Rotation % 4)));
        Rotation = (byte)(Rotation%4);
    }
    public void UnPhantomize()
    {
        foreach (GameObject _el in Elements)
        {
            _el.tag = "WorkbenchBrick";
            _el.layer = 9;
        }
        MasterObject.tag = "WorkbenchBrick";
        Highlighter.UnHighLight(MasterObject);
    }
    public void Destroy()
    {
        foreach (PhantomConstruction _phc in ChildConstructions)
            _phc.Destroy();
        this.Hide();
        Elements.Clear();
        MonoBehaviour.Destroy(MasterObject);
    }

    //Constructors
    public PhantomConstruction(XmlNode BuildingDesc)
    {
        //Construction
        Elements = new List<GameObject>();
        ChildConstructions = new List<PhantomConstruction>();
        MasterObject = new GameObject();

        //Logs
        //Debug.Log("Phantom building counstructor called");

        //Defenitions
        TypeName = BuildingDesc.Attributes[0].InnerText;
        MasterObject.name = BuildingDesc.Attributes[0].InnerText;
        foreach (XmlNode _brick in BuildingDesc.ChildNodes[0/*version*/].ChildNodes[1].ChildNodes)
        {
            Elements.Add(AddElement(_brick));
        }
        foreach (XmlNode _wb in BuildingDesc.ChildNodes[0/*version*/].ChildNodes[0].ChildNodes)
        {
            Elements.Add(AddElement(_wb));
        }
        Hide();
    }
    public PhantomConstruction(WorkbenchTemplate Template)
    {
        //Construction
        Elements = new List<GameObject>();
        ChildConstructions = new List<PhantomConstruction>();
        MasterObject = new GameObject();

        //Logs
        //Debug.Log("Phantom building counstructor called");

        //Defenitions
        Type = Template.Type;
        TypeName = Template.Name;
        MasterObject.name = Template.Name;
        foreach (WorkbenchElement _el in Template.Elements)
        {
            Elements.Add(AddElement(_el));
        }
        Hide();
    }

    //Private methods
    private GameObject AddElement(XmlNode ElementDescription)
    {
        GameObject _Element;
        Vector3 _Coordinates;
        XmlNodeList _CoordinatesDescription;
        switch (ElementDescription.Name)
        {
            case "Workbench":
                //Defenition
                ushort _type = ushort.Parse(ElementDescription.ChildNodes[1].InnerText);
                PhantomConstruction _phc = new PhantomConstruction(Workbenches.Templates.Find(x=>x.Type == _type));
                _CoordinatesDescription = ElementDescription.ChildNodes[0].ChildNodes;
                _Coordinates = new Vector3(float.Parse(_CoordinatesDescription[0].InnerText), float.Parse(_CoordinatesDescription[1].InnerText) + 0.05f, float.Parse(_CoordinatesDescription[2].InnerText));
                byte _rotation = byte.Parse(ElementDescription.ChildNodes[2].InnerText);

                //Processing
                ChildConstructions.Add(_phc);
                _phc.MasterObject.transform.SetParent(MasterObject.transform);
                _phc.Rotate(_rotation);
                _phc.Show(_Coordinates);
                _Element = _phc.MasterObject;
                break;
            case "Brick":
                //Defenition
                _CoordinatesDescription = ElementDescription.ChildNodes[0].ChildNodes;
                _Coordinates = new Vector3(float.Parse(_CoordinatesDescription[0].InnerText), float.Parse(_CoordinatesDescription[1].InnerText)+0.05f, float.Parse(_CoordinatesDescription[2].InnerText));
                ushort _ResID = ushort.Parse(ElementDescription.ChildNodes.Item(1).InnerText);

                //Processing
                _Element = MonoBehaviour.Instantiate(Settings.BrickPrefab, Vector3.zero, Quaternion.identity);
                _Element.transform.SetParent(MasterObject.transform, false);
                _Element.transform.localPosition = _Coordinates;
                _Element.GetComponent<MeshRenderer>().material = Resources.GetResource(_ResID).Material;
                _Element.name = Resources.GetResource(_ResID).Name;
                _Element.tag = "Untagged";
                _Element.layer = 10;
                Highlighter.Phantomize(_Element);
            break;
            default:
            Log.Warning(scr,"Unknown element type in phantom constructing: ");
            Functions.ReadXMLNode(ElementDescription,scr);
            _Element = null;
            break;
        }
        return _Element;
    }
    private GameObject AddElement(WorkbenchElement ElementDescription)
    {
        GameObject _Element = MonoBehaviour.Instantiate(Settings.BrickPrefab, Vector3.zero, Quaternion.identity);
        _Element.transform.SetParent(MasterObject.transform, false);
        _Element.transform.localPosition = ElementDescription.Coordinates;
        _Element.GetComponent<MeshRenderer>().material = Resources.GetResource(ElementDescription.ResourceID).Material;
        _Element.name = Resources.GetResource(ElementDescription.ResourceID).Name;
        _Element.tag = "Untagged";
        _Element.layer = 10;
        Highlighter.Phantomize(_Element);
        return _Element;
    }
}