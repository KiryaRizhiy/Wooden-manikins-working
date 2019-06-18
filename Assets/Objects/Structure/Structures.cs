using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class Structures{

    //Identifyers
    public static ushort BuildingID
    {
        get
        {
            _BuildingID += 1;
            return _BuildingID;
        }
        private set
        {
            _BuildingID = value;
        }
    }
    //Identifyers twins
    private static ushort _BuildingID;

    private static List<BuildingRequirements> AllBuildingsReqirements;
    public static List<Structure> BuildingList { get; private set; }

    private static string scr = "Buildings";

    public static void Initialize()
    {
        AllBuildingsReqirements = new List<BuildingRequirements>();
        BuildingList = new List<Structure>();
        UploadTemplates();
    }
    public static void UploadTemplates()
    {
        AllBuildingsReqirements.Clear();
        XmlDocument _allBuildings = new XmlDocument();
        _allBuildings.Load(Settings.BuildingsStoragePath);
        foreach (XmlNode _b in _allBuildings.DocumentElement.ChildNodes)
                AllBuildingsReqirements.Add(new BuildingRequirements(_b));
    }
    public static Structure AddBuilding(PhantomConstruction Prototype)
    {
        Structure _b = new Structure(Prototype);
        BuildingList.Add(_b);
        return _b;
    }

    //Public methods
    //public static void AddBuilding(Building BuildingObj)
    //{
    //    BuildingList.Add(BuildingObj);
    //}
    public static BuildingRequirements GetRequiredresources(string BuildingName, byte Version = 0)
    {
        return AllBuildingsReqirements.Find(x => (x.TypeName == BuildingName) && (x.Version == Version));
    }

}
//Classes
public class BuildingRequirements
{
    public ushort id { get; private set; }
    public string TypeName { get; private set; }
    public byte Version { get; private set; }
    public List<ResourceEnumerator> ResourceRequirements { get; private set; }
    private string scr = "Buildings";
    public BuildingRequirements(XmlNode BuildingXmlDescription)
    {
        ResourceRequirements = new List<ResourceEnumerator>();
        Version = 0;
        TypeName = BuildingXmlDescription.Attributes[0].InnerText;
        XmlNode BuildingVersionXmlDescription = BuildingXmlDescription.ChildNodes[0/*version*/];
        foreach (XmlNode _b in BuildingVersionXmlDescription.ChildNodes[1].ChildNodes)//Считываем обычные кирпичи
        {
            ushort _rtype;
            ResourceEnumerator _currEnr;
            _rtype = ushort.Parse(_b.ChildNodes[1].InnerText);
            _currEnr = ResourceRequirements.Find(x => x.Resource.Type == _rtype);
            if (_currEnr == null)
                ResourceRequirements.Add(new ResourceEnumerator(_rtype, 1));
            else
                _currEnr.Count++;
        }
        foreach (XmlNode _w in BuildingVersionXmlDescription.ChildNodes[0].ChildNodes)//Считываем станки
        {
            ushort _wbtype;
            ResourceEnumerator _currEnr;
            _wbtype = ushort.Parse(_w.ChildNodes[1].InnerText);
            foreach (WorkbenchElement _wbe in Workbenches.Templates.Find(x => x.Type == _wbtype).Elements)
            {
                _currEnr = ResourceRequirements.Find(x => x.Resource.Type == _wbe.ResourceID);
                if (_currEnr == null)
                    ResourceRequirements.Add(new ResourceEnumerator(_wbe.ResourceID, 1));
                else
                    _currEnr.Count++;
            }
        }
        Log.Notice(scr, this);
    }
}