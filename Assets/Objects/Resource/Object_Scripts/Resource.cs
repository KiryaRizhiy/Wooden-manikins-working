using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource{

    // Внешние поля
    public ushort Type
    {
        get 
        {
            return _Type; 
        }
        set
        {
            if (_Type == 0)
                _Type = value;
        }
    }
    public string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            if (_Name == null)
                _Name = value;
        }
    }
    public Material Material
    {
        get
        {
            return _Material;
        }
        set
        {
            if (_Material == null)
                _Material = value;
        }
    }
    public Sprite Sprite
    {
        get
        {
            return _Sprite;
        }
        set
        {
            if (_Sprite == null)
                _Sprite = value;
        }
    }
    public ushort MachineToCreate
    {
        get
        {
            return _MachineToCreate;
        }
        set
        {
            if (_MachineToCreate == 0)
                _MachineToCreate = value;
        }
    }
    public List<ResourceDetail> Details
    {
        get
        {
            return _Details;
        }
        set
        {
            if (_Details == null)
                _Details = value;
        }
    }
    public List<ResourceSubType> SubTypes
    {
        get
        {
            return _SubTypes;
        }
        set
        {
            if (_SubTypes.Count == 0)
                _SubTypes = value;
        }
    }
    public float TimeToCreate
    {
        get
        {
            return _TimeToCreate;
        }
        set
        {
            if (_TimeToCreate == 0.1f)
                _TimeToCreate = value;
        }
    }
    public float TimeToMine
    {
        get
        {
            return _TimeToMine;
        }
        set
        {
            if (_TimeToMine == 0.1f)
                _TimeToMine = value;
        }
    }
    public float TimeToBuild
    {
        get
        {
            return _TimeToBuild;
        }
        set
        {
            if (_TimeToBuild == 0.1f)
                _TimeToBuild = value;
        }
    }
    public byte Volume
    {
        get
        {
            return _Volume;
        }
        set
        {
            if (_Volume == 1)
                _Volume = value;
        }
    }
    public bool IsResourceForWorldBuilding;
    public ushort UpperBoarderPercent, LowerBoarderPercent;
    public DepositFigure Figure;
    public bool MayBeUsedInStructures;
    public ResourceTypeClassification Classification;

    //Public methods

    // Внутренние переменные
    private ushort _Type,_MachineToCreate;
    private string _Name;
    private Material _Material;
    private Sprite _Sprite;
    private List<ResourceDetail> _Details;
    private List<ResourceSubType> _SubTypes; // Подтип ресурса. Каждый элемент _ResourceSubType состоит из пары Тип ресурса - Материал ресурса
    private float _TimeToCreate = 0.1f, _TimeToMine = 0.1f, _TimeToBuild = 0.1f;
    private byte _Volume = 1;
}
//Вспомогательные классы
public class ResourceSubType
{
    public ushort Type;
    public string MaterialName;
    public ResourceSubType(ushort T, string N)
    {
        Type = T;
        MaterialName = N;
    }
}
public class ResourceDetail
{
    public ushort Type;
    public short Count;
    public short CountWithMinus
    {
        get
        {
            return (short)(-1 * Count);
        }
    }
    public ResourceDetail(ushort T, short C)
    {
        Type = T;
        Count = C;
    }
}
public enum ResourceTypeClassification { Resource, DiggingTool, CraftingTool, PunchingTool};