using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public static class Workbenches
{

    public static List<WorkbenchTemplate> Templates { get; private set; }
    public static ushort WorkbenchID
    {
        get
        {
            _workbenchid += 1;
            return _workbenchid;
        }
        private set
        {
            return;
        }
    }

    private static ushort _workbenchid = 0;

    public static void UploadTemplates()
    {
        Templates = new List<WorkbenchTemplate>();
        XmlDocument Workbranches = new XmlDocument();
        Workbranches.Load(Settings.WorkbenchesStoragePath);
        foreach (XmlNode _t in Workbranches.DocumentElement.ChildNodes)
            Templates.Add(new WorkbenchTemplate(_t));
    }
}
public class WorkbenchTemplate
{
    public string Name { get; private set; }
    public ushort Type { get; private set; }
    public List<WorkbenchElement> Elements { get; private set; }
    public ushort BasicBrickID { get; private set; }
    public WorkbenchTemplate(XmlNode Workbench)
    {
        Elements = new List<WorkbenchElement>();
        Name = Workbench.Attributes[0].InnerText;
        Type = ushort.Parse(Workbench.Attributes[1].InnerText);
        BasicBrickID = ushort.MaxValue;
        for (ushort i = 0; i < Workbench.ChildNodes[0].ChildNodes.Count; i++)
        {
            if (Workbench.ChildNodes[0].ChildNodes[i].Attributes.Count > 0)
                if (Workbench.ChildNodes[0].ChildNodes[i].Attributes[0].Name == "Main" && Workbench.ChildNodes[0].ChildNodes[i].Attributes[0].InnerText == "True")
                    BasicBrickID = i;
            //Functions.ReadXMLNode(Workbench.ChildNodes[0].ChildNodes[i].ChildNodes[0]);
            Elements.Add(new WorkbenchElement(Workbench.ChildNodes[0].ChildNodes[i],i==BasicBrickID));
        }
        //this.Log();
    }
    public void Log()
    {
        Debug.Log("Logging workbench named " + Name);
        foreach (WorkbenchElement _e in Elements)
            _e.Log();
    }
}
public class WorkbenchElement
{
    public Vector3 Coordinates { get; private set; }
    public ushort ResourceID { get; private set; }
    public bool BasicBrick { get; private set; }
    public WorkbenchElement(XmlNode ElementXMLDescription, bool Basic = false)
    {
        XmlNodeList _XMLCoordinates = ElementXMLDescription.ChildNodes[0].ChildNodes;
        Coordinates = new Vector3(float.Parse(_XMLCoordinates[2].InnerText), float.Parse(_XMLCoordinates[1].InnerText), float.Parse(_XMLCoordinates[0].InnerText));
        ResourceID = ushort.Parse(ElementXMLDescription.ChildNodes[1].InnerText);
        BasicBrick = Basic;
    }
    public void Log()
    {
        Debug.Log("Coordinates: " + Coordinates + " Resource: " + Resources.GetResource(ResourceID).Name);
    }
}