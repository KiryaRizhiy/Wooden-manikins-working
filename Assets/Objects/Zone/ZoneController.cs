using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ZoneController {

    public static List<Storage> AllStorages { get; private set; }
    public static List<Field> AllFields { get; private set; }
    public static List<Pasture> AllPastures { get; private set; }
    public static List<Mine> AllMines { get; private set; }
    public static List<Gathering> AllGatherings { get; private set; }

    public static ushort ZoneCount
    {
        get
        {
            _zc += 1;
            return _zc;
        }
    }
    public static ushort _zc = 0;
    private static string scr = "ZoneController";

    public static void Initialize()
    {
        AllStorages = new List<Storage>();
        AllFields = new List<Field>();
        AllPastures = new List<Pasture>();
        AllMines = new List<Mine>();
        AllGatherings = new List<Gathering>();
    }

    //Processing
    public static void CreateZone(Vector3 _sp, Vector3 _ep, byte ZoneType, byte StorageType = 0)
    {
        Vector3 _StartPoint, _EndPoint;
        Functions.OrderVectors(_sp, _ep, out _StartPoint, out _EndPoint);
        switch (ZoneType)
        {
            case 0:
                Log.Notice(scr,"ZoneTypeIsMissing");
                break;
            case 1:
                AllStorages.Add(new Storage(_StartPoint,_EndPoint));
                break;
            case 2:
                AllFields.Add(new Field(_StartPoint,_EndPoint));
                break;
            case 3:
                AllPastures.Add(new Pasture(_StartPoint,_EndPoint));
                break;
            case 4:
                AllMines.Add(new Mine(_StartPoint,_EndPoint));
                AllMines[AllMines.Count - 1].HighlightAll();
                break;
            case 5:
                AllGatherings.Add(new Gathering(_StartPoint, _EndPoint));
                AllGatherings[AllGatherings.Count - 1].HighlightAll();
                break;
            default:
                Log.Notice(scr, "Zone type " + ZoneType + " undefined");
                break;
        }
    }
    public static void RemoveComponent(GameObject _obj)
    {
        foreach (Storage _s in AllStorages)
        {
            _s.RemoveBrick(_obj);
        }
        foreach (Field _s in AllFields)
        {
            _s.RemoveBrick(_obj);
        }
        foreach (Pasture _s in AllPastures)
        {
            _s.RemoveBrick(_obj);
        }
        foreach (Mine _s in AllMines)
        {
            _s.RemoveBrick(_obj);
        }
        foreach (Gathering _s in AllGatherings)
        {
            _s.RemoveBrick(_obj);
        }
    }

    //Interaction
    public static Storage GetStorage(ushort ID)
    {
        return AllStorages.Find(x => x.ID == ID);
    } //Переделать, надо смотреть на статус
    public static Storage GetStorage(string Name)
    {
        return AllStorages.Find(x => x.Name == Name);
    }
    public static Storage GetStorage(ushort ResourceType, short Count)
    {
        return AllStorages.Find(x => x.StorageSack.HasAResource(Resources.ResourceLibrary.Find(y => y.Type == ResourceType), Count));
    }
    public static Storage GetStorage(ResourceTypeClassification Classification, out Resource ToolType)
    {
        foreach (Resource _r in Resources.ResourceLibrary.FindAll(res => res.Classification == Classification))
            if (AllStorages.FindAll(s => s.StorageSack.HasAResource(_r, 1)).Count > 0)
            {
                ToolType = _r;
                return AllStorages.Find(s => s.StorageSack.HasAResource(_r, 1));
            }
        ToolType = null;
        return null;
    }
    public static Field GetField(ushort ID)
    {
        return AllFields.Find(x => x.ID == ID);
    }
    public static Field GetField(string Name)
    {
        return AllFields.Find(x => x.Name == Name);
    }
    public static Pasture GetPasture(ushort ID)
    {
        return AllPastures.Find(x => x.ID == ID);
    }
    public static Pasture GetPasture(string Name)
    {
        return AllPastures.Find(x => x.Name == Name);
    }
    public static Mine GetMine(ushort ID)
    {
        return AllMines.Find(x => x.ID == ID);
    }
    public static Mine GetMine(string Name)
    {
        return AllMines.Find(x => x.Name == Name);
    }
    public static Gathering GetGatheringZone(string Name)
    {
        return AllGatherings.Find(x => x.Name == Name);
    }
    public static Zone GetZone(ushort ID)
    {
        return (new List<Zone> { AllMines.Find(x => x.ID == ID), AllPastures.Find(x => x.ID == ID), AllStorages.Find(x => x.ID == ID) , AllFields.Find(x => x.ID == ID)}).Find(x => x != null);
    }

    //Info storage //Переделать конструктор зон
    public abstract class Zone
    {
        public ushort ID { get; private set; }
        public string Name { get; private set; }
        public byte Type { get; private set; }
        //1 - Склад
        //2 - Поле
        //3 - Пастбище
        //4 - Зона добычи
        public ZoneState State
        {
            get
            {
                return _state;
            }
            private set
            {
                Log.Notice(scr,"Zone " + Name + " status changed from: " + _state.Description + " to: " + value.Description);
                _state = value;
            }
        }
        public string StringType { get; protected set; }
        public bool HasPointer {get {return (_Description!=null&&_Description.activeInHierarchy);}}

        protected List<GameObject> _Bricks = new List<GameObject>();

        //twins fields
        private ZoneState _state = ZoneState.New;
        private GameObject _Description;

        public Zone(byte ZoneType,Vector3 ZoneStartPoint, Vector3 ZoneEndPoint, string ZoneName = null)
        {
            State = ZoneState.Active;
            switch (ZoneType)
            {
                case 0:
                    StringType = "ZoneTypeIsMissing";
                    break;
                case 1:
                    StringType = "Storage";
                    break;
                case 2:
                    StringType = "Field";
                    break;
                case 3:
                    StringType = "Pasture";
                    break;
                case 4:
                    StringType = "Mine";
                    break;
                case 5:
                    StringType = "Gathering";
                    break;
                default:
                    StringType = "Zone type " + ZoneType.ToString() + " undefined";
                    break;
            }
            ID = ZoneController.ZoneCount;
            if (ZoneName == null)
                Name = StringType + "-" + ID.ToString();
            else
                Name = ZoneName;
            Vector3 _coords;
            Log.Notice(scr, "Creating zone between " + ZoneStartPoint + " and " + ZoneEndPoint);
            if (ZoneStartPoint != ZoneEndPoint)
            {
                for (int i = 0; i <= Mathf.RoundToInt(ZoneEndPoint.x) - Mathf.RoundToInt(ZoneStartPoint.x); i++)
                    for (int j = 0; j <= Mathf.RoundToInt(ZoneEndPoint.y) - Mathf.RoundToInt(ZoneStartPoint.y); j++)
                        for (int k = 0; k <= Mathf.RoundToInt(ZoneEndPoint.z) - Mathf.RoundToInt(ZoneStartPoint.z); k++)
                        {
                            _coords = ZoneStartPoint + i * Vector3.forward + j * Vector3.up + k * Vector3.right;
                            if (Map.BrickExists(_coords))
                            {
                                if (Map.GetBrick(_coords).gameObject.activeInHierarchy)
                                {
                                    Map.GetBrick(_coords).SetZoneId(this.ID);
                                    this.AddBrick(Map.GetBrick(_coords).gameObject);
                                }
                                if (!HasPointer)
                                {
                                    ShowZonePointer(_coords);
                                }
                            }
                            else
                                Log.Notice(scr, i + ":" + j + ":" + k + ":" + " Check fail");
                        }
                Log.Notice(scr, "Zone #" + this.ID + " created. Zone type is '" + this.StringType + "'");
            }
            else
                Log.Notice(scr, "Zone #" + this.ID + " created. Zone type is '" + this.StringType + "'. Created zone is empty");
            //HighlightAll();
        }

        //Interactions
        public virtual GameObject GetTarget()
        {
            return _Bricks[Random.Range(0, _Bricks.Count)];
        }
        public virtual void AddBrick(GameObject _b)
        {
            _Bricks.Add(_b);
            //_b.GetComponent<Brick>().SetZoneId(ID);
            //Log.Notice(scr,"Successfull check. Brick added to zone " + Name);
        }
        public virtual void RemoveBrick(GameObject _b)
        {
            _Bricks.RemoveAll(x => x == _b);
            _b.GetComponent<Brick>().SetZoneId();
            if (_Bricks.Count == 0)
                Disable();
        }
        public virtual void Disable()
        {
            State = ZoneState.Disabled;
        }

        //Visual
        public void HighlightAll()
        {
            foreach (GameObject _obj in _Bricks)
                Highlighter.HighLight(_obj,true);
        }
        public void UnHighlightAll()
        {
            foreach (GameObject _obj in _Bricks)
                Highlighter.UnHighLight(_obj,true);
        }
        public virtual void ShowZoneDetails()
        {
            Log.Notice(scr,"Zone " + Name + ". Id: " + ID + ". Type: " + Type + "-" + StringType + ". Zone state: " + State.Description);
        }
        public IEnumerator ShowZone()
        {
        HighlightAll();
        yield return new WaitForSeconds(0.5f);
        UnHighlightAll();
        yield return new WaitForSeconds(0.1f);
        HighlightAll();
        yield return new WaitForSeconds(0.5f);
        UnHighlightAll();
        yield return new WaitForSeconds(0.1f);
        HighlightAll();
        yield return new WaitForSeconds(0.5f);
        UnHighlightAll();
        yield return new WaitForSeconds(0.1f);
        }
        public void ShowZonePointer(Vector3 Coordinates)
        {
            //GameObject _br = Map.GetBrick(Coordinates).gameObject;
            //Create and place object
            _Description = new GameObject();
            _Description.name = "Zone description";
            //_Description.transform.SetParent(_br.transform, false);
            _Description.transform.position += Vector3.up * 3.5f + Coordinates;
            //Draw the line
            LineRenderer _lrend = _Description.AddComponent<LineRenderer>();
            _lrend.SetPosition(0, _Description.transform.position - Vector3.up * 3.5f);
            _lrend.SetPosition(1, _Description.transform.position);
            _lrend.startColor = Color.white;
            _lrend.endColor = Color.black;
            _lrend.startWidth = 0.03f;
            _lrend.endWidth = 0.05f;
            _lrend.material = new Material(Shader.Find("Unlit/Texture"));
            //Write the text
            TextMesh _txt = _Description.AddComponent<TextMesh>();
            _txt.font = Settings.MenuTextFont;
            _txt.text = "Зона: " + this.Name;
            _txt.anchor = TextAnchor.LowerCenter;
            _txt.characterSize = 0.35f;
            //_txt.
            //Make the text to follow the camera
            _Description.AddComponent<BoxCollider>();
            _Description.GetComponent<BoxCollider>().center = Vector3.zero;
            _Description.GetComponent<BoxCollider>().size = Vector3.one + Vector3.right * 2;
            _Description.AddComponent<UICameraFollower>();
            _Description.AddComponent<ZoneInteractor>().Connect(this);
            _Description.AddComponent<Basic>();
            _Description.layer = 11;
        }
    }
    public class Storage : Zone
    {
        public byte RepositoryType;
        public Sack StorageSack { get; private set; }
        public Storage(Vector3 StorageStartPoint, Vector3 StorageEndPoint):base(1,StorageStartPoint,StorageEndPoint)
        {
            StorageSack = new Sack(this.GetTarget(), 1);
        }
        public Storage(PhantomConstruction PitTemplate)
            : base(1, Vector3.zero, Vector3.zero, "A resources storage for building the " + PitTemplate.MasterObject.name)
        {
            foreach (GameObject _b in PitTemplate.Elements)
            {
                if (_b.transform.childCount == 0)
                {
                    if (Map.BrickExists(_b.transform.position) && Map.GetBrick(_b.transform.position).gameObject.activeInHierarchy)
                        AddBrick(Map.GetBrick(_b.transform.position).gameObject);
                }
                else
                    foreach (GameObject _subb in Functions.GetAllChildren(_b))
                    {
                        if (Map.BrickExists(_subb.transform.position) && Map.GetBrick(_subb.transform.position).gameObject.activeInHierarchy)
                            AddBrick(Map.GetBrick(_subb.transform.position).gameObject);
                    }
            }
            ZoneController.AllStorages.Add(this);
            StorageSack = new Sack(this.GetTarget(), 1);
        }
        public override void RemoveBrick(GameObject _b)
        {
            _Bricks.RemoveAll(x => x == _b);
        }
        public override void ShowZoneDetails()
        {
            base.ShowZoneDetails();
            Links.Interface.ShowSack(StorageSack);
        }
    }
    public class Field : Zone
    {
        public Field(Vector3 FieldStartPoint, Vector3 FieldEndPoint):base(2,FieldStartPoint,FieldEndPoint)
        {
        }
    }
    public class Pasture : Zone
    {
        public Pasture(Vector3 PastureStartPoint, Vector3 PastureEndPoint):base(3,PastureStartPoint,PastureEndPoint)
        {
        }
    }
    public class Mine : Zone
    {
        public override GameObject GetTarget()
        {
            foreach (GameObject _c in _Bricks)
            {
                if (!(_c.GetComponent<Basic>().Busy))
                {
                    _c.GetComponent<Basic>().Busy = true;
                    Log.Notice(scr,"Returning a component " + _c);
                    return _c;
                }
            }
            Log.Notice(scr,StringType + "'" + Name + "' has no free components");
            return null;
        }
        public Mine(Vector3 MineStartPoint, Vector3 MineEndPoint):base(4,MineStartPoint,MineEndPoint)
        {
        }
        public Mine(PhantomConstruction PitTemplate)
            : base(4, Vector3.zero, Vector3.zero, "A pit for" + PitTemplate.MasterObject.name)
        {
            foreach (GameObject _b in PitTemplate.Elements)
            {
                if (_b.transform.childCount == 0)
                {
                    if (Map.BrickExists(_b.transform.position) && Map.GetBrick(_b.transform.position).gameObject.activeInHierarchy)
                        AddBrick(Map.GetBrick(_b.transform.position).gameObject);
                }
                else
                    foreach (GameObject _subb in Functions.GetAllChildren(_b))
                    {
                        if (Map.BrickExists(_subb.transform.position) && Map.GetBrick(_subb.transform.position).gameObject.activeInHierarchy)
                            AddBrick(Map.GetBrick(_subb.transform.position).gameObject);
                    }
            }
            ZoneController.AllMines.Add(this);
        }
    }
    public class Gathering : Zone
    {
        public Gathering(Vector3 GatheringStartPoint, Vector3 GatheringEndPoint)
            : base(5, Vector3.zero, Vector3.zero)
        {
            Vector3 _coords;
            for (int i = 0; i <= Mathf.RoundToInt(GatheringEndPoint.x) - Mathf.RoundToInt(GatheringStartPoint.x); i++)
                for (int j = 0; j <= Mathf.RoundToInt(GatheringEndPoint.y) - Mathf.RoundToInt(GatheringStartPoint.y); j++)
                    for (int k = 0; k <= Mathf.RoundToInt(GatheringEndPoint.z) - Mathf.RoundToInt(GatheringStartPoint.z); k++)
                        {
                            _coords = GatheringStartPoint + i * Vector3.forward + j * Vector3.up + k * Vector3.right;
                            if (Map.BrickExists(_coords))
                            {
                                GameObject _b = Map.GetBrick(_coords).gameObject;
                                if (_b.activeInHierarchy&&_b.transform.childCount==1)
                                {
                                    if (_b.transform.GetChild(0).tag == "Tree")
                                        AddBrick(_b.transform.GetChild(0).gameObject);
                                }
                            }
                            else
                                Log.Notice(scr, "Check fail");
                        }
                Log.Notice(scr, "Zone #" + this.ID + " created. Zone type is '" + this.StringType + "'");
                //HighlightAll();
        }
        public bool IsEmpty()
        {
            //foreach (ZoneComponent _t in _Bricks)
            //{
            //    Debug.Log(_t._Component.name + " of " + _t._Component.transform.parent.name + " and branches exists: " + _t._Component.GetComponent<Tree>().HasBranches);
            //}
            return _Bricks.Find(x => x.GetComponent<Tree>().HasBranches && !x.GetComponent<Basic>().Busy) == null;
        }
        public override GameObject GetTarget()
        {
            return _Bricks.Find(x=>x.GetComponent<Tree>().HasBranches);
        }
    }
}
public class ZoneState
{
    public static ZoneState New { get { return _new; } }
    public static ZoneState Active { get { return _active; } }
    public static ZoneState Disabled { get { return _disabled; } }

    private static ZoneState _new = new ZoneState("New");
    private static ZoneState _active = new ZoneState("Active");
    private static ZoneState _disabled = new ZoneState("Disabled");

    public string Description { get; private set; }

    private ZoneState(string Desc)
    {
        Description = Desc;
    }
}