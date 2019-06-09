using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sack {

    public GameObject _Carrier { get; private set; }

    private float _OccupedSpace;
    private float _Capacity = 100f;
    public List<SackCell> _Resources { get; private set; }
    private string scr = "Sack";

    void Start()
    {
        _Resources = new List<SackCell>();
    }
    public Sack(GameObject Carrier, byte CarrierType = 0)
    {
        _Resources = new List<SackCell>();
        _OccupedSpace = 0;
        _Capacity = 100f;
        _Carrier = Carrier;
        if (CarrierType != 0)
            _Capacity = 1000f; //Научиться считать в зависимости от площади зоны
        Log.Notice(scr,"Sack for " + Carrier + " creacted");
    }
    public bool Exists()
    {
        return true;
    }
    public byte Input(Resource Res, int Cnt)
    {
        if (!HasEnoughSpaceFor(Res, Cnt))
            return 1;
        if (_Resources.Count > 0)
            if (_Resources.Find(x => x.Resource.Type == Res.Type) != null)
                _Resources.Find(x => x.Resource.Type == Res.Type).Count += Cnt;
            else
                _Resources.Add(new SackCell(Res, Cnt));
        else
            _Resources.Add(new SackCell(Res, Cnt));
        _OccupedSpace += Cnt*Res.Volume;
        //Log.Notice(scr,"Input " + Res.Name + " successfull");
        LogAllSack();
        return 0;
    }
/*
 * 0 - ок
 * 1 - нет свободного места
 */
    public byte Input(List<Resource> _ResourceTypes, List<int> _ResourceCounts)
    {
        if (!HasEnoughSpaceFor(_ResourceTypes,_ResourceCounts))
           return 1;
        for (int i = 0; i < _ResourceTypes.Count; i++)
        {
            Input(_ResourceTypes[i], _ResourceCounts[i]);
        }
        return 0;
    }
    public byte Output(Resource Res, int Count=1)
    {
        if (HasAResource(Res, Count))
        {
            if (_Resources.Find(x => x.Resource == Res).Count == Count)
                _Resources.RemoveAll(x => x.Resource == Res);
            else
                _Resources.Find(x => x.Resource == Res).Count -= Count;
            return 0;
        }
        return 1;
    }
    public void Clear()
    {
        _Resources.Clear();
        Log.Notice(scr, _Carrier + "' sack cleared");
    }
/*
* 0 - Удаление произведено
* 1 - Нет ресурсов
*/
    public bool HasAResource(Resource Res, int Count = 1)
    {
        return _Resources.Exists(x => (x.Resource.Type == Res.Type) && (x.Count >= Count));
    }
    public bool HasEnoughSpaceFor(Resource Res, int Count)
    {
        return _OccupedSpace + Count * Res.Volume < _Capacity;
    }
    public bool HasEnoughSpaceFor(List<ResourceEnumerator> ResCntList)
    {
        List<Resource> _ResList = new List<Resource>();
        List<int> _CntList = new List<int>();
        foreach(ResourceEnumerator _rc in ResCntList)
        {
            _ResList.Add(_rc.Resource);
            _CntList.Add(_rc.Count);
            Log.Notice(scr,_Carrier + "Checking free space for: " + _rc.Resource.Name + " of count:" + _rc.Count + ". Resource volume is " + _rc.Resource.Volume);
        }
        return HasEnoughSpaceFor(_ResList,_CntList);
    }
    public bool HasEnoughSpaceFor(List<Resource> ResList, List<int> CntList)
    {
        float _SpaceRequired = 0; //Применить GroupBy,GroupJoin
        for (int i = 0; i < ResList.Count; i++)
        {
            _SpaceRequired += CntList[i]*ResList[i].Volume;
        }
        Log.Notice(scr, _Carrier + "Checking free space for: " + _SpaceRequired + ". Occuped: " + _OccupedSpace + ". Sack capacity: " + _Capacity);
        if (_OccupedSpace + _SpaceRequired > _Capacity)
            return false;
        else
            return true;
    }
	// Use this for initialization
	
	// Update is called once per frame
    public void LogAllSack()
    {
        if (_Resources.Count > 0)
            foreach (SackCell _sc in _Resources)
                Log.Notice(scr,_Carrier + "  " + _sc.Resource.Name + " : " + _sc.Count);
        else
            Log.Notice(scr, _Carrier + "This sack is emty yet");
    }   
}
public class SackCell
{
    public Resource Resource;
    public int Count;
    public NameValue Description { get { return new NameValue(Resource.Name,Count.ToString()); } }
    public SackCell(Resource Res, int Cnt)
    {
        Resource = Res;
        Count = Cnt;
    }
}