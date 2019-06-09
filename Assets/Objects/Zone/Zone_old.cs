using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Zone_old: MonoBehaviour {

    //public ushort ID
    //{
    //    get
    //    {
    //        return _ID;
    //    }
    //    set
    //    {
    //        if (_ID == 0)
    //            _ID = value;
    //    }
    //}
    //public string Name
    //{
    //    get
    //    {
    //        return _Name;
    //    }
    //    set
    //    {
    //        if (_Name == null)
    //            _Name = value;
    //    }
    //}
    //public byte Type
    //{
    //    get
    //    {
    //        return _ZoneType;
    //    }
    //    set
    //    {
    //        if (_ZoneType == 0)
    //        {
    //            switch (value)
    //            {
    //                case 0:
    //                    TextType = "ZoneTypeIsMissing";
    //                    break;
    //                case 1:
    //                    TextType = "Storage";
    //                    break;
    //                case 2:
    //                    TextType = "Field";
    //                    break;
    //                case 3:
    //                    TextType = "Pasture";
    //                    break;
    //                case 4:
    //                    TextType = "Mine";
    //                    break;
    //                default:
    //                    TextType = "Zone type " + value.ToString() + " undefined";
    //                    break;
    //            }
    //            _ZoneType = value;
    //            Debug.Log("Zone type set");
    //        }
    //        else
    //            Debug.Log("Zone type already set");
    //    }
    //}
    //public string TextType { get; private set; }

    //private ushort _ID = 0;    
    //private string _Name = null;    
    //internal List<ZoneComponent> _Bricks = new List<ZoneComponent>();
    //private byte _ZoneType = 0;
    ////1 - Склад
    ////2 - Поле
    ////3 - Пастбище
    ////4 - Зона добычи

    ////Interactions
    //public virtual GameObject GetTarget()
    //{
    //    return _Bricks[Random.Range(0, _Bricks.Count)]._Component;
    //}
    //public void AddBrick(GameObject _b)
    //{
    //    _Bricks.Add(new ZoneComponent(_b));
    //    //Debug.Log("Successfull check. Brick added to zone " + Name);
    //}
    //public void RemoveBrick(GameObject _b)
    //{
    //    _Bricks.RemoveAll(x => x._Component == _b);
    //}
    
    ////Visual
    //public void HighlightAll()
    //{
    //    foreach (ZoneComponent _obj in _Bricks)
    //        GetComponent<Highlighter>().HighLight(_obj._Component);
    //}
    //public void UnHighlightAll()
    //{
    //    foreach (ZoneComponent _obj in _Bricks)
    //        GetComponent<Highlighter>().UnHighLight(_obj._Component);
    //}
    //public void ShowZone()
    //{
    //    foreach (ZoneComponent _obj in _Bricks)
    //        Debug.Log(_obj._Component.name);
    //}
	
    //// Info storage
    //public class Storage:Zone
    //{
    //    public byte RepositoryType;
    //    public Sack Sack;
    //}
    //public class Field:Zone
    //{
    //}
    //public class Pasture:Zone
    //{
    //}
    //public class Mine:Zone
    //{
    //    public override GameObject GetTarget()
    //    {
    //        foreach (ZoneComponent _c in _Bricks)
    //        {
    //            if (!(_c._Busy))
    //            {
    //                _c._Busy = true;
    //                return _c._Component;
    //            }
    //        }
    //        Debug.Log(TextType + "'" + Name + "' has no free components");
    //        return null;
    //    }

    //}
    //public class ZoneComponent
    //{
    //    public GameObject _Component;
    //    public bool _Busy;
    //    public ZoneComponent(GameObject _o)
    //    {
    //        _Component = _o;
    //        _Busy = false;
    //    }
    //}
}
