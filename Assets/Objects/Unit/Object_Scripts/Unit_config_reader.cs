using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Unit_config_reader : MonoBehaviour
{

//    void Start () {
		
//    }
//    void Update () {
		
//    }
//    public void Do(ushort _ActionID, GameObject _Target = null)// Метод должен проверять, может ли юнит выполнить какое либо действие
//    {
//        StartCoroutine(ActionEngine(_ActionID, _Target));
//    }
//    private IEnumerator ActionEngine(ushort _ActionID, GameObject _Target = null)
//    {
//        bool _FoundNoAction = true;
//        ushort _ReturnedReason;
//        string[] _ReasonsList = { "Checking passed successfully", "", "Target required", "Invalid target type for this action", "Target is too close", "Target is too far", "Target has no sack", "Target has no suitable resources", "Actor has no sack", "Actor has no suitable resources", "Actors sack has not enough plase to complete the action", "Targets sack has not enough plase to complete the action", "Cant check target's buisiness. Target is not a Unit", "Target is busy", "target is free", "Actor is busy", "Actor is free", "Actor has unenough knowledges", "Unknown mistake" };
//        Debug.Log("Action engine received the signal successfully");
//        XmlDocument _ActionsConfig = new XmlDocument();
//        _ActionsConfig.Load("C:/GD/You and world/v.0.0.1/First/Configs/Actions.xml");
//        XmlNode _CurrentAction = null;
//        foreach (XmlNode _n in _ActionsConfig.DocumentElement.ChildNodes)
//        {
//            if (ushort.Parse(_n.ChildNodes.Item(0).InnerText) == _ActionID)
//            {
//                _FoundNoAction = false;
//                _CurrentAction = _n;
//                break;
//            }
//        }
//        if (_FoundNoAction)
//        {
//            Debug.LogError("No action with ID " + _ActionID + " found");
//            yield return null;
//        }
//        Debug.Log("Action found " + _CurrentAction.ChildNodes.Item(0).InnerText + ":" + _CurrentAction.ChildNodes.Item(1).InnerText);
//        yield return _ReturnedReason = CheckPossibility(_CurrentAction.ChildNodes.Item(2).ChildNodes, _Target);
//        switch (_ReturnedReason)
//        {
//            case 0:
//                Debug.Log(_ReturnedReason + ". " + _ReasonsList[_ReturnedReason]);
//                ActivateWorkflow(_CurrentAction.ChildNodes.Item(3).ChildNodes, _ActionID, _Target);
//                break;
//            case 4:
//            case 5:
//                Debug.Log(_ReturnedReason + ". " + _ReasonsList[_ReturnedReason] + ". Trying to fix.");
//                int _MinD, _MaxD;
//                if (_CurrentAction.ChildNodes.Item(2).ChildNodes.Item(0).ChildNodes.Item(0).InnerText != null)
//                    _MaxD = int.Parse(_CurrentAction.ChildNodes.Item(2).ChildNodes.Item(0).ChildNodes.Item(0).InnerText);
//                else
//                    _MaxD = int.MaxValue;
//                if (_CurrentAction.ChildNodes.Item(2).ChildNodes.Item(0).ChildNodes.Item(1).InnerText != null)
//                    _MinD = int.Parse(_CurrentAction.ChildNodes.Item(2).ChildNodes.Item(0).ChildNodes.Item(1).InnerText);
//                else
//                    _MinD = 0;
//                GetComponent<_RouteBiulder>().BuildRange(_Target.transform.position, _MaxD, _MinD);
//                yield return new WaitWhile(() => GetComponent<_RouteBiulder>()._IsWalking);
//                Debug.Log("Now i would start doing again");
//                StartCoroutine(ActionEngine(_ActionID, _Target));
//                break;
//            default:
//                Debug.LogError(_ReturnedReason + ". " + _ReasonsList[_ReturnedReason]);
//                break;
//        }
//    }
//    private ushort CheckPossibility(XmlNodeList _Conditions, GameObject _Target = null)
//    {
//        bool _CheckPassed = false;
//        // Проверка на наличие цели/ соответствие типа цели
//        if (IsNodeNotEmpty(_Conditions.Item(5)))
//        {
//            Debug.Log("Node 6 is not empty");
//            List<string> _TargetTypes = new List<string>();
//            Debug.Log("Action possibility checking started");
//            //XMLRecursiveReader(_Conditions.Item(0).ParentNode);
//            foreach (XmlNode _n in _Conditions.Item(5).ChildNodes)
//                if (_n.InnerText == "1")
//                    _TargetTypes.Add(_n.Name);
//            foreach (string _s in _TargetTypes)
//                Debug.Log("Possible target: " + _s);
//            if ((_Target == null) && (_TargetTypes.Count > 0))
//                return 2;
//            foreach (string _s in _TargetTypes)
//                if (_s == _Target.tag)
//                    _CheckPassed = true;
//            if (!_CheckPassed)
//                return 3;
//        }
//        else
//            Debug.Log("Node 6 is empty");

//        // Проверка на расстояние до цели
//        if (IsNodeNotEmpty(_Conditions.Item(0)))
//        {
//            Debug.Log("Node 1 is not empty");
//            Debug.Log("Start checking distance to target");
//            float _MinDistance = float.Parse(_Conditions.Item(0).ChildNodes.Item(0).InnerText), _MaxDistance = float.Parse(_Conditions.Item(0).ChildNodes.Item(1).InnerText);
//            Debug.Log("Distance - Min:" + _MinDistance + ", Max:" + _MaxDistance + ". Current:" + Vector3.Distance(GetComponent<Transform>().position, _Target.transform.position));
//            if (Vector3.Distance(GetComponent<Transform>().position, _Target.transform.position) < _MinDistance)
//                return 4;
//            if (Vector3.Distance(GetComponent<Transform>().position, _Target.transform.position) > _MaxDistance)
//                return 5;
//        }
//        else
//            Debug.Log("Node 1 is empty");

//        // Проверка на наличие нужных ресурсов у цели
//        if (IsNodeNotEmpty(_Conditions.Item(1)))
//        {
//            Debug.Log("Node 2 is not empty");
//            List<int> _ResourceTypes = new List<int>();
//            List<int> _ResourceCounts = new List<int>();
//            Debug.Log("Start checking target" + _Target + " resources");
//            if (_Target.GetComponent<Sack>() != null)
//            {
//                _CheckPassed = true;
//                foreach (XmlNode _n in _Conditions.Item(1).ChildNodes)
//                    _ResourceTypes.Add(int.Parse(_n.ChildNodes.Item(0).InnerText));
//                foreach (XmlNode _n in _Conditions.Item(1).ChildNodes)
//                    if (_n.ChildNodes.Item(1).InnerText == null)
//                        _ResourceTypes.Add(1);
//                    else
//                        _ResourceTypes.Add(int.Parse(_n.ChildNodes.Item(1).InnerText));

//                for (int i = 0; i < _ResourceTypes.Count; i++)
//                    if (!_Target.GetComponent<Sack>().HasAResource(_ResourceTypes[i], _ResourceCounts[i]))
//                        _CheckPassed = false;
//            }
//            else
//                return 6;
//            if (!_CheckPassed)
//                return 7;
//            _ResourceCounts.Clear();
//            _ResourceCounts.TrimExcess();
//            _ResourceTypes.Clear();
//            _ResourceTypes.TrimExcess(); ;
//        }
//        else
//            Debug.Log("Node 2 is empty");

//        // Проверка на наличие нужных ресурсов у действующего
//        if (IsNodeNotEmpty(_Conditions.Item(2)))
//        {
//            Debug.Log("Node 3 is not empty");
//            List<int> _ResourceTypes = new List<int>();
//            List<int> _ResourceCounts = new List<int>();
//            _CheckPassed = true;
//            Debug.Log("Start checking actors resources");
//            if (GetComponent<Sack>() != null)
//            {
//                foreach (XmlNode _n in _Conditions.Item(2).ChildNodes)
//                    _ResourceTypes.Add(int.Parse(_n.ChildNodes.Item(0).InnerText));
//                foreach (XmlNode _n in _Conditions.Item(2).ChildNodes)
//                    if (_n.ChildNodes.Item(1).InnerText == null)
//                        _ResourceTypes.Add(1);
//                    else
//                        _ResourceTypes.Add(int.Parse(_n.ChildNodes.Item(1).InnerText));
//                for (int i = 0; i < _ResourceTypes.Count; i++)
//                    if (!GetComponent<Sack>().HasAResource(_ResourceTypes[i], _ResourceCounts[i]))
//                        _CheckPassed = false;
//            }
//            else
//                return 8;
//            if (!_CheckPassed)
//                return 9;
//            _ResourceCounts.Clear();
//            _ResourceCounts.TrimExcess();
//            _ResourceTypes.Clear();
//            _ResourceTypes.TrimExcess();
//        }
//        else
//            Debug.Log("Node 3 is empty");

//        /// Проверка на наличие свободного места у цели
//        if (IsNodeNotEmpty(_Conditions.Item(0).ParentNode.ParentNode.ChildNodes.Item(4).ChildNodes.Item(3)))
//        {
//            _CheckPassed = false;
//            List<int> _ResourcesCounts = new List<int>();
//            List<int> _ResourcesTypes = new List<int>();
//            XmlNodeList _ResourcesToAdd = _Conditions.Item(0).ParentNode.ParentNode.ChildNodes.Item(4).ChildNodes.Item(3).ChildNodes;
//            /// Actor sack capacity checking
//            foreach (XmlNode _n in _ResourcesToAdd)
//            {
//                Debug.Log("Check resources to add in actors sack " + _n.Name + " ");
//                if (_n.ChildNodes.Item(2).InnerText == "2")
//                {
//                    _ResourcesTypes.Add(int.Parse(_n.ChildNodes.Item(0).InnerText));
//                    if (!IsNodeNotEmpty(_n.ChildNodes.Item(1)))
//                        _ResourcesCounts.Add(1);
//                    else
//                        _ResourcesCounts.Add(int.Parse(_n.ChildNodes.Item(1).InnerText));
//                }
//            }
//            if (!GetComponent<Sack>().HasEnoughSpaceFor(_ResourcesTypes, _ResourcesCounts))
//                return 10;
//            _ResourcesCounts.Clear();
//            _ResourcesTypes.Clear();
//            /// Target sack capacity checking
//            foreach (XmlNode _n in _ResourcesToAdd)
//                if (_n.ChildNodes.Item(2).InnerText == "1")
//                {
//                    _ResourcesTypes.Add(int.Parse(_n.ChildNodes.Item(0).InnerText));
//                    if (!IsNodeNotEmpty(_n.ChildNodes.Item(1)))
//                        _ResourcesCounts.Add(1);
//                    else
//                        _ResourcesCounts.Add(int.Parse(_n.ChildNodes.Item(1).InnerText));
//                }
//            if (_ResourcesCounts.Count > 0)
//            {
//                Debug.Log("Need to check " + _ResourcesCounts.Count + " resources in targets sack");
//                if (_Target.GetComponent<Sack>() != null)
//                {
//                    if (!_Target.GetComponent<Sack>().HasEnoughSpaceFor(_ResourcesTypes, _ResourcesCounts))
//                        return 11;
//                }
//                else
//                    return 6;
//            }
//            _ResourcesCounts.Clear();
//            _ResourcesTypes.Clear();
//        }
//        else
//            Debug.Log("Free space not required");

//        /// Проверка на занятость участвующих в действии
//        /// Target:
//        /// 
//        if (_Conditions.Item(3).ChildNodes.Item(0).InnerText != "0")
//        {
//            if (!IsNodeNotEmpty(_Conditions.Item(3).ChildNodes.Item(0)))
//            {
//                if (_Target.GetComponent<Unit>() == null)
//                    return 12;
//                if (_Target.GetComponent<Unit>()._Busy)
//                    return 13;
//            }
//            else
//            {
//                if (!_Target.GetComponent<Unit>()._Busy)
//                    return 14;
//            }
//        }
//        ///Actor
//        if (_Conditions.Item(3).ChildNodes.Item(1).InnerText != "0")
//        {
//            if (!IsNodeNotEmpty(_Conditions.Item(3).ChildNodes.Item(1)))
//            {
//                if (GetComponent<Unit>()._Busy)
//                    return 15;
//            }
//            else
//            {
//                if (GetComponent<Unit>()._Busy)
//                    return 16;
//            }
//        }

//        /// Проверка на наличие знаний
//        if (IsNodeNotEmpty(_Conditions.Item(4)))
//        {
//            List<int> _IDs = new List<int>();
//            List<int> _LVLs = new List<int>();
//            foreach (XmlNode _n in _Conditions.Item(4).ChildNodes)
//            {
//                _IDs.Add(int.Parse(_Conditions.Item(4).ChildNodes.Item(0).InnerText));
//                _LVLs.Add(int.Parse(_Conditions.Item(4).ChildNodes.Item(1).InnerText));
//            }
//            if (!GetComponent<Unit>().HasSuchKnowledges(_IDs, _LVLs))
//                return 17;
//            Debug.Log("Node 6 is not empty");
//        }
//        else
//            Debug.Log("Node 6 is empty");

//        return 0;
//    }
//    /* Список ответов:
//     * 0 - все ок
//     * 1 - Такое действие отсутствует
//     * 2 - Отсутствует требуемая цель
//     * 3 - Отсутствует цель требуемого типа
//     * 4 - Цель слишком близко
//     * 5 - Цель слишком далеко
//     * 6 - У цели нет хранилища
//     * 7 - В хранилище цели нет достаточного количества ресурсов
//     * 8 - У актера нет хранилища
//     * 9 - В хранилище актера нет достаточного количества ресурсов
//     * 10 - В хранилище актера недостаточно места для завершения действия
//     * 11 - В хранилище цели недостаточно места для завершения действия
//     * 12 - Невозможно проверить занятость цели, т.к. цель не является юнитом
//     * 13 - Цель занята
//     * 14 - Цель свободна
//     * 15 - Актер занят
//     * 16 - Актер свободен
//     * 17 - Актер имеет недостаточно знаний
//     * 256 - Неизвестная ошибка
//     */
//    private ushort ActivateWorkflow(XmlNodeList _WF, ushort _ActionID, GameObject _Target = null)
//    {
//        float _SecondsToDo;
//        WorkflowPackage _wp;
//        _SecondsToDo = float.Parse(_WF.Item(0).ChildNodes.Item(0).InnerText);
//        _SecondsToDo *= float.Parse(_WF.Item(0).ChildNodes.Item(1).ChildNodes.Item(0).InnerText) * GetComponent<Unit>()._Strength;
//        _SecondsToDo *= float.Parse(_WF.Item(0).ChildNodes.Item(1).ChildNodes.Item(1).InnerText) * GetComponent<Unit>()._Agility;
//        _SecondsToDo *= float.Parse(_WF.Item(0).ChildNodes.Item(1).ChildNodes.Item(2).InnerText) * GetComponent<Unit>()._Intellect;
//        _SecondsToDo *= float.Parse(_WF.Item(0).ChildNodes.Item(1).ChildNodes.Item(3).InnerText) * GetComponent<Unit>()._Wisdom;
//        _wp = new WorkflowPackage(_Target,_WF.Item(0).ParentNode.ParentNode.ChildNodes.Item(4).ChildNodes,_SecondsToDo, IsNodeNotEmpty(_WF.Item(1).ChildNodes.Item(0)), IsNodeNotEmpty(_WF.Item(1).ChildNodes.Item(1)));

//        //bool _ActorsBusyness = IsNodeNotEmpty(_WF.Item(1).ChildNodes.Item(0)),
//        //    _TargetBusyness = IsNodeNotEmpty(_WF.Item(1).ChildNodes.Item(1));

//        //if (_ActorBusyness)
//        //    GetComponent<Unit>()._Busy = true;
//        //if (_TargetBusyness)
//        //    _Target.GetComponent<Unit>()._Busy = true;
//        Debug.Log("Total time to do:" + _SecondsToDo);
//        StartCoroutine("Workflow", _wp);             
//        return 0;
//    }
//    private IEnumerator Workflow(WorkflowPackage _wp)
//    {
//        float _SecondsPassed = 0;
//        if (_wp._ActorBusyness)
//            GetComponent<Unit>()._Busy = true;
//        if (_wp._TargetBusyness)
//            _wp._Target.GetComponent<Unit>()._Busy = true;
//        while (_SecondsPassed <= _wp._SecondsToDo)
//        {
//            _SecondsPassed += Time.deltaTime;
//            yield return new WaitForEndOfFrame();
//            Debug.Log((_wp._SecondsToDo - _SecondsPassed) + " seconds still remains");
//        }
//        if (_wp._ActorBusyness)
//            GetComponent<Unit>()._Busy = false;
//        if (_wp._TargetBusyness)
//            _wp._Target.GetComponent<Unit>()._Busy = false;
//        MakeEffects(_wp._Effects,_wp._Target);
//        Debug.Log("Done");
//    }
//    private void MakeEffects(XmlNodeList _Effects, GameObject _Target)
//    {
//        GetComponent<Unit>().SkillImprove(ushort.Parse(_Effects.Item(0).ParentNode.ParentNode.ChildNodes.Item(0).InnerText));
//        if (IsNodeNotEmpty(_Effects.Item(0)))
//            Debug.Log("There are no way to put an syndrom");
//        if (IsNodeNotEmpty(_Effects.Item(1)))
//            Debug.Log("There are no way to improve characteristics");
//        if (IsNodeNotEmpty(_Effects.Item(2)))
//        {
//            Debug.Log("Need to delete a target");
//            if (_Target.GetComponent<Brick>() != null)
//                _Target.GetComponent<Brick>().Remove();
//        }
//    }
//    private bool IsNodeNotEmpty(XmlNode _Root)
//    {
//        if (_Root.HasChildNodes)
//            foreach (XmlNode _n in _Root.ChildNodes)
//            {
//                if (IsNodeNotEmpty(_n))
//                    return true;
//            }
//        if (_Root.InnerText == "")
//            return false;
//        else
//        {
//            //Debug.Log("Not empty element is " + _Root.Name + ": " +_Root.InnerText);
//            return true;
//        }
//    }
//    private void XMLRecursiveReader(XmlNode _Root, int _RCount = 0)
//    {
//        string _d = new string('.', _RCount);
//        Debug.Log(_d + " " + _Root.Name + " " + _Root.InnerText);
//        foreach (XmlNode _n in _Root.ChildNodes)
//            XMLRecursiveReader(_n, _RCount + 1);
//    }
//    private class WorkflowPackage
//    {
//        public float _SecondsToDo;
//        public bool _ActorBusyness, _TargetBusyness;
//        public GameObject _Target;
//        public XmlNodeList _Effects;
//        public WorkflowPackage(GameObject _t,XmlNodeList _e, float _s, bool _ab, bool _tb)
//        {
//            _Target = _t;
//            _SecondsToDo = _s;
//            _ActorBusyness = _ab;
//            _TargetBusyness = _tb;
//            _Effects = _e;
//        }
//    }
}