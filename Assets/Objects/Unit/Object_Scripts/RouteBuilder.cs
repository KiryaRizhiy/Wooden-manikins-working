using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteBuilder : MonoBehaviour {

    public GameObject _Pointer;
    public int _MaxIterations, _MaxWalkingAroundDistance;
    public float _WalkSpeed;
    public bool _ShowLines,_Walk,LogWriting=false;
    public bool _IsWalking { get; private set; }

    private RaycastHit _Hit;
    private float _xpos, _zpos, _MaxDistance = float.MaxValue, _MinDistance = 0;
    private List<Vector3> _Route = new List<Vector3>();
    private Transform _Transform;
    private int _RecurcionDepth;
    private int mask = (1<<9)|(1<<11);
    private Vector3 _Target;
    private string scr = "UnitRoute", scrw = "UnitRouteWalking";
    
    void Start()
    {
        _Transform = GetComponent<Transform>();
        mask = ~mask;
        _IsWalking = false;
    }
    public void LogRoute()
    {
        foreach(Vector3 _Step in _Route)
            Log.Notice(scr,_Step);
    }
    public void BuildRange(Vector3 _Aim, float _MaxD, float _MinD)//Вычисляем правильную цель
    {
        List<RaycastHit> _HitArr = new List<RaycastHit>();
        RaycastHit _Hit;
        _MaxDistance = _MaxD;
        _MinDistance = _MinD;
        if (Vector3.Distance(_Transform.position, _Aim) < _MinDistance)
        {
            _Target = _Transform.position + (_Transform.position - _Aim);
            if (Physics.RaycastAll(_Target + Vector3.down, Vector3.up, 3f, mask).Length > 0)
            {
                _HitArr.AddRange(Physics.RaycastAll(_Target + Vector3.down, Vector3.up, 50f, mask));
                //_TempHit = _HitArr[_HitArr.Count - 1]; // неправильно нахожу следующую позицию. Надо находить минимальную "Дыру" высотой в 2 блока, и ставить точку на ней. Иначе - не пройдет по тоннелю
                //_Target.y = _HitArr[_HitArr.Count - 1].point.y + 1f;
                _Target.y = _HitArr.Find(x=>
                    (_HitArr.IndexOf(x)==_HitArr.Count-1)//Выбираем последний элемент
                    ||//Или
                    (
                        (_HitArr.IndexOf(x) < _HitArr.Count - 1)//Он непоследний
                        &&//И
                        (_HitArr[_HitArr.IndexOf(x)+1].point.y - x.point.y >= 3)//Между соседними элементами имеется зазор в 3 единицы
                        &&//И
                        (x.point.y >= gameObject.transform.position.y-1)//Он выше объекта
                    )
                    ).point.y + 1f;
                _HitArr.Clear();
            }
            else
            {
                Physics.Raycast(_Target + Vector3.down*0.9f, Vector3.down, out _Hit, 50f, mask);
                _Target = _Hit.point;
            }
            if (LogWriting)
                Log.Notice(scr,"Target inverted");
        }
        else
        {
            _Target = _Aim;
            _Target.y += 0.5f;
        }
        Build(_Target);
    }
    public void Build(Vector3 _Aim)
    {
        if (LogWriting)
            Log.Notice(scr,"Building way initiated");
        _IsWalking = true;
        StopAllCoroutines();
        _RecurcionDepth = 0;
        _Route.Clear();
        Physics.Raycast(GetComponent<Transform>().position, Vector3.down, out _Hit,10,mask);
        if (Mathf.Round(_Hit.point.x) > 0)
            _xpos = Mathf.Floor(_Hit.point.x);
        else
            _xpos = Mathf.Ceil(_Hit.point.x);
        if (Mathf.Round(_Hit.point.z) > 0)
            _zpos = Mathf.Floor(_Hit.point.z);
        else
            _zpos = Mathf.Ceil(_Hit.point.z);
        _Route.Add(_Hit.point);
        _Route.Add(new Vector3(_xpos, _Hit.point.y, _zpos));
        StartCoroutine("BuildWay",new Vector3[2]{(new Vector3(_xpos, _Hit.point.y, _zpos)),_Aim});
        //StartCoroutine("BuildWay", new Vector3[2] { _Hit.point, _Aim });
    }
    IEnumerator BuildRec(Vector3[] BypassCoordinates)
    {
        if (LogWriting)
            Log.Notice(scr,"Start building the bypass form " + _Route[_Route.Count - 1] + " to " + BypassCoordinates[0]);
        yield return StartCoroutine("BuildWay", new Vector3[2] { _Route[_Route.Count - 1], BypassCoordinates[0] });
        if (LogWriting)
            Log.Notice(scr,"Reached the bypass point. Start building the rest of the route");
        yield return StartCoroutine("BuildWay", new Vector3[2] { _Route[_Route.Count - 1], BypassCoordinates[1] });
    }
    IEnumerator BuildWay(Vector3[] StartNTarget)
    {
        _RecurcionDepth += 1;
        int _Iteration = 0, _CyclesDepth = _RecurcionDepth;
        RaycastHit _TempHit = new RaycastHit();
        WaitForEndOfFrame _Wait = new WaitForEndOfFrame();
        List<RaycastHit> _HitArr = new List<RaycastHit>();
        Vector3 _CurrentPositon = StartNTarget[0], _StartPosition = StartNTarget[0], _Target = StartNTarget[1];
        Vector2 _CurrPos2D;
        if (LogWriting)
            Log.Notice(scr,"Basic way building iteration " + _CyclesDepth + " from " + StartNTarget[0] + " to " + StartNTarget[1] + " started");
        while (_CurrentPositon != _Target)
        {
            _CurrPos2D = NextPosition(new Vector2(_StartPosition.x, _StartPosition.z), new Vector2(_Target.x, _Target.z), new Vector2(_CurrentPositon.x, _CurrentPositon.z));
            //Debug.Log("Shoot from " + new Vector3(_CurrPos2D.x, _CurrentPositon.y - 1.1f, _CurrPos2D.y));
            if (Physics.RaycastAll(new Vector3(_CurrPos2D.x, _CurrentPositon.y - 1.1f, _CurrPos2D.y), Vector3.up, 5.5f,mask).Length > 1)
            {
                _HitArr.AddRange(Physics.RaycastAll(new Vector3(_CurrPos2D.x, _CurrentPositon.y - 1.1f, _CurrPos2D.y), Vector3.up, 5.5f, mask));
                //_TempHit = _HitArr[_HitArr.Count - 1]; // неправильно нахожу следующую позицию. Надо находить минимальную "Дыру" высотой в 2 блока, и ставить точку на ней. Иначе - не пройдет по тоннелю
                _TempHit = _HitArr.Find(x =>
                    (
                        (_HitArr.IndexOf(x) < _HitArr.Count - 1)//Он непоследний
                        &&//И
                        (_HitArr[_HitArr.IndexOf(x) + 1].point.y - x.point.y >= 3)//Между соседними элементами имеется зазор в 3 единицы
                        &&//И
                        (x.point.y >= _Route[_Route.Count-1].y - 1)//Он выше объекта
                    ));
                Log.Notice(scr,"Logging raycast array");
                foreach (RaycastHit _h in _HitArr)
                    Log.Notice(scr,"Ray #" + _HitArr.IndexOf(_h) + ", point: " + _h.point + ", object: " + _h.transform.name);
                if (!(_HitArr.IndexOf(_TempHit)>=0))
                    _TempHit = _HitArr.Find(x => (_HitArr.IndexOf(x) == _HitArr.Count - 1));
                _CurrentPositon = new Vector3(_TempHit.point.x, _TempHit.point.y + 1f, _TempHit.point.z);
            }
            else
            {
                Log.Notice(scr, "No let found. Looking for position to move forwards");
                Physics.Raycast(new Vector3(_CurrPos2D.x, _CurrentPositon.y + 1.1f, _CurrPos2D.y), Vector3.down, out _Hit, 10, mask);
                Log.Notice(scr, "Position is" + _Hit.point + ". Target is " + _Target);
                _CurrentPositon = _Hit.point;
            }
            if (_Iteration > _MaxIterations)
            {
                _Iteration = 0;
                yield return _Wait;
                //Log.Warning(scr, "Max iterations reached, route building breakes");
                //break;
            }
            if (_HitArr != null)
                if (_HitArr.Count > 0)
                    _HitArr.Clear();
            //Log.Notice(scr,"Add to route " + _CurrentPositon);
            _Route.Add(_CurrentPositon);
            ShowLine(_Route[_Route.Count - 2], _Route[_Route.Count - 1]);
            _Iteration += 1;
        }
            //Log.Notice(scr,"Initiate analyzing");
            yield return StartCoroutine("BasicWaypointsAnalyzer", _CyclesDepth);
            if (LogWriting)
                Log.Notice(scr, "Route building step " + _CyclesDepth + " finished");
            ShowTheWay(_Route, false);
            if (_CyclesDepth == 1)
            {
                StopCoroutine("Walk");
                StopCoroutine("WalkToPoint");
                StartCoroutine("Walk");
            }
        
    }
    IEnumerator BasicWaypointsAnalyzer(int _Itreation)
    {
        if (LogWriting)
            Log.Notice(scr,"Analyzing step " + _Itreation + " started");
        Vector3[] _Dots;
        Vector3 _Target,_AlternativePoint1, _AlternativePoint2;
        //RaycastHit _Hit;
        //List<Vector3> _RouteCopy = new List<Vector3>();

        //Log.Notice(scr,"Route for analysis:");
        //for (int j = 0; j < _Route.Count; j += 1)
        //Log.Notice(scr,_Route[j]);

        for (int i = 0; i < _Route.Count - 1; i += 1)
        {
            //Log.Notice(scr,"Analyzing " + _Route[i]);
            if ((_Route[i].y - _Route[i + 1].y) < (-1)) // Встретили препядствие, обрабатываем
            {
                if (LogWriting)
                    Log.Notice(scr,"Met the let " + _Route[i + 1] + " at iterration step " + i + ". Waypoints count =" + _Route.Count);
                _Dots = DotsForRaycast(_Route[i], _Route[i + 1]);
                _Target = _Route[_Route.Count-1];
                
                ShowLine(_Dots[1], _Dots[1] + _Dots[0]);
                _AlternativePoint1 = LetCrossing(_Route[i],_Dots[5],_Dots[3],true);
                ShowLine(_Dots[2], _Dots[2] + _Dots[0]);
                _AlternativePoint2 = LetCrossing(_Route[i],_Dots[5],_Dots[4],false);
                _Route.RemoveRange(i + 1, _Route.Count - i - 1);
                if (Vector3.Distance(_AlternativePoint1, _Target) < Vector3.Distance(_AlternativePoint2, _Target))// Вместо цели поставить текущую точку
                {
                    yield return StartCoroutine("BuildRec", new Vector3[] { _AlternativePoint1, _Target });
                }
                else
                {
                    yield return StartCoroutine("BuildRec", new Vector3[] { _AlternativePoint2, _Target });
                }
                break;
            }
            else
                if ((_Route[i].y - _Route[i + 1].y) > 1) // Встретили пропасть, обрабатываем
                {
                    if (LogWriting)
                        Log.Notice(scr,"Met the abyss " + _Route[i + 1] + " at iterration step " + i + ". Waypoints count =" + _Route.Count);
                    _Dots = DotsForRaycast(_Route[i + 1], _Route[i]);
                    _Target = _Route[_Route.Count - 1];

                    ShowLine(_Dots[1], _Dots[1] + _Dots[0]);
                    _AlternativePoint1 = AbyssCrossing(_Route[i],-1*_Dots[5],_Dots[4], true);
                    ShowLine(_Dots[2], _Dots[2] + _Dots[0]);
                    _AlternativePoint2 = AbyssCrossing(_Route[i],-1*_Dots[5],_Dots[3], false);
                    _Route.RemoveRange(i + 1, _Route.Count - i - 1);
                    if (Vector3.Distance(_AlternativePoint1, _Target) < Vector3.Distance(_AlternativePoint2, _Target))
                    {
                        yield return StartCoroutine("BuildRec", new Vector3[] { _AlternativePoint1, _Target });
                    }
                    else
                    {
                        yield return StartCoroutine("BuildRec", new Vector3[] { _AlternativePoint2, _Target });
                    }
                    break;
                }
                else
                {
                // Путь проходим, все норм
                }
            if ((Mathf.Ceil(i / _MaxIterations) - (i / _MaxIterations)) == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        if (LogWriting)
            Log.Notice(scr,"The way " + _Itreation + " analyzed");
        yield return new WaitForEndOfFrame(); 
    }
    private Vector3 LetCrossing(Vector3 _CurrentPosition, Vector3 _LetDirection, Vector3 _CheckingDirection, bool _GoRight, bool _NotComplexCrossing = true)
    {
        Vector3 _AdditionalLetDirection;
        RaycastHit _Hit;
        bool _FoundPassedPointFlg = false,_LetFlg = false;

        // вычислять GoRight самостоятельно

        if (_GoRight)
        {
            _AdditionalLetDirection = Functions.LeftVector(_CheckingDirection);
        }
        else
        {
            _AdditionalLetDirection = Functions.RightVector(_CheckingDirection);
        }
        if (LogWriting)
            Log.Notice(scr,"Start building the route around the let " + (_CurrentPosition + _LetDirection) + " in direction of " + _LetDirection + _AdditionalLetDirection + ". Looking for bypass in direction of " + _CheckingDirection + ". Embeded cycle = " + (!_NotComplexCrossing));
        _CurrentPosition.y += 1.5f;

        for (int i = 1; i < _MaxWalkingAroundDistance; i += 1)
        {
            //ShowLine(_CurrentPosition + _CheckingDirection * (i - 1), _CurrentPosition + _CheckingDirection * i);
            if (!Physics.Raycast(_CurrentPosition + _CheckingDirection * (i - 1), _CheckingDirection, 1f, mask) || _LetFlg) // Нет ли препядствий для движения вправо?
            {
                //ShowLine(_CurrentPosition + _CheckingDirection * i, _CurrentPosition + _CheckingDirection * i + Vector3.down);
                if (Physics.Raycast(_CurrentPosition + _CheckingDirection * i, Vector3.down, out _Hit, 2.6f, mask) || _LetFlg) //Нет ли в точке справа пропасти?
                {
                    _CurrentPosition.y = _Hit.point.y + 1.5f;
                    if (_LetFlg&&_NotComplexCrossing)
                    {
                        _CurrentPosition.y -= 1.5f;
                        if (LogWriting)
                            Log.Notice(scr,"No bypass for the let found. Try to get around first");
                        if (_GoRight)
                            return LetCrossing(LetCrossing((_CurrentPosition + _CheckingDirection * (i - 2)), Functions.RightVector(_LetDirection), Functions.RightVector(_CheckingDirection), _GoRight, false), _LetDirection, _CheckingDirection, _GoRight);
                        else
                            return LetCrossing(LetCrossing((_CurrentPosition + _CheckingDirection * (i - 2)), Functions.LeftVector(_LetDirection), Functions.LeftVector(_CheckingDirection), _GoRight, false), _LetDirection, _CheckingDirection, _GoRight);
                    }
                    //ShowLine((_CurrentPosition + _CheckingDirection * i), (_CurrentPosition + _CheckingDirection * i + _LetDirection));
                    //ShowLine((_CurrentPosition + _CheckingDirection * i), (_CurrentPosition + _CheckingDirection * i + _AdditionalLetDirection));
                    if ((!Physics.Raycast((_CurrentPosition + _CheckingDirection * i), _LetDirection, 1f, mask)) || (!Physics.Raycast((_CurrentPosition + _CheckingDirection * i), _AdditionalLetDirection, 1f, mask)))// Можем ли пройти вперед?
                    {
                        if (_NotComplexCrossing)
                        {
                            foreach (Vector3 v in _Route)
                            {
                                if (_Hit.point == v)
                                {
                                    _FoundPassedPointFlg = true;
                                    break;
                                }
                            }
                            if (!(_FoundPassedPointFlg))
                            {
                                if (LogWriting)
                                    Log.Notice(scr,"Found the baypass for the let at " + _Hit.point);
                                return (_CurrentPosition + _CheckingDirection * i - new Vector3(0, 1.5f, 0));
                            }
                            else
                            {
                                if (LogWriting)
                                    Log.Notice(scr,"No baypass for the let. The only existing baypass " + _Hit.point + " already passed");
                            }
                        }
                        else
                        {
                            if (Physics.Raycast((_CurrentPosition + _CheckingDirection * i + _AdditionalLetDirection), Vector3.down, out _Hit, 2.6f, mask))
                                {
                                    ShowLine((_CurrentPosition + _CheckingDirection * i + _AdditionalLetDirection), (_CurrentPosition + _CheckingDirection * i + _AdditionalLetDirection + Vector3.down));
                                    if (LogWriting)
                                        Log.Notice(scr,"Found the baypass around the let at " + _Hit.point);
                                    return _Hit.point;
                                }
                            
                            else
                            {
                                if (Physics.Raycast((_CurrentPosition + _CheckingDirection * i + _LetDirection), Vector3.down, out _Hit, 2.6f, mask))
                                {
                                    ShowLine((_CurrentPosition + _CheckingDirection * i + _LetDirection), (_CurrentPosition + _CheckingDirection * i + _LetDirection + Vector3.down));
                                    if (LogWriting)
                                        Log.Notice(scr,"Found the baypass around the let at " + _Hit.point);
                                    return _Hit.point;
                                }
                                else
                                    if (LogWriting)
                                        Log.Notice(scr,"There is some shit in method of complex route building... need check");
                            }
                        }
                    }
                }
                else
                {
                    if (LogWriting)
                        Log.Notice(scr,"Found an abyss at " + (_CurrentPosition + _CheckingDirection * (i - 1)));
                    return Vector3.positiveInfinity;
                }
            }
            else
            {
                _LetFlg = true;
            }
        }
        if (LogWriting)
            Log.Notice(scr,"No baypass for the let. Max walking around distance reached");
        return Vector3.positiveInfinity;
    }
    private Vector3 AbyssCrossing(Vector3 _CurrentPosition, Vector3 _AbyssDirection, Vector3 _CheckingDirection, bool _GoRight, bool _NotComplexCrossing = true)
    {
        Vector3 _AdditionalAbyssDirection;
        RaycastHit _Hit;
        bool _FoundPassedPointFlg = false,_LetFlg = false,_AbyssFlg = false;

        // вычислять GoRight самостоятельно

        if (_GoRight)
        {
            _AdditionalAbyssDirection = Functions.LeftVector(_CheckingDirection);
        }
        else
        {
            _AdditionalAbyssDirection = Functions.RightVector(_CheckingDirection);
        }
        if (LogWriting)
            Log.Notice(scr,"Start building the route around the abyss " + (_CurrentPosition + _AbyssDirection) + " in direction of " + _AbyssDirection + _AdditionalAbyssDirection + ". Looking for bypass in direction of " + _CheckingDirection + ". Embeded cycle = " + (!_NotComplexCrossing));
        _CurrentPosition.y += 1.5f;

        for (int i = 1; i < _MaxWalkingAroundDistance; i += 1)
        {
            //ShowLine(_CurrentPosition + _CheckingDirection * (i - 1), _CurrentPosition + _CheckingDirection * i);
            if (!Physics.Raycast(_CurrentPosition + _CheckingDirection * (i - 1), _CheckingDirection, 1f, mask) || _LetFlg || _AbyssFlg) // Нет ли препядствий для движения вправо?
            {
                //ShowLine(_CurrentPosition + _CheckingDirection * i, _CurrentPosition + _CheckingDirection * i + Vector3.down);
                if (Physics.Raycast(_CurrentPosition + _CheckingDirection * i, Vector3.down, out _Hit, 2.6f, mask) || _LetFlg || _AbyssFlg) //Нет ли в точке справа пропасти?
                {
                    _CurrentPosition.y = _Hit.point.y + 1.5f;
                    if (_LetFlg/*&&_NotComplexCrossing*/)
                    {
                        _CurrentPosition.y -= 1.5f;
                        if (LogWriting)
                            Log.Notice(scr,"No bypass for the abyss found. Try to get around first");
                        if (_GoRight)
                            return LetCrossing(LetCrossing((_CurrentPosition + _CheckingDirection * (i - 2)), Functions.RightVector(_AbyssDirection), Functions.RightVector(_CheckingDirection), _GoRight, false), _AbyssDirection, _CheckingDirection, _GoRight);
                        else
                            return LetCrossing(LetCrossing((_CurrentPosition + _CheckingDirection * (i - 2)), Functions.LeftVector(_AbyssDirection), Functions.LeftVector(_CheckingDirection), _GoRight, false), _AbyssDirection, _CheckingDirection, _GoRight);
                    }
                    if (_AbyssFlg)
                    {
                        _CurrentPosition.y -= 1.5f;
                        if (LogWriting)
                            Log.Notice(scr,"No bypass for the abyss found. Try to get around first");
                        if (_GoRight)
                            return AbyssCrossing(AbyssCrossing((_CurrentPosition + _CheckingDirection * (i - 2)), Functions.RightVector(_AbyssDirection), Functions.RightVector(_CheckingDirection), _GoRight, false), _AbyssDirection, _CheckingDirection, _GoRight);
                        else
                            return AbyssCrossing(AbyssCrossing((_CurrentPosition + _CheckingDirection * (i - 2)), Functions.LeftVector(_AbyssDirection), Functions.LeftVector(_CheckingDirection), _GoRight, false), _AbyssDirection, _CheckingDirection, _GoRight);

                    }
                    //ShowLine((_CurrentPosition + _CheckingDirection * i), (_CurrentPosition + _CheckingDirection * i + _AbyssDirection));
                    //ShowLine((_CurrentPosition + _CheckingDirection * i), (_CurrentPosition + _CheckingDirection * i + _AdditionalAbyssDirection));
                    if ((Physics.Raycast((_CurrentPosition + _CheckingDirection * i + _AbyssDirection), Vector3.down, 2.6f, mask)) || (Physics.Raycast((_CurrentPosition + _CheckingDirection * i + _AdditionalAbyssDirection), Vector3.down, 2.6f, mask)))// Можем ли пройти вперед?
                    {
                        if (_NotComplexCrossing)
                        {
                            foreach (Vector3 v in _Route)
                            {
                                if (_Hit.point == v)
                                {
                                    _FoundPassedPointFlg = true;
                                    break;
                                }
                            }
                            if (!(_FoundPassedPointFlg))
                            {
                                if (LogWriting)
                                    Log.Notice(scr,"Found the baypass for the abyss at " + _Hit.point);
                                return (_CurrentPosition + _CheckingDirection * i - new Vector3(0,1.5f,0));
                            }
                            else
                            {
                                if (LogWriting)
                                    Log.Notice(scr,"No baypass for the Abyss. The only existing baypass " + _Hit.point + " already passed");
                            }
                        }
                        else
                        {
                            if (Physics.Raycast((_CurrentPosition + _CheckingDirection * i + _AdditionalAbyssDirection), Vector3.down, out _Hit, 2.6f, mask))
                                {
                                    ShowLine((_CurrentPosition + _CheckingDirection * i + _AdditionalAbyssDirection), (_CurrentPosition + _CheckingDirection * i + _AdditionalAbyssDirection + Vector3.down));
                                    if (LogWriting)
                                        Log.Notice(scr,"Found the baypass around the abyss at " + _Hit.point);
                                    return _Hit.point;
                                }
                            
                            else
                            {
                                if (Physics.Raycast((_CurrentPosition + _CheckingDirection * i + _AbyssDirection), Vector3.down, out _Hit, 2.6f, mask))
                                {
                                    ShowLine((_CurrentPosition + _CheckingDirection * i + _AbyssDirection), (_CurrentPosition + _CheckingDirection * i + _AbyssDirection + Vector3.down));
                                    if (LogWriting)
                                        Log.Notice(scr,"Found the baypass around the Abyss at " + _Hit.point);
                                    return _Hit.point;
                                }
                                else
                                    if (LogWriting)
                                        Log.Notice(scr,"There is some shit in method of complex route building... need check");
                            }
                        }
                    }
                }
                else
                {
                    if (LogWriting)
                        Log.Notice(scr,"Abyss found");
                    _AbyssFlg = true;
                }
            }
            else
            {
                if (LogWriting)
                    Log.Notice(scr,"Let found");
                _LetFlg = true;
            }
        }
        if (LogWriting)
            Log.Notice(scr,"No baypass for the let. Max walking around distance reached");
        return Vector3.positiveInfinity;
    }
    private void ShowLine(Vector3 _Start, Vector3 _Finish, bool _DeleteFlg = true)
    {
        if (_ShowLines)
        {
            GameObject _Line = new GameObject();
            LineRenderer _LR = _Line.AddComponent<LineRenderer>();
            _LR.SetWidth(0.15f, 0.05f);
            _LR.SetPosition(0, _Start);
            _LR.SetPosition(1, _Finish);
            if (_DeleteFlg)
                Destroy(_Line, 2f);
        }
    }
    private void ShowTheWay(List<Vector3> _Route, bool _DeleteFlg = false)
    {
        if (_ShowLines)
        {
            for (int i = 0; i < _Route.Count - 1; i += 1)
            {
                if (_DeleteFlg)
                    Destroy(Instantiate(_Pointer, _Route[i], Quaternion.identity), 2f);
                else
                    Instantiate(_Pointer, _Route[i], Quaternion.identity);
                ShowLine(_Route[i], _Route[i + 1], _DeleteFlg);
            }
            if (_DeleteFlg)
                Destroy(Instantiate(_Pointer, _Route[_Route.Count - 1], Quaternion.identity), 2f);
            else
                Instantiate(_Pointer, _Route[_Route.Count - 1], Quaternion.identity);
        }
    }
    private Vector2 NextPosition(Vector2 _StartPosition, Vector2 _TargetPosition, Vector2 _CurrentPosition2D)
    {
        if (LogWriting)
            Log.Notice(scr,"Current position is" + _CurrentPosition2D + "Target position is " + _TargetPosition);
        float _MinDistanceToTarget = Mathf.Infinity, _DistanceToTarget = 0;
        int _MinI = 2, _MinJ = 2;
        for (int i = -1; i < 2; i = i+1)
        {
            for (int j = -1; j < 2; j= j+1)
            {
                if ((i != 0) || (j != 0))
                {
                    _DistanceToTarget = Mathf.Sqrt(Mathf.Pow((_TargetPosition.x - (_CurrentPosition2D.x + i)), 2) + Mathf.Pow((_TargetPosition.y - (_CurrentPosition2D.y + j)), 2));
                    if (_DistanceToTarget < _MinDistanceToTarget)
                    {
                        _MinDistanceToTarget = _DistanceToTarget;
                        _MinI = i;
                        _MinJ = j;
                    }
                }
            }
        }
        _CurrentPosition2D.x = _CurrentPosition2D.x + _MinI;
        _CurrentPosition2D.y = _CurrentPosition2D.y + _MinJ;
        if (LogWriting)
            Log.Notice(scr,"Next position is" + _CurrentPosition2D);
        return _CurrentPosition2D;
    }
    private Vector3[] DotsForRaycast(Vector3 _Position, Vector3 _Aim) //Убрать левую и правую точки из вывода. Они тут не нужны
    {
        //Log.Notice(scr,"Dots for raycast input. Position: " + _Position + " Aim: " + _Aim);
        Vector3[] _Result = new Vector3[6];
        //_Result[0] - нормализованное направление Raycast-ов
        //_Result[1] - правая точка
        //_Result[2] - левая точка
        //_Result[3] - Координаты вектора, направленного вправо от нормали поверхности, в которую попали правым лучом (используем в качестве направления для обхода препядствия справа)
        //_Result[4] - Координаты вектора, направленного влево от нормали поверхности, в которую попали левым лучом (используем в качестве направления для обхода препядствия слева)
        //_Result[5] - Ненормализованные двумерные координаты направления препядствия
        _Result[0] = new Vector3(_Aim.x - _Position.x, _Aim.y - _Position.y,_Aim.z - _Position.z);
        _Result[5] = _Result[0];
        _Result[5].y = 0;
        _Result[0].Normalize();
        //Log.Notice(scr,"Ray direction is " + _Result[0]);
        _Result[1]= _Position + 0.2f*(new Vector3(_Result[0].z,0.05f,-1*(_Result[0].x)));
        //Instantiate(_Pointer, _Result[1], Quaternion.identity);
        _Result[2]= _Position + 0.2f*(new Vector3(-1*(_Result[0].z),0.05f,_Result[0].x));
        //Instantiate(_Pointer, _Result[2], Quaternion.identity);
        //Log.Notice(scr,"Dots for raycast output. Direction: " + _Result[0] + " Right position: " + _Result[1] + " Left position:" + _Result[2]);
        Physics.Raycast(_Result[1], _Result[0], out _Hit, mask); // Стреляем правым лучом
        //ShowLine(_Dots[1], _Dots[1] + _Dots[0]);
        _Result[3] = Functions.LeftVector(_Hit.normal);

        Physics.Raycast(_Result[2], _Result[0], out _Hit, mask); // Стреляем левым лучом
        //ShowLine(_Dots[2], _Dots[2] + _Dots[0]);

        _Result[4] = Functions.RightVector(_Hit.normal);

        return _Result;
    }
    IEnumerator Walk()
    {
        if (_Walk)
        {
            GetComponent<Animator>().SetBool("Walking", true);
            Log.Notice(scr, "Go GO GOOOOO!!!");
            for (int i = 1; i <= _Route.Count - 1; i += 1)
            {
                //Log.Notice(scr,"Walking iteration " + i);
                yield return StartCoroutine("WalkToPoint", new Vector3[] { _Route[i - 1], _Route[i] });
                if (Vector3.Distance(_Transform.position, _Target) <= _MinDistance && Vector3.Distance(_Transform.position, _Target) >= _MaxDistance)
                {
                    if (LogWriting)
                        Log.Notice(scr,"Reached distance extremmum");
                    _MinDistance = 0;
                    _MaxDistance = int.MaxValue;
                    break;
                }
            }
            GetComponent<Animator>().SetBool("Walking", false);
            _IsWalking = false;
            Log.Notice(scr, "Finished");
        }
    }
    IEnumerator WalkToPoint(Vector3[] _Points)
    {
        _Points[1].y += 1;
        Vector3 _Direcrion /* = _Points[1] - transform.position*/;
        //_Direcrion.Normalize();
        //Log.Notice(scr,"Going from " + _Points[0] + " to " + _Points[1]);
        _Transform.rotation = Quaternion.LookRotation(Functions.LeftVector((_Points[1] - Vector3.up * _Points[1].y + _Transform.position.y * Vector3.up) - _Transform.position), Vector3.up);
        ShowLine(_Transform.position,_Points[1] - Vector3.up * _Points[1].y + _Transform.position.y * Vector3.up);

        _Direcrion = _Points[1] - transform.position;

        Log.Notice(scrw, "Start walking from " + _Transform.position + " to " + _Points[1] + ". Computed direction: " + _Direcrion);
        while(!(_Transform.position == _Points[1]))
        {
            Log.Notice(scrw, "Walking iteration. Current position " + _Transform.position);
            if (Vector3.Magnitude(_Points[1] - transform.position) > Vector3.Magnitude(Vector3.Normalize(_Direcrion) * _WalkSpeed * Time.deltaTime))
            {
                //Vector3 _rvector = Vector3.RotateTowards(_Transform.right, (_Points[1] - Vector3.up * _Points[1].y + _Transform.position.y * Vector3.up) - _Transform.position, 0.3f, 0.0f);
                //ShowLine(_Transform.position, _Transform.position + _rvector);
                //_Transform.rotation = Quaternion.LookRotation(_rvector);
                //_Transform.Translate(_Transform.forward * _WalkSpeed * Time.deltaTime);
                _Transform.position += Vector3.Normalize(_Direcrion) * _WalkSpeed * Time.deltaTime;//Двигаемся
                //_Transform.Translate(Vector3.Normalize(_Direcrion) * _WalkSpeed * Time.deltaTime);
            }
            else
                _Transform.position = _Points[1];//Если "Перелет", то устанавливаем объект в целевую точку
            yield return new WaitForEndOfFrame();
        }
            yield return new WaitForFixedUpdate();
        Log.Notice(scrw,"Current ghole reached");
    }
}