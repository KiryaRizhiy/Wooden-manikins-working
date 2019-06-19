using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench {

    public string Name { get; private set; }
    public ushort Type { get; private set; }
    public ushort ID { get; private set; }
    //public List<GameObject> Bricks { get; private set; }
    public GameObject Base { get; private set; }
    public GameObject Master { get; private set; }
    public Sack Sack { get; private set; }

    //public Workbench(Vector3 StartPoint, ushort TemplateType, byte Rotation, Structure ParentBuilding)
    //{
    //    //initialization
    //    Master = new GameObject();
    //    //Bricks = new List<GameObject>();
    //    Sack = new Sack(Master);

    //    Type = TemplateType;
    //    WorkbenchTemplate Template = Workbenches.Templates.Find(x => x.Type == Type);
    //    Name = Template.Name;
    //    ID = Workbenches.WorkbenchID;
    //    Master.transform.position = StartPoint;
    //    Master.transform.SetParent(ParentBuilding.MasterObject.transform,true);
    //    Master.name = Template.Name;
    //    Master.tag = "WorkbenchBrick";
    //    foreach (WorkbenchElement _e in Template.Elements)
    //    {
    //        Vector3 _RotatedCoordinates = new Vector3();
    //        _RotatedCoordinates = _e.Coordinates;
    //        for (byte i = 0; i < Rotation; i++)
    //            _RotatedCoordinates = Functions.RightVector(_RotatedCoordinates);
    //        GameObject _CurrentBrick = Links.Bricks.AddBrick(Functions.ObjectToWorldCoordinates(StartPoint + _RotatedCoordinates), _e.ResourceID, true);
    //        //Bricks.Add(_CurrentBrick);
    //        if (_e.BasicBrick) Base = _CurrentBrick;
    //        _CurrentBrick.transform.SetParent(Master.transform, true);
    //        _CurrentBrick.tag = "WorkbenchBrick";
    //    }
    //}
    public Workbench(PhantomConstruction Prototype, Structure ParentBuilding)
    {
        Master = new GameObject();
        Sack = new Sack(Master);
        Base = Master;

        Type = Prototype.Type;
        Name = Prototype.TypeName;
        ID = Workbenches.WorkbenchID;
        Master.transform.position = Prototype.MasterObject.transform.position;
        Master.transform.SetParent(ParentBuilding.MasterObject.transform, true);
        Master.name = Prototype.TypeName;
        Master.tag = "WorkbenchBrick";
    }
}
