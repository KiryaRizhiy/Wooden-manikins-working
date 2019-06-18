using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class ProcessManager : MonoBehaviour {

    protected static List<Process> _AllProcesses { get; set; }
    protected static List<ProcessTemplate> _AllTemplates { get; set; }
    protected static Units _Units;
    protected static ushort _MaxProcessID = 0, _MaxProcessTemplateId = 0;
    protected static XmlDocument _ActionsConfig = new XmlDocument();
    //protected static Resources _Resources;
    protected static string scr = "ProcessManager", scrac = "ProcessManagerActionConstructor", scracd = "ProcessManagerActionConstructorDetails", scrs = "ProcessManagerSteps", scrui = "ProcessManagerUI";

    private int MaxValueCount_Crutch = -1;
    private int CurrentProcessId;
    private ZoneController.Zone _Mine;
    private ZoneController.Zone _Storage;
    private ZoneController.Zone _Gathering;

    //Technical
    void Start()
    {
        _AllProcesses = new List<Process>();
        _AllTemplates = new List<ProcessTemplate>();
        SetMU(transform.parent.GetChild(0).gameObject);
        SetAC();
        //SetR(transform.parent.GetChild(2).gameObject.GetComponent<Resources>());
        //Debug.Log(_ZController.gameObject.name);
        StartCoroutine(ProcessInitializeJob());
    }
    private static void SetMU(GameObject _mu)
    {
        _Units = _mu.GetComponent<Units>();
    }
    private static void SetAC()
    {
        _ActionsConfig.Load("C:/GD/You and world/v.0.0.1/First/Configs/objects/Action.xml");
    }
    //private static void SetR(Resources _r)
    //{
    //    _Resources = _r;
    //}
    public class KnowledgeEnumerator
    {
        public ushort KnowledgeLevel;
        public ushort KnowledgeID;
        public KnowledgeEnumerator(ushort ID, ushort Level)
        {
            KnowledgeLevel = Level;
            KnowledgeID = ID;
        }
    }
    internal static XmlNode GetAction(ushort ActionID)
    {
        foreach (XmlNode _n in _ActionsConfig.DocumentElement.ChildNodes)
        {
            if (ushort.Parse(_n.ChildNodes.Item(0).InnerText) == ActionID)
                return _n;
        }
        Log.Warning(scr,"No action with ID " + ActionID + " found");
        return null;
    }

    //UI
    public void ValueChanged(int Value)//Переключалка типов процессов. Основное назначение - подгрузить нужные поля и наполнить их актуальным контентом
    {
        if (Value != MaxValueCount_Crutch)
        {
            CurrentProcessId = Value;
            switch (Value)
            {
                case 0://Production
                    HideAllFields();
                    //ShowFields(_ProductionFields);
                    Links.Interface.ProcessCreationProductionResourceSelect.GetComponent<Dropdown>().options.Clear();
                    foreach (Resource _r in Resources.ResourceLibrary.FindAll(x => x.Details != null))
                    {
                        Log.Notice(scrui, _r); ;
                        Links.Interface.ProcessCreationProductionResourceSelect.options.Add(new Dropdown.OptionData(_r.Name));
                    }
                    Links.Interface.ProcessCreationProductionSubpanel.SetActive(true);
                    //Ниже костыль чтобы Dropdown работал как кнопка
                    MaxValueCount_Crutch = Links.Interface.ProcessCreationProcessTypeSelect.options.Count;
                    Links.Interface.ProcessCreationProcessTypeSelect.options.Add(Links.Interface.ProcessCreationProcessTypeSelect.options[Value]);
                    Links.Interface.ProcessCreationProcessTypeSelect.value = MaxValueCount_Crutch;
                    Links.Interface.ProcessCreationProcessTypeSelect.options.RemoveAt(MaxValueCount_Crutch);
                    MaxValueCount_Crutch = -1;
                    break;
                case 1://Mining
                    HideAllFields();
                    Links.Interface.ProcessCreationMiningMineSelect.options.Clear();
                    Links.Interface.ProcessCreationMiningStoreSelect.options.Clear();
                    foreach (ZoneController.Zone _m in ZoneController.AllMines)
                    {
                        Links.Interface.ProcessCreationMiningMineSelect.options.Add(new Dropdown.OptionData(_m.Name));
                    }
                    foreach (ZoneController.Zone _s in ZoneController.AllStorages)
                    {
                        Links.Interface.ProcessCreationMiningStoreSelect.options.Add(new Dropdown.OptionData(_s.Name));
                    }
                    Links.Interface.ProcessCreationMiningMineSelect.Select();
                    Links.Interface.ProcessCreationMiningStoreSelect.Select();
                    _Mine = ZoneController.AllMines[0];
                    _Storage = ZoneController.AllStorages[0];
                    Links.Interface.ProcessCreationMiningSubpanel.SetActive(true);
                    //Ниже костыль чтобы Dropdown работал как кнопка
                    MaxValueCount_Crutch = Links.Interface.ProcessCreationProcessTypeSelect.options.Count;
                    Links.Interface.ProcessCreationProcessTypeSelect.options.Add(Links.Interface.ProcessCreationProcessTypeSelect.options[Value]);
                    Links.Interface.ProcessCreationProcessTypeSelect.value = MaxValueCount_Crutch;
                    Links.Interface.ProcessCreationProcessTypeSelect.options.RemoveAt(MaxValueCount_Crutch);
                    MaxValueCount_Crutch = -1;
                    //ShowFields(_MiningFields);
                    break;
                //case 2://Hawling
                //    //HideAllFields();
                //    break;
                case 2:
                    HideAllFields();
                    Links.Interface.ProcessCreationGatheringZoneSelect.options.Clear();
                    Links.Interface.ProcessCreationGatheringStoreSelect.options.Clear();
                    foreach (ZoneController.Zone _g in ZoneController.AllGatherings)
                        Links.Interface.ProcessCreationGatheringZoneSelect.options.Add(new Dropdown.OptionData(_g.Name));
                    foreach (ZoneController.Zone _s in ZoneController.AllStorages)
                    {
                        Links.Interface.ProcessCreationGatheringStoreSelect.options.Add(new Dropdown.OptionData(_s.Name));
                    }
                    _Gathering = ZoneController.AllGatherings[0];
                    _Storage = ZoneController.AllStorages[0];
                    Links.Interface.ProcessCreationGatheringSubpanel.SetActive(true);
                    //Ниже костыль чтобы Dropdown работал как кнопка
                    MaxValueCount_Crutch = Links.Interface.ProcessCreationProcessTypeSelect.options.Count;
                    Links.Interface.ProcessCreationProcessTypeSelect.options.Add(Links.Interface.ProcessCreationProcessTypeSelect.options[Value]);
                    Links.Interface.ProcessCreationProcessTypeSelect.value = MaxValueCount_Crutch;
                    Links.Interface.ProcessCreationProcessTypeSelect.options.RemoveAt(MaxValueCount_Crutch);
                    MaxValueCount_Crutch = -1;
                    //ShowFields(_GatheringFields);
                    break;
                default:
                    HideAllFields();
                    Log.Warning(scr, "Trying to enable unknown process submenu with id" + Value);
                    break;
            }
        }
    }
    public void SetMine(int value)
    {
        Log.Notice(scrui,"Selected mine" + Links.Interface.ProcessCreationMiningMineSelect.options[Links.Interface.ProcessCreationMiningMineSelect.value].text);
        _Mine = ZoneController.GetMine(Links.Interface.ProcessCreationMiningMineSelect.options[Links.Interface.ProcessCreationMiningMineSelect.value].text);
    }
    public void SetGatheringZone(int value)
    {
        Log.Notice(scrui, "Selected garthering zone " + Links.Interface.ProcessCreationGatheringZoneSelect.options[Links.Interface.ProcessCreationGatheringZoneSelect.value].text);
        _Gathering = ZoneController.GetGatheringZone(Links.Interface.ProcessCreationGatheringZoneSelect.options[Links.Interface.ProcessCreationGatheringZoneSelect.value].text);
    }
    public void SetMiningStorage(int value)
    {
        Log.Notice(scrui, "Selected storage" + Links.Interface.ProcessCreationMiningStoreSelect.options[Links.Interface.ProcessCreationMiningStoreSelect.value].text);
        _Storage = ZoneController.GetStorage(Links.Interface.ProcessCreationMiningStoreSelect.options[Links.Interface.ProcessCreationMiningStoreSelect.value].text);
    }
    public void SetGatheringStorage(int value)
    {
        Log.Notice(scrui, "Selected storage" + Links.Interface.ProcessCreationGatheringStoreSelect.options[Links.Interface.ProcessCreationGatheringStoreSelect.value].text);
        _Storage = ZoneController.GetStorage(Links.Interface.ProcessCreationGatheringStoreSelect.options[Links.Interface.ProcessCreationGatheringStoreSelect.value].text);
    }
    private void HideAllFields()
    {
        Links.Interface.ProcessCreationProductionSubpanel.SetActive(false);
        Links.Interface.ProcessCreationMiningSubpanel.SetActive(false);
        Links.Interface.ProcessCreationGatheringSubpanel.SetActive(false);
    }
    private void ShowFields(List<GameObject> _F)
    {
        foreach (GameObject _f in _F)
            _f.SetActive(true);
    }
    public void SaveProcess()
    {
        Log.Notice(scr, "Process creation initiated");
        switch (CurrentProcessId)
        {
            case 0:
                // Production action
                string _RequiredResouceName = Links.Interface.ProcessCreationProductionResourceSelect.options[Links.Interface.ProcessCreationProductionResourceSelect.value].text;
                new Production(Resources.ResourceLibrary.Find(x => x.Name == _RequiredResouceName).Type);
                break;
            case 1:
                new Mining(_Mine.Name, _Storage.Name);
                break;
            case 2:
                new Gather(_Gathering.Name, _Storage.Name, int.Parse(Links.Interface.ProcessCreationGatheringLimitInput.text));
                break;
            default:
                Log.Warning(scr, "Trying to create unknown process with unknown type" + Links.Interface.ProcessCreationProcessTypeSelect.value);
                break;
        }
        //transform.parent.gameObject.GetComponent<Player_control_script>().SwitchToMainMenuMode();
        Links.Interface.SwitchToMainMenu();
    }

    //Info storage
    public abstract class ProcessTemplate
    {
        public ushort Id { get; private set; }
        public string Name { get; private set; }
        public string Status {
            get
            {
                return _status;
            }
            protected set
            {
                Log.Notice(scr, "Process template: " + Id + ". " + Name + " status changed form: " + _status + " to: " + value);
                _status = value;
            }
        }

        private string _status;

        public ProcessTemplate(string _name)
        {
            Id = _MaxProcessTemplateId;
            Name = _name;
            _status = "active";
            _MaxProcessTemplateId++;
            Log.Notice(scr, "Process template '" + _name + " #" + Id + "' Created");
            _AllTemplates.Add(this);
        }
        public abstract Process Start();
        public abstract Action ConstructNextAction(ushort Step, int ProcID);
        public virtual bool IsValid()
        {
            return true;
        }
        public void Disable()
        {
            Status = "disabled";
        }
    }
    public class Mining : ProcessTemplate
    {
        public ZoneController.Mine _Mine { get; private set; }
        public ZoneController.Storage _Storage { get; private set; }
        private ZoneController.Storage _ToolStorage;
        private Resource _fossil,_tool;
        public Mining(string _MineName, string _StorageName):base("Mining :" + (_MaxProcessTemplateId).ToString())
        {
            _Mine = ZoneController.GetMine(_MineName);
            _Storage = ZoneController.GetStorage(_StorageName);
            Log.Notice(scr,"Mining process template created. " + "Mining from " + _Mine.Name + ", carrying to " + _Storage.Name);
        }
        public override Process Start()
        {
            return new Process(_Units.FindAWorker(), this);
        }
        public override Action ConstructNextAction(ushort Step,int ProcID)
        {
            switch (Step)
            {
                case 0:
                    _ToolStorage = ZoneController.GetStorage(ResourceTypeClassification.DiggingTool, out _tool);
                    Action _take = new Action(5, _ToolStorage.GetTarget(), ProcID);
                    _take._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_tool.Type, -1, 1));//Забираем ресурс из склада
                    _take._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_tool.Type, 1, 2));//Кладем в рюкзак юнита
                    _take._Effects._TargetSack = _ToolStorage.StorageSack;
                    return _take;
                case 1:
                    GameObject _Target = _Mine.GetTarget();
                    Action _mine = new Action(4,_Target,ProcID);
                    _fossil = _Target.GetComponent<Brick>().Resource;
                    _mine._WorkFlow._LeadTime = _fossil.TimeToMine;
                    _mine._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_fossil.Type, 1, 2));
                    return _mine;
                case 2:
                    Action _haul = new Action(5, _Storage.GetTarget(), ProcID);
                    _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_fossil.Type, -1, 2));
                    _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_fossil.Type, 1, 1));
                    _haul._Effects._TargetSack = _Storage.StorageSack;
                    return _haul;
                case 3:
                    Action _put = new Action(5, _ToolStorage.GetTarget(), ProcID);
                    _put._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_tool.Type, 1, 1));
                    _put._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_tool.Type, -1, 2));
                    _put._Effects._TargetSack = _ToolStorage.StorageSack;
                    return _put;
                default:
                    return null;
            }
        }
        public override bool IsValid()
        {
            Resource ShayMonMeAboutThisCrutch;
            if (_Mine.State != ZoneState.Active || _Storage.State != ZoneState.Active || ZoneController.GetStorage(ResourceTypeClassification.DiggingTool, out ShayMonMeAboutThisCrutch) == null)
            {
                Status = "disabled";
                return false;
            }
            else
                return true;
        }
    }
    public class Production : ProcessTemplate
    {
        private Resource _RequiredResource;
        private ZoneController.Storage _Source;
        private Workbench _wb;
        public Production(ushort ResourceId)
            : base("Production :" + (Resources.ResourceLibrary.Find(x => x.Type == ResourceId).Name))
        {
            _RequiredResource = Resources.ResourceLibrary.Find(x => x.Type == ResourceId);
        }
        public override Process Start()
        {
            _wb = Structures.BuildingList.Find(x => x.HasAWorkbench(_RequiredResource.MachineToCreate)).BWorkbenches.Find(x => x.Type == _RequiredResource.MachineToCreate);
            return new Process(_Units.FindAWorker(), this);
        }
        public override Action ConstructNextAction(ushort Step, int ProcID)
        {
            switch (Step - _RequiredResource.Details.Count * 2)
            {
                case 0:
                    //Производим ресурс
                    Action _Product = new Action(2, _wb.Base, ProcID);
                    _Product._Effects._TargetSack = _wb.Sack;
                    _Product._WorkFlow._LeadTime = _RequiredResource.TimeToCreate;
                    foreach (ResourceDetail _d in _RequiredResource.Details)
                    {
                        _Product._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_d.Type,_d.CountWithMinus,1));
                    }
                    _Product._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Type,1,1));
                    return _Product;
                case 1: //Забираем со станка
                    Action _haul2 = new Action(5, _wb.Base, ProcID);
                    _haul2._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Type, -1, 1));
                    _haul2._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Type, 1,2));
                    _haul2._Effects._TargetSack = _wb.Sack;
                    return _haul2;
                case 2: //Несем на склад
                    _Source = ZoneController.AllStorages.Find(x=>x.StorageSack.HasEnoughSpaceFor(_RequiredResource,1));
                    if (_Source == null)
                        return new Action(0, null, ProcID, "No free storage for " + _RequiredResource.Name + " found");
                    Action _haul3 = new Action(5, _Source.GetTarget(), ProcID);
                    _haul3._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Type, -1,2));
                    _haul3._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Type, 1, 1));
                    _haul3._Effects._TargetSack = _Source.StorageSack;
                    return _haul3;
                case 3:
                    return null;
                default://Приносим нужные ресурсы к станку
                    Action _haul;
                    if ((Step + 1) % 2 == 0)//Если шаг нечетный
                    {
                        //Мы относим к станку что взяли
                        _haul = new Action(5, _wb.Base, ProcID);
                        _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Details[Step / 2].Type, _RequiredResource.Details[Step / 2].CountWithMinus, 2));
                        _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Details[Step / 2].Type,_RequiredResource.Details[Step / 2].Count, 1));
                        _haul._Effects._TargetSack = _wb.Sack;
                    }
                    else // Иначе
                    {
                        //Берем ресурс чтоб отнести к станку
                        _Source = ZoneController.GetStorage(_RequiredResource.Details[Step / 2].Type, _RequiredResource.Details[Step / 2].Count);
                        if (_Source == null)
                        {
                            return new Action(0, null, ProcID, "Storage with " + _RequiredResource.Details[Step / 2].Count + " " +_RequiredResource.Details[Step / 2].Type + "'s not found");
                        }
                        _haul = new Action(5, _Source.GetTarget(), ProcID);
                        _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Details[Step / 2].Type, _RequiredResource.Details[Step / 2].CountWithMinus, 1));
                        _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_RequiredResource.Details[Step / 2].Type, _RequiredResource.Details[Step / 2].Count, 2));
                        _haul._Effects._TargetSack = _Source.StorageSack;
                    }
                    return _haul;
            }
        }//По хорошему надо б научиться юзать hauling
    }
    public class Building : ProcessTemplate
    {
        private Mining _DiggingBuildingPit;//Подпроцесс откапывания блоков, мешающих зданию
        private Hauling _HaulingResouces;//Подпроцесс перетаскивания ресурсов для строительства
        private PhantomConstruction _BuidlingTemplate; //Шаблон строящегося здания
        private Structure _Structure; //Экземпляр строящегося здания
        private Resource _CurrentBrickResource;
        private Vector3 _CurrentBrickCoordinates;
        private bool _WorkbenchesExists, _BasicElementsBuilded = false;
        private GameObject _target,_parent;
        private Workbench _CurrentWorkbench;
        private List<GameObject>.Enumerator _BuildingElementsEnumerator, _WorkbenchElementEnumerator;
        private List<PhantomConstruction>.Enumerator _WorkbenchesEnumerator;

        public Building(PhantomConstruction BuidlingPhantom)
            : base("Building : " + BuidlingPhantom.MasterObject.name)
        {
            BuidlingPhantom.Hide();
            _Structure = new Structure(_BuidlingTemplate);
            _Structure.State = StructureState.Building;
            ZoneController.Mine _Pit = new ZoneController.Mine(BuidlingPhantom);
            ZoneController.Storage _Storage = ZoneController.AllStorages[0];
            _DiggingBuildingPit = new Mining(_Pit.Name, _Storage.Name);
            _AllProcesses.Add(_DiggingBuildingPit.Start());
            //Status = "paused";
            _BuidlingTemplate = BuidlingPhantom;
            _BuidlingTemplate.MasterObject.transform.position -= Vector3.up * 0.05f;
            _BuildingElementsEnumerator = _BuidlingTemplate.Elements.GetEnumerator();
            _WorkbenchesEnumerator = _BuidlingTemplate.ChildConstructions.GetEnumerator();
            _WorkbenchesExists = _WorkbenchesEnumerator.MoveNext();
            if (_WorkbenchesExists)
            {
                _WorkbenchElementEnumerator = _WorkbenchesEnumerator.Current.Elements.GetEnumerator();
                _CurrentWorkbench = new Workbench(_WorkbenchesEnumerator.Current,_Structure);
            }
        }
        public override Process Start()
        {
            return new Process(_Units.FindAWorker(),this);
        }
        public override Action ConstructNextAction(ushort Step, int ProcID)
        {            
            //Выбираем объект, который надо построить
            if (!_BasicElementsBuilded)
                if (_BuildingElementsEnumerator.MoveNext())
                {
                    Log.Notice(scrac, "Phantom building data: Name - " + _BuidlingTemplate.MasterObject.name);
                    Log.Notice(scrac, "Elements count - " + _BuidlingTemplate.Elements.Count);
                    Log.Notice(scrac, "Current block data: Name - " + _BuildingElementsEnumerator.Current.name + ", Coordinates - " + _BuildingElementsEnumerator.Current.transform.position + ", order - " + _BuidlingTemplate.Elements.IndexOf(_BuidlingTemplate.Elements.GetEnumerator().Current));
                    _target = _BuildingElementsEnumerator.Current;
                    _parent = _Structure.MasterObject;
                    while (_target.transform.childCount > 0 && !_BasicElementsBuilded)//Если у блока есть дочерние, то перебираем блоки, пока не наткнемся на какой нибудь без дочерних или пока не доберемся до конца списка
                    {
                        _BasicElementsBuilded = _BuildingElementsEnumerator.MoveNext();
                        if (!_BasicElementsBuilded) _target = _BuildingElementsEnumerator.Current;
                    }
                }
                else
                    _BasicElementsBuilded = true;

            if (_BasicElementsBuilded && _WorkbenchesExists)//Если основные кирпичи уже построили и еще существуют станки
            {
                if (_WorkbenchElementEnumerator.MoveNext())//Если станок еще не достроен - целью считается его элемент
                {
                    _target = _WorkbenchElementEnumerator.Current;
                    _parent = _WorkbenchesEnumerator.Current.MasterObject;
                }
                else
                {
                    if (_WorkbenchesEnumerator.MoveNext())//Если станок достроен, но еще нужно достроить станки
                    {
                        _WorkbenchElementEnumerator = _WorkbenchesEnumerator.Current.Elements.GetEnumerator();//Переключаем перечислитель на элементы текущего станка
                        _CurrentWorkbench = new Workbench(_WorkbenchesEnumerator.Current, _Structure);
                    }
                    else
                    {
                        Log.Notice(scrac, "Finishing building process, destroying phantom construction with workbenches");
                        _BuidlingTemplate.Destroy();
                        this.Status = "disabled";
                        _Structure.State = StructureState.Active;
                        return null;//Заканчиваем процесс, все построили
                    }
                }
            }

            if (_BasicElementsBuilded && !_WorkbenchesExists)// Если кирпичи достроили и станков нет
            {
                Log.Notice(scrac, "Finishing building process, destroying phantom construction");
                _BuidlingTemplate.Destroy();
                this.Status = "disabled";
                _Structure.State = StructureState.Active;
                return null;//Заканчиваем процесс, все построили
            }

            //Сохраняем данные объекта для создания действия
            _CurrentBrickResource = Resources.ResourceLibrary.Find(x => x.Name == _target.name);
            Log.Notice(scrac, "Resource of current block: " + _CurrentBrickResource.Name);
            _CurrentBrickCoordinates = _target.transform.position;

            //Создаем действие на постройку
            Action _build = new Action(6, _target, ProcID);
            _build._Effects._NewBlockResourceType = _CurrentBrickResource;
            _build._Effects._NewBlockCoordinates = _CurrentBrickCoordinates;
            _build._WorkFlow._LeadTime = _CurrentBrickResource.TimeToBuild;
            _build._Effects._TargetSack = _HaulingResouces._Target.StorageSack;
            _build._Effects._NewBlockParent = _parent;
            return _build;
        }
        public override bool IsValid()
        {
            if (_DiggingBuildingPit.Status == "active") // Яму еще не выкопали
                return false;
            else
            {
                if (_HaulingResouces == null)//Ресурсы еще не начали приносить
                {
                    _BuidlingTemplate.MasterObject.transform.position += Vector3.down;
                    ZoneController.Storage _PitStorage = new ZoneController.Storage(_BuidlingTemplate);
                    _BuidlingTemplate.MasterObject.transform.position += Vector3.up;
                    List<ResourceEnumerator> _AllBricks = new List<ResourceEnumerator>();
                    _AllBricks = Structures.GetRequiredresources(_BuidlingTemplate.MasterObject.name).ResourceRequirements;
                    _HaulingResouces = new Hauling(null, _PitStorage, _AllBricks);
                    Log.Notice(scr, Structures.GetRequiredresources(_BuidlingTemplate.MasterObject.name));
                    return false;
                }
                else
                {
                    if (_HaulingResouces.Status == "active")//Ресурсы еще не принесли
                        return false;
                    else
                    {
                        if (_HaulingResouces._Target.State == ZoneState.Active)
                            _HaulingResouces._Target.Disable();
                        return true;
                    }
                }
            }
        }
    }
    public class Hauling : ProcessTemplate
    {
        public ZoneController.Storage _Source { get; private set; }
        public ZoneController.Storage _Target { get; private set; }
        public List<ResourceEnumerator> _wishlist;
        private ResourceEnumerator _CurrentResource;
        private Action _haul;
        public Hauling(ZoneController.Storage Source = null, ZoneController.Storage Target = null, List<ResourceEnumerator> Resources = null)
            : base("Hauling")
        {
            _Source = Source;
            _Target = Target;
            _wishlist = Resources;
        }
        public override Process Start()
        {
            return new Process(_Units.FindAWorker(), this);
        }
        public override Action ConstructNextAction(ushort Step, int ProcID)
        {
            switch (Step)
            {
                case 0://Забрать ресурс
                    _CurrentResource = _wishlist.Find(x => !_Target.StorageSack.HasAResource(x.Resource, x.Count));//Находим в вишлисте ресурс, которого еще нет на целевом складе
                    ZoneController.Storage _localsoruce;
                    if (_Source == null)
                    {
                        _localsoruce = ZoneController.GetStorage(_CurrentResource.Resource.Type, _CurrentResource.Count);
                        if (_localsoruce == null)
                            return new Action(0, null, ProcID, "Storage with " + _CurrentResource.Count + " " + _CurrentResource.Resource.Name + "'s not found");
                    }
                    else
                        _localsoruce = _Source;
                    _haul = new Action(5, _localsoruce.GetTarget(), ProcID);
                    _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_CurrentResource.Resource.Type, _CurrentResource.MinusCount, 1));
                    _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_CurrentResource.Resource.Type, _CurrentResource.Count, 2));
                    _haul._Effects._TargetSack = _localsoruce.StorageSack;
                    return _haul;
                case 1://Принести ресурс
                    ZoneController.Storage _localtarget;
                    if (_Target == null)
                    {
                        _localtarget = ZoneController.AllStorages.Find(x => x.StorageSack.HasEnoughSpaceFor(_CurrentResource.Resource, _CurrentResource.Count));
                        if (_localtarget == null)
                            return new Action(0, null, ProcID, "Storage with enough space for " + _CurrentResource.Count + " " + _CurrentResource.Resource.Name + "'s not found");
                    }
                    else
                        _localtarget = _Target;
                    _haul = new Action(5, _localtarget.GetTarget(), ProcID);
                    _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_CurrentResource.Resource.Type, _CurrentResource.MinusCount, 2));
                    _haul._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_CurrentResource.Resource.Type, _CurrentResource.Count, 1));
                    _haul._Effects._TargetSack = _localtarget.StorageSack;
                    return _haul;
                default:// Сбрасываем параметры до дефолтов
                    _CurrentResource = null;
                    _haul = null;
                    return _haul;
            }
        }
        public override bool IsValid()
        {
            if (_wishlist.Find(x => !_Target.StorageSack.HasAResource(x.Resource, x.Count)) == null)
            {
                Status = "disabled";
                return false;
            }
            else return true;
        }
    }
    public class Gather : ProcessTemplate
    {
        public ZoneController.Gathering _gatherZone { get; private set; }
        public ZoneController.Storage _storage { get; private set; }
        private List<ResourceEnumerator> _gatheredResources;
        public int _limit { get; private set; }
        public Gather(string GatherZoneName, string StorageName, int Limit):base("Gathering")
        {
            _gatherZone = ZoneController.GetGatheringZone(GatherZoneName);
            _storage = ZoneController.GetStorage(StorageName);
            _limit = Limit;
            Log.Notice(scr, "Gathering process template created. " + "Gathering from " + _gatherZone.Name + ", carrying to " + _storage.Name + ", garthering limit is: " + _limit);
        }
        public override Process Start()
        {
            return new Process(_Units.FindAWorker(), this);
        }
        public override Action ConstructNextAction(ushort Step, int ProcID)
        {
            switch (Step)
            {
                case 0:
                    Action _g = new Action(1,_gatherZone.GetTarget(),ProcID);
                    _gatheredResources = _g._Effects._ResourcesToCreate;
                    return _g ;
                case 1:
                    Action _h = new Action(5, _storage.GetTarget(), ProcID);
                    foreach (ResourceEnumerator _r in _gatheredResources)
                    {
                        _h._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_r.Resource.Type, _r.MinusCount, 2));//Забрать ресурс у актера
                        _h._Effects._ResourcesToCreate.Add(new ResourceEnumerator(_r.Resource.Type, _r.Count, 1));//Сложить на склад
                        _h._Effects._TargetSack = _storage.StorageSack;
                    }
                    return _h;
                default:
                    return null;
            }
        }
        public override bool IsValid()
        {
            //Debug.Log("Gathering zone has goods: " + _gatherZone.IsEmpty() + ", and storage is not full: " + !_storage.StorageSack.HasAResource(Resources.GetResource(7), _limit));
            return (!_gatherZone.IsEmpty())&&(!_storage.StorageSack.HasAResource(Resources.GetResource(7),_limit));//Шаблон процесса валиден, если зона сбора не пуста и предел сбора ресурсов не достигнут
        }
    }
    
    //Processing
    public class Action
    {
        public int ProcessID {get; private set;}
        public ushort ID { get; private set; }
        public string Name { get; private set; }
        public GameObject _Target;

        public Conditions _Conditions;
        public Workflow _WorkFlow;
        public Effects _Effects;

        /// <summary>
        /// Object, that shows the unit instructions to act. Some special actions may brake process. Null action - state process break.
        /// </summary>
        /// <param name="ActionID">0-Special action to break the process</param>
        /// <param name="Target"></param>
        /// <param name="ProcID"></param>
        /// <param name="ErrMsg">Message, thrown to console when breaking the process with action 0</param>
        public Action(ushort ActionID, GameObject Target = null, int ProcID = 0, string ErrMsg = "Unknown error")
        {
            if (ActionID != 0)
            {
                Log.Notice(scrac, "Process #" + ProcID + " constructs new action with Id: " + ActionID + ". Target is " + Target.name);
                ProcessID = ProcID;
                ID = ushort.Parse(GetAction(ActionID).ChildNodes.Item(0).InnerText);
                Name = GetAction(ActionID).ChildNodes.Item(1).InnerText;
                _Conditions = new Conditions(ActionID);
                _WorkFlow = new Workflow(ActionID);
                _Effects = new Effects(ActionID);
                _Target = Target;
            }
            else
            {
                Name = ErrMsg;
            }
        }
        public class Workflow
        {
            public float _LeadTime = 0,SecondsToDo;
            public int _InfluenceStrength = 0, _InfluenceAgility = 0, _InfluenceIntellect = 0, _InfluenceWisdom = 0;
            public bool _SetTargetBusy = false, _SetActorBusy = false;
            public string _ActorGraphics, _TargetGraphics, _ActorMethod, _ActorMethodComponent, _TargetMethod, _TargetMethodComponent;
            public Workflow(ushort _ActionID)
            {
                Log.Notice(scrac,"Workflow for action " + _ActionID + " construction");
                XmlNodeList _Workflow = GetAction(_ActionID).ChildNodes.Item(3).ChildNodes;
                if (Functions.IsNodeNotEmpty(_Workflow[0],scracd))
                {
                    _LeadTime = float.Parse(_Workflow[0].ChildNodes.Item(0).InnerText);
                }
                if (Functions.IsNodeNotEmpty(_Workflow[0].ChildNodes.Item(1), scracd))
                {
                    if (Functions.IsNodeNotEmpty(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(0), scracd))
                        _InfluenceStrength = int.Parse(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(0).InnerText);
                    if (Functions.IsNodeNotEmpty(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(1), scracd))
                        _InfluenceAgility = int.Parse(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(1).InnerText);
                    if (Functions.IsNodeNotEmpty(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(2), scracd))
                        _InfluenceIntellect = int.Parse(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(2).InnerText);
                    if (Functions.IsNodeNotEmpty(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(3), scracd))
                        _InfluenceWisdom = int.Parse(_Workflow[0].ChildNodes.Item(1).ChildNodes.Item(3).InnerText);
                }
                if (!Functions.IsNodeNotEmpty(_Workflow[1].ChildNodes.Item(0), scracd))
                    _SetActorBusy = true;
                if (!Functions.IsNodeNotEmpty(_Workflow[1].ChildNodes.Item(1), scracd))
                    _SetTargetBusy = true;
                if (Functions.IsNodeNotEmpty(_Workflow[2].ChildNodes.Item(0), scracd))
                    _ActorGraphics = _Workflow[2].ChildNodes.Item(0).InnerText;
                if (Functions.IsNodeNotEmpty(_Workflow[2].ChildNodes.Item(1), scracd))
                    _TargetGraphics = _Workflow[2].ChildNodes.Item(1).InnerText;
                if (Functions.IsNodeNotEmpty(_Workflow[3].ChildNodes.Item(0), scracd))
                {
                    _ActorMethodComponent = _Workflow[3].ChildNodes[0].ChildNodes[0].InnerText;
                    _ActorMethod = _Workflow[3].ChildNodes[0].ChildNodes[1].InnerText;
                }
                if (Functions.IsNodeNotEmpty(_Workflow[3].ChildNodes.Item(1), scracd))
                {
                    _TargetMethodComponent = _Workflow[3].ChildNodes[1].ChildNodes[0].InnerText;
                    _TargetMethod = _Workflow[3].ChildNodes[1].ChildNodes[1].InnerText;
                }
                Log.Notice(scrac,"Workflow for action " + _ActionID + " constructed");
            }
        }
        public class Conditions
        {
            public float _MinDistanceToTarget, _MaxDistanceToTarget;
            //public List<ResourceEnumerator> _TargetHasAResource, _ActorHasAResource;
            public byte _TargetBusyness, _ActorBusyness; //<!--2 = must be free, 1 = Must be busy,0 = Anyway--> 
            public List<KnowledgeEnumerator> _Knowledges;
            public List<string> _ReqiuredTargets;
            public ResourceTypeClassification RequiredTool = ResourceTypeClassification.Resource;
            public Conditions(ushort _ActionID)
            {
                //_TargetHasAResource = new List<ResourceEnumerator>();
                //_ActorHasAResource = new List<ResourceEnumerator>();
                _Knowledges = new List<KnowledgeEnumerator>();
                _ReqiuredTargets = new List<string>();
                Log.Notice(scrac, "Conditions for action " + _ActionID + " construction");
                XmlNodeList _Conditions = GetAction(_ActionID).ChildNodes.Item(2).ChildNodes;
                if (Functions.IsNodeNotEmpty(_Conditions.Item(3), scracd))
                {
                    foreach (XmlNode _n in _Conditions.Item(3).ChildNodes)
                        if (_n.InnerText == "1")
                            _ReqiuredTargets.Add(_n.Name);
                }
                if (Functions.IsNodeNotEmpty(_Conditions.Item(0), scracd))
                {
                    if (Functions.IsNodeNotEmpty(_Conditions.Item(0).ChildNodes.Item(0), scracd))
                        _MinDistanceToTarget = float.Parse(_Conditions.Item(0).ChildNodes.Item(0).InnerText);
                    else
                        _MinDistanceToTarget = 0;
                    if (Functions.IsNodeNotEmpty(_Conditions.Item(0).ChildNodes.Item(1), scracd))
                        _MaxDistanceToTarget = float.Parse(_Conditions.Item(0).ChildNodes.Item(1).InnerText);
                    else
                        _MaxDistanceToTarget = float.MaxValue;
                }
                //if (Functions.IsNodeNotEmpty(_Conditions.Item(1)))
                //{
                //    foreach (XmlNode _n in _Conditions.Item(1).ChildNodes)
                //    {
                //        if (_n.ChildNodes.Item(1).InnerText == null)
                //            _TargetHasAResource.Add(new ResourceEnumerator(ushort.Parse(_n.ChildNodes.Item(0).InnerText), 1));
                //        else
                //            _TargetHasAResource.Add(new ResourceEnumerator(ushort.Parse(_n.ChildNodes.Item(0).InnerText), ushort.Parse(_n.ChildNodes.Item(1).InnerText)));
                //    }
                //}
                //if (Functions.IsNodeNotEmpty(_Conditions.Item(2)))
                //{
                //    foreach (XmlNode _n in _Conditions.Item(2).ChildNodes)
                //    {
                //        if (_n.ChildNodes.Item(2).InnerText == null)
                //            _ActorHasAResource.Add(new ResourceEnumerator(ushort.Parse(_n.ChildNodes.Item(0).InnerText), 1));
                //        else
                //            _ActorHasAResource.Add(new ResourceEnumerator(ushort.Parse(_n.ChildNodes.Item(0).InnerText), ushort.Parse(_n.ChildNodes.Item(1).InnerText)));
                //    }
                //}

                if (Functions.IsNodeNotEmpty(_Conditions.Item(1).ChildNodes.Item(0), scracd))
                    _TargetBusyness = byte.Parse(_Conditions.Item(1).ChildNodes.Item(0).InnerText);
                else
                    _TargetBusyness = 2;
                if (Functions.IsNodeNotEmpty(_Conditions.Item(1).ChildNodes.Item(1), scracd))
                    _ActorBusyness = byte.Parse(_Conditions.Item(1).ChildNodes.Item(1).InnerText);
                else
                    _ActorBusyness = 2;
                if (Functions.IsNodeNotEmpty(_Conditions.Item(2), scracd))
                    foreach (XmlNode _n in _Conditions.Item(2).ChildNodes)
                        _Knowledges.Add(new KnowledgeEnumerator(ushort.Parse(_Conditions.Item(2).ChildNodes.Item(0).InnerText), ushort.Parse(_Conditions.Item(2).ChildNodes.Item(1).InnerText)));
                if (Functions.IsNodeNotEmpty(_Conditions[4], scracd))
                    RequiredTool = (ResourceTypeClassification)int.Parse(_Conditions[4].InnerText);
                Log.Notice(scrac, "Conditions for action " + _ActionID + " constructed");
            }
        }
        public class Effects
        {
            public ushort _SymptomForActor, _SymptomForTarget;
            public byte _ActorCharsImproveStrength = 0, _ActorCharsImproveAgility = 0, _ActorCharsImproveIntellect = 0, _ActorCharsImproveWisdom = 0;
            public byte _TargetCharsImproveStrength = 0, _TargetCharsImproveAgility = 0, _TargetCharsImproveIntellect = 0, _TargetCharsImproveWisdom = 0;
            public bool _RemoveTarget = false;
            public Sack _TargetSack;
            public List<ResourceEnumerator> _ResourcesToCreate = new List<ResourceEnumerator>();
            public Resource _NewBlockResourceType;
            public Vector3 _NewBlockCoordinates;
            public GameObject _NewBlockParent;
            public Effects(ushort _ActionID)
            {
                Log.Notice(scrac, "Effects for " + _ActionID + " under construction");
                XmlNodeList _Effects = GetAction(_ActionID).ChildNodes.Item(4).ChildNodes;
                if (Functions.IsNodeNotEmpty(_Effects.Item(0), scracd))
                {
                    if (Functions.IsNodeNotEmpty(_Effects.Item(0).ChildNodes.Item(0), scracd))
                        _SymptomForActor = ushort.Parse(_Effects.Item(0).ChildNodes.Item(0).InnerText);
                    if (Functions.IsNodeNotEmpty(_Effects.Item(0).ChildNodes.Item(1), scracd))
                        _SymptomForTarget = ushort.Parse(_Effects.Item(0).ChildNodes.Item(1).InnerText);
                }
                if (Functions.IsNodeNotEmpty(_Effects.Item(1),scracd))
                {
                    if (Functions.IsNodeNotEmpty(_Effects.Item(1).ChildNodes.Item(0),scracd))
                    {
                        XmlNodeList _ActorCharsInf = _Effects.Item(1).ChildNodes.Item(0).ChildNodes;
                        if (Functions.IsNodeNotEmpty(_ActorCharsInf[0],scracd))
                            _ActorCharsImproveStrength = byte.Parse(_ActorCharsInf[0].InnerText);
                        if (Functions.IsNodeNotEmpty(_ActorCharsInf[1],scracd))
                            _ActorCharsImproveAgility = byte.Parse(_ActorCharsInf[1].InnerText);
                        if (Functions.IsNodeNotEmpty(_ActorCharsInf[2],scracd))
                            _ActorCharsImproveIntellect = byte.Parse(_ActorCharsInf[2].InnerText);
                        if (Functions.IsNodeNotEmpty(_ActorCharsInf[3],scracd))
                            _ActorCharsImproveWisdom = byte.Parse(_ActorCharsInf[3].InnerText);
                    }
                    if (Functions.IsNodeNotEmpty(_Effects.Item(1).ChildNodes.Item(1),scracd))
                    {
                        XmlNodeList _TargetCharsInf = _Effects.Item(1).ChildNodes.Item(1).ChildNodes;
                        if (Functions.IsNodeNotEmpty(_TargetCharsInf[0],scracd))
                            _TargetCharsImproveStrength = byte.Parse(_TargetCharsInf[0].InnerText);
                        if (Functions.IsNodeNotEmpty(_TargetCharsInf[1],scracd))
                            _TargetCharsImproveAgility = byte.Parse(_TargetCharsInf[1].InnerText);
                        if (Functions.IsNodeNotEmpty(_TargetCharsInf[2],scracd))
                            _TargetCharsImproveIntellect = byte.Parse(_TargetCharsInf[2].InnerText);
                        if (Functions.IsNodeNotEmpty(_TargetCharsInf[3],scracd))
                            _TargetCharsImproveWisdom = byte.Parse(_TargetCharsInf[3].InnerText);
                    }
                }
                if (Functions.IsNodeNotEmpty(_Effects.Item(2),scracd))
                    _RemoveTarget = true;
                if (Functions.IsNodeNotEmpty(_Effects.Item(3),scracd))
                {
                    // put into <!--Default,2 Actor, 1 Target,0 Landskape-->
                    foreach(XmlNode _rs in _Effects.Item(3).ChildNodes)
                        if (Functions.IsNodeNotEmpty(_rs.ChildNodes.Item(2),scracd))
                            _ResourcesToCreate.Add(new ResourceEnumerator(ushort.Parse(_rs.ChildNodes.Item(0).InnerText), short.Parse(_rs.ChildNodes.Item(1).InnerText), byte.Parse(_rs.ChildNodes.Item(2).InnerText)));
                        else
                            _ResourcesToCreate.Add(new ResourceEnumerator(ushort.Parse(_rs.ChildNodes.Item(0).InnerText), short.Parse(_rs.ChildNodes.Item(1).InnerText), 2));
                }
                Log.Notice(scrac, "Effects for action " + _ActionID + " constructed");
            }
        }
    }
    public class Process
    {
        private GameObject _Actor;
        public int ProcessID { get; private set; }
        public string Status { get; private set; }
        public ushort Step { get; private set; }
        private ProcessTemplate _Template;
        public Process(GameObject Actor, ProcessTemplate Template)
        {
            Log.Notice(scr,"Initialize process " + _MaxProcessID + " of template " + Template.Name);
            _Template = Template;
            ProcessID = _MaxProcessID;
            _MaxProcessID++;
            _Actor = Actor;
            Step = 0;
            Status = "active";
            this.DoNextStep();
        }
        public void DoNextStep()
        {
            Action _NextAct = _Template.ConstructNextAction(Step, ProcessID);
            if (_NextAct == null)
            {
                Log.Notice(scr, "Process: " + _Template.Name + " : " + ProcessID + " done");
                Status = "disabled";
                _Actor.GetComponent<Unit>().ProcessDone();
                return;
            }
            if (_NextAct.ID == 0)
            {
                Log.Warning(scr, "Process: " + _Template.Name + " : " + ProcessID + " breaked. Reason:" + _NextAct.Name);
                _Actor.GetComponent<Unit>().ProcessDone();
                Status = "disabled";
                return;
            }
            Log.Notice(scrs, "Process: " + _Template.Name + " : " + ProcessID + " step " + Step + " " + _NextAct.Name + " sent for processing");
            Status = "processing";
            _Actor.GetComponent<Unit>().InitiateAction(_NextAct);
            Step++;
        }
    }
    protected ProcessTemplate FindTemplate(ushort ID)
    {
        return _AllTemplates.Find(x => x.Id == ID);
    }
    public void StepDone(int ProcessID)
    {
        Log.Notice(scrs,"Process ID: " + ProcessID + " step done");
        _AllProcesses.Find(x => x.ProcessID == ProcessID).DoNextStep();
    }
    private IEnumerator ProcessInitializeJob()
    {
        while (true)
        {
            if (_AllTemplates.FindAll(x=>x.Status == "active").Count > 0)
                foreach (ProcessTemplate PT in _AllTemplates.FindAll(x=>x.Status == "active"))
                {
                    if (PT.IsValid())
                    {
                        byte i = 0;
                        while (i < 3)
                        {
                            if (_Units.FindAWorker() != null)
                                _AllProcesses.Add(PT.Start());
                            else
                                break;
                            i++;
                        }
                    }
                    else
                        Log.Notice(scr,"Process template " + PT.Name + " #" + PT.Id + " is invalid");
                }
            //else
            //    Debug.Log("Still no processes yet");
            yield return new WaitForSeconds(5f);
        }
    }
}
public class ResourceEnumerator
{
    public short Count;
    public Resource Resource;
    public short MinusCount { get { return (short)-Count; } }
    public byte PutIntoPointer; //<!--2 Actor, 1 Target,0 Landskape-->
    public ResourceEnumerator(ushort type, short count)
    {
        Count = count;
        Resource = Resources.GetResource(type);
        //Debug.Log("Resource added to enumerator");
        //Resource.Log();
    }
    /// <summary>
    /// Конструктор перечеслителя ресурсов с указанием "Куда сложить ресурс"
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <param name="put">2- Актер, 1 - Цель, 0 - Земля</param>
    public ResourceEnumerator(ushort type, short count, byte put)
    {
        Count = count;
        Resource = Resources.GetResource(type);
        //Debug.Log("Resource added to enumerator");
        //Resource.Log();
        PutIntoPointer = put;
    }
}