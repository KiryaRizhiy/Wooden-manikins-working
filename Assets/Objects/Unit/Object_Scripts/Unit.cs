using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

public class Unit: MonoBehaviour
{
    // Технические характеристики
    private string scr = "Unit", scra = "UnitActions",scrad = "UnitActionsDetails", scrp = "UnitParameters";

    public int CurrentProcessID
    {
        get
        { return _CurrProcID; }
        private set
        {
            Log.Notice(scrp, "CurrentProcessID changed to " + value,gameObject);
            _CurrProcID = value;
        }
    }
    private int _CurrProcID;

    // Характеристики состояния:
    private byte _Hunger, _Sleepy, _Tried, _Hygiene, _Mood, _Age;
    // Характеристики навыков:
    public List<Skill> _Skills { get; private set; }// для оптимизации можно будет убрать Vector3 и напилить свой класс, но это потом
    // Каждый элемент списка _Skills состоит из 3-х цифр. 1-я цифра - ID действия, 2-я цифра - текущий уровень навыка, 3-я цифра - общий уровень навыка

    // Характеристики качеств:
    public byte _Strength = 1, _Agility = 1, _Intellect = 1, _Wisdom = 1; 

    // Характеристики здоровья:
    private byte _HitPoints, _EndurancePoints;
    private List<byte> _Symptoms = new List<byte>();
    // в списке _Symptoms лежат ID симптомов юнита

    // Характеристики знаний
    private List<Knowledge> _Knowledges = new List<Knowledge>(); // для оптимизации можно будет убрать Vector3 и напилить свой класс, но это потом
    // Каждый элемент списка _Knowledges состоит из 2-х цифр. ID знания и уровень обладания им

    // Характеристики общие
    private string _Name;
    public Sack Sack { get; private set; }
    private byte _Soul, _Goverment_ID, _Unit_type;
    public bool Busy
    {
        get
        { return _Busy; }
        private set
        {
            Log.Notice(scrp, "Unit busyness changed to:" + value,gameObject);
            _Busy = value;
        }
    }
    private Resource Tool
    {
        get
        {
            return _tool;
        }
        set
        {
            if (value == null)
            {
                _tool = null;
                if (_toolObject != null)
                    Destroy(_toolObject);
            }
            else
            {
                Transform _rightHandTransform = transform.GetChild(0).GetChild(1).GetChild(0);
                GameObject _t;
                switch (value.Classification)
                {
                    case ResourceTypeClassification.CraftingTool:
                        _t = Instantiate(Settings.CraftToolPrefab);
                        break;
                    case ResourceTypeClassification.DiggingTool:
                        _t = Instantiate(Settings.DigToolPrefab);
                        _t.transform.SetParent(_rightHandTransform, false);
                        _t.transform.localPosition = new Vector3(0, 0, -3.4f);
                        _t.transform.rotation = Quaternion.Euler(new Vector3(140,90,180));
                        _t.transform.localScale = new Vector3(18.40679f, 10.2409f, 18.40679f);
                        break;
                    case ResourceTypeClassification.PunchingTool:
                        _t = Instantiate(Settings.PunchToolPrefab);
                        break;
                    default:
                        _t = Instantiate(Settings.BrickPrefab);
                        Log.Warning(scr, "Unit " + gameObject.name + " is trying to take unknown tool type " + value.Classification);
                        break;
                }
                _toolObject = _t;
                //_toolObject.transform.SetParent(_rightHandTransform, false);
                //_toolObject.transform.localPosition = new Vector3(0, 0, -3.4f);
                //_toolObject.transform.localScale = new Vector3(18.40679f, 10.2409f, 18.40679);
                _tool = value;
            }
        }
    }
    private Resource _tool;
    private GameObject _toolObject;
    private bool _Busy;
    private string[] _ReasonsList = { "Checking passed successfully", "", "Target required", "Invalid target type for this action", "Target is too close", "Target is too far", "Target has no sack", "Target has no suitable resources", "Actor has no sack", "Actor has no suitable resources", "Targets sack has not enough plaсe to complete the action", "Actors sack has not enough place to complete the action", "Cant check target's buisiness. Target is not a Unit", "Target is busy", "target is free", "Actor is busy", "Actor is free", "Actor has unenough knowledges", "Actor has no tool", "Actor has no suitable tool", "Unknown mistake" };
    // Любимые действия для отдыха пока не заводил. Заведу на более поздних этапах

    void Start()
    {
        _Skills = new List<Skill>();
        Sack = new Sack(gameObject);
        CurrentProcessID = -1;
    }
    public void SkillImprove(ushort _ActionID)
    {
        if (_Skills.Find(x => x._ActionID.Equals(_ActionID)) != null)
            _Skills.Find(x => x._ActionID.Equals(_ActionID)).ActionDone();
        else
            _Skills.Add(new Skill(_ActionID));
    }
    public bool HasSuchKnowledges(List<int> _Types, List<int> _Levels)
    {
        bool _CheckNotPassed;
        for (int i = 0; i < _Types.Count; i++)
        {
            _CheckNotPassed = true;
            foreach (Knowledge _k in _Knowledges)
            {
                if ((_Types[i] == _k._KnowledgeID) && (_Levels[i] <= _k._KnowledgeLevel))
                    _CheckNotPassed = false;
            }
            if (_CheckNotPassed)
                return false;
        }
        return true;
    }
    public bool HasSuchKnowledges(ushort Type, ushort Level)
    {
        foreach(Knowledge _k in _Knowledges)
            if (_k._KnowledgeID == Type && _k._KnowledgeLevel == Level)
                return true;
        return false;
    }

    public void TakeTool(Resource NewTool)
    {
        Tool = NewTool;
    }
    public void InitiateAction(ProcessManager.Action Act)
    {
        CurrentProcessID = Act.ProcessID;
        StartCoroutine(DoTheAction(Act));
    }
    public IEnumerator DoTheAction(ProcessManager.Action Act)
    {
        Log.Notice(scra, transform.name + " is doing action of process " + Act.ProcessID,gameObject);
        ushort _possibilitycode;
        _possibilitycode = CheckPossibility(Act);
        if (Act._WorkFlow._SetActorBusy)
            Busy = true;
        switch (_possibilitycode)
        {
            case 0:
                Log.Notice(scra,_possibilitycode + ". " + _ReasonsList[_possibilitycode]);
                ActivateWorkflow(Act);
                break;
            case 4:
            case 5:
                Log.Notice(scra, _possibilitycode + ". " + _ReasonsList[_possibilitycode] + ". Trying to fix.");
                GetComponent<RouteBuilder>().BuildRange(Act._Target.transform.position, Act._Conditions._MaxDistanceToTarget, Act._Conditions._MinDistanceToTarget);
                yield return new WaitWhile(() => GetComponent<RouteBuilder>()._IsWalking);
                Log.Notice(scra, "Action sent for re-processing");
                Busy = false;
                InitiateAction(Act);
                break;
            default:
                Log.Warning(scra, _possibilitycode + ". " + _ReasonsList[_possibilitycode]);
                Busy = false;
                ProcessDone();
                break;
        }
    }
    private ushort CheckPossibility(ProcessManager.Action Act)
    {
        Log.Notice(scra, "Action possibility checking started");
        bool _CheckPassed = false;

        //Проверка на наличие нужной цели
        if ((Act._Target == null) && (Act._Conditions._ReqiuredTargets.Count > 0))
            return 2;
        if (Act._Conditions._ReqiuredTargets.Count > 0)
        {
            foreach (string _s in Act._Conditions._ReqiuredTargets)
                if (_s == Act._Target.tag)
                    _CheckPassed = true;
            if (!_CheckPassed)
                return 3;
        }

        // Проверка на расстояние до цели
        if (Act._Conditions._ReqiuredTargets.Count > 0)
        {
            if (Vector3.Distance(GetComponent<Transform>().position, Act._Target.transform.position) < Act._Conditions._MinDistanceToTarget)
                return 4;
            if (Vector3.Distance(GetComponent<Transform>().position, Act._Target.transform.position) > Act._Conditions._MaxDistanceToTarget)
                return 5;
        }

        // Проверка на наличие нужных ресурсов у цели
        if ( Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 1)&&(x.Count < 0)).Count >0)
        {
            if (Act._Target.GetComponent<Unit>() == null && Act._Effects._TargetSack == null)
            {
                Log.Notice(scrad,"Target's sack content:");
                Act._Effects._TargetSack.LogAllSack();
                return 6;
            }
            if (Act._Effects._TargetSack == null)
                Act._Effects._TargetSack = Act._Target.GetComponent<Unit>().Sack;
            foreach (ResourceEnumerator _r in Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 1) && (x.Count < 0)))
                if (!Act._Effects._TargetSack.HasAResource(_r.Resource, _r.Count))
                    return 7;
        }
        // Проверка на наличие нужных ресурсов у действующего
        if (Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 2) && (x.Count < 0)).Count > 0)//Создаваемые ресурсы присутсвуют            
        {
            foreach (ResourceEnumerator _r in Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 2) && (x.Count < 0)))
                if (!Sack.HasAResource(_r.Resource, _r.Count))
                    return 9;
        }
        if (Act._Effects._NewBlockResourceType!=null)//Ресурсы для строительства блока присутсвуют
        {
            if (!Act._Effects._TargetSack.HasAResource(Act._Effects._NewBlockResourceType))
                return 7;
        }
        /// Проверка на наличие свободного места у цели 
        if (Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 1) && (x.Count > 0)).Count > 0)
        {
            if (Act._Target.GetComponent<Unit>() == null && !Act._Effects._TargetSack.Exists())
            {
                Log.Notice(scrad, "First condition: " + (Act._Target.GetComponent<Unit>().Sack == null) + " Second condition: " + (!Act._Effects._TargetSack.Exists()));
                Log.Notice(scrad, "Target's sack content:");
                Act._Effects._TargetSack.LogAllSack();
                return 6;
            }
            if (!Act._Effects._TargetSack.Exists())
                Act._Effects._TargetSack = Act._Target.GetComponent<Unit>().Sack;
            if (!Sack.HasEnoughSpaceFor(Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 1) && (x.Count > 0))))
                return 10;                  
        }
        // Проверка на наличие свободного места у актера
        if (Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 2) && (x.Count > 0)).Count > 0)
        {
            if (!Sack.HasEnoughSpaceFor(Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 2) && (x.Count > 0))))
                return 11;
            //foreach (ProcessManager.ResourceEnumerator _r in Act._Effects._ResourcesToCreate.FindAll(x => (x.PutIntoPointer == 2) && (x.Count > 0)))
            //    if (!Sack.HasEnoughSpaceFor(_r.Resource, _r.Count))
            //        return 11;
        }
        // Проверка на занятость участвующих в действии
        // Target:
        // 
        if (Act._Conditions._TargetBusyness != 0)
        {
            if (Act._Target.GetComponent<Unit>() == null)
                return 12;
            if (Act._Target.GetComponent<Unit>().Busy && Act._Conditions._TargetBusyness == 2)
                return 13;
            if (!Act._Target.GetComponent<Unit>().Busy && Act._Conditions._TargetBusyness == 1)
                return 14;
        }
        ///Actor
        if (Act._Conditions._ActorBusyness != 0)
        {
            if (GetComponent<Unit>().Busy && Act._Conditions._ActorBusyness == 2)
                return 15;
            if (!GetComponent<Unit>().Busy && Act._Conditions._ActorBusyness == 1)
                return 16;
        }

        /// Проверка на наличие знаний

        if (Act._Conditions._Knowledges.Count > 0)
            foreach (ProcessManager.KnowledgeEnumerator _k in Act._Conditions._Knowledges)
                if (!GetComponent<Unit>().HasSuchKnowledges(_k.KnowledgeID, _k.KnowledgeLevel))
                    return 17;

        ///Проверка на наличие инструмента
        if (Act._Conditions.RequiredTool != ResourceTypeClassification.Resource)
        {
            //if (Tool == null)//Если требуется инструмент и его нет
            //    return 18;
            //if (Act._Conditions.RequiredTool != Tool.Classification)//Если у юнита в руках инструмент неправильного типа
            //    return 19;            
        }

        return 0;
    }
    /* Список ответов:
     * 0 - все ок
     * 1 - Такое действие отсутствует
     * 2 - Отсутствует требуемая цель
     * 3 - Отсутствует цель требуемого типа
     * 4 - Цель слишком близко
     * 5 - Цель слишком далеко
     * 6 - У цели нет хранилища
     * 7 - В хранилище цели нет достаточного количества ресурсов
     * 8 - У актера нет хранилища
     * 9 - В хранилище актера нет достаточного количества ресурсов
     * 10 - В хранилище цели недостаточно места для завершения действия
     * 11 - В хранилище актера недостаточно места для завершения действия
     * 12 - Невозможно проверить занятость цели, т.к. цель не является юнитом
     * 13 - Цель занята
     * 14 - Цель свободна
     * 15 - Актер занят
     * 16 - Актер свободен
     * 17 - Актер имеет недостаточно знаний
     * 256 - Неизвестная ошибка
     */
    private ushort ActivateWorkflow(ProcessManager.Action Act)
    {
        Log.Notice(scrad,"Workflow activated");
        Act._WorkFlow.SecondsToDo = Act._WorkFlow._LeadTime;
        Act._WorkFlow.SecondsToDo *= 1 + Act._WorkFlow._InfluenceStrength / 100;
        Act._WorkFlow.SecondsToDo *= 1 + Act._WorkFlow._InfluenceAgility / 100;
        Act._WorkFlow.SecondsToDo *= 1 + Act._WorkFlow._InfluenceIntellect / 100;
        Act._WorkFlow.SecondsToDo *= 1 + Act._WorkFlow._InfluenceWisdom / 100;
        Log.Notice(scrad, "Total time to do " + Act._WorkFlow.SecondsToDo + " seconds");
        if (Act._WorkFlow._TargetMethod != null || Act._WorkFlow._ActorMethod != null)//Запускаем методы на актере и на цели
        {
            Type _t;
            if (Act._WorkFlow._ActorMethod != null)
            {
                _t = typeof(Unit).Assembly.GetType( Act._WorkFlow._ActorMethodComponent);                
                gameObject.GetComponent(_t).BroadcastMessage(Act._WorkFlow._TargetMethod);
            }
            if (Act._WorkFlow._TargetMethod != null)
            {
                //Debug.Log("Trying to initiate target's method " + Act._WorkFlow._TargetMethod + " of component " + Act._WorkFlow._TargetMethodComponent);
                _t = typeof(Unit).Assembly.GetType(Act._WorkFlow._TargetMethodComponent);
                //Debug.Log("Found type" + _t.Name + " of " + _t.Assembly.FullName);
                Act._Target.SendMessage(Act._WorkFlow._TargetMethod, SendMessageOptions.RequireReceiver);
            }
        }
        StartCoroutine("Workflow", Act);
        return 0;
    }
    /* Список ответов:
     * 0 - все ок
     */
    private IEnumerator Workflow(ProcessManager.Action Act)
    {
        Log.Notice(scrad,"Processing ...");
        if (Act._WorkFlow._SetTargetBusy)
        {
            Act._Target.GetComponent<Unit>().Busy = true;
        }
        if (Act._WorkFlow._ActorGraphics != null)
            GetComponent<Animator>().SetBool(Act._WorkFlow._ActorGraphics, true);
        if (Act._Conditions.RequiredTool != ResourceTypeClassification.Resource)
        {
            Sack.Output(Resources.ResourceLibrary.Find(x => x.Classification == Act._Conditions.RequiredTool));
            TakeTool(Resources.ResourceLibrary.Find(x => x.Classification == Act._Conditions.RequiredTool));
        }
        float _SecondsPassed = 0;
        while (_SecondsPassed < Act._WorkFlow.SecondsToDo)
        {
            _SecondsPassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (Act._WorkFlow._SetTargetBusy)
        {
            Act._Target.GetComponent<Unit>().Busy = false;
        }
        if (Act._Conditions.RequiredTool != ResourceTypeClassification.Resource)
        {
            Sack.Input(Resources.ResourceLibrary.Find(x => x.Classification == Act._Conditions.RequiredTool),1);
            TakeTool(null);
        }
        if (Act._WorkFlow._ActorGraphics != null)
            GetComponent<Animator>().SetBool(Act._WorkFlow._ActorGraphics, false);
        MakeEffects(Act);
    }
    private ushort MakeEffects(ProcessManager.Action Act)
    {
        // Качаем скилл
        SkillImprove(Act.ID);

        // Накладываем синдром
        if (Act._Effects._SymptomForActor != 0)
            Log.Notice(scrad, "There are no way to put an syndrom");

        // Удаляем цель
        if (Act._Effects._RemoveTarget)
        {
            Log.Notice(scrad, "Need to delete a target");
            if (Act._Target.GetComponent<Brick>() != null)
                Map.RemoveBrick(Act._Target.transform.position);
            else
                return 1;
        }
        // Добавляем/убираем ресурсы при производстве
        if (Act._Effects._ResourcesToCreate.Count > 0)
            foreach (ResourceEnumerator _r in Act._Effects._ResourcesToCreate)
                switch (_r.PutIntoPointer) //2-Actor, 1-Target, 0-Landscape
                {
                    case 2:
                        Sack.Input(_r.Resource, _r.Count);
                        break;
                    case 1:
                        Act._Effects._TargetSack.Input(_r.Resource, _r.Count);
                        break;
                    case 0:
                    default:
                        //кладем ресурс на землю
                        break;
                }
        if (Act._Effects._NewBlockResourceType != null)//Убираем ресурсы при строительстве и строим блок
        {
            Act._Effects._TargetSack.Output(Act._Effects._NewBlockResourceType);
            if (Act._Effects._NewBlockParent == null)
                Map.AddBrick(Act._Effects._NewBlockCoordinates, Act._Effects._NewBlockResourceType);
            else
                Map.AddBrick(Act._Effects._NewBlockCoordinates, Act._Effects._NewBlockResourceType).gameObject.transform.SetParent(Act._Effects._NewBlockParent.transform);
        }
        Log.Notice(scra, "Action " + Act.ID + ". " + Act.Name + " of process #" + Act.ProcessID + " finished", gameObject);
        Busy = false;
        Links.Processes.StepDone(Act.ProcessID);
        return 0;
    }
    /* Список ответов:
     * 0 - все ок
     * 1 - Цель неудаляема
     */

    public void ProcessDone()
    {
        CurrentProcessID = -1;
    }

	// Use this for initialization

	// Update is called once per frame
	void Update () {
	}
    private class Knowledge
    {
        public ushort _KnowledgeID, _KnowledgeLevel, _KnowledgeLevelPoints;
    }
    public class Skill
    {
        public ushort _ActionID, _SlillLevel;
        public int _LevelProgress;
        public Skill(ushort _ID)
        {
            _SlillLevel = 1;
            _LevelProgress = 1;
            _ActionID = _ID;
        }
        public void ActionDone()
        {
            _LevelProgress++;
            if (_LevelProgress >= _SlillLevel * 50)
            {
                _SlillLevel++;
                _LevelProgress = 0;
            }
        }
    } //Замутить прям свой IEnumerable класс со скиллами
}