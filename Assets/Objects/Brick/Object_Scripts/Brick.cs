using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick: MonoBehaviour {

    //private int[] Coordinates = new int[3];
    public List<Brick> Neighbours
    {
        get
        {
            List<Brick> _res = new List<Brick>();
            foreach (Vector3 _shift in Settings.Vector3Neighbours)
                if (Map.BrickExists(gameObject.transform.position + _shift))
                    _res.Add(Map.GetBrick(gameObject.transform.position + _shift));
            return _res;
        }
    }
    public bool HasNeighbours
    {
        get
        {
            foreach (Vector3 _shift in Settings.Vector3Neighbours)
                if (Map.BrickExists(gameObject.transform.position + _shift))
                    return true;
            return false;
        }
    }
    public byte NeighboursCount
    {
        get
        {
            byte _res = 0;
            foreach (Vector3 _shift in Settings.Vector3Neighbours)
                if (Map.BrickExists(gameObject.transform.position + _shift))
                    _res++;
            return _res;
        }
    }
    //public Vector3 _V3Coordinates { get; private set; }
    //public bool Visible = true;
    public ushort _ZoneId {get; private set;}
    public Resource Resource {
        get
        {
            return Resources.GetResource(_resourceType);
        }
        set
        {
            _resourceType = value.Type;
            GetComponent<MeshRenderer>().material = value.Material;
        }
    }

    [SerializeField]
    private ushort _resourceType;

    public void SetCoordinates(int x,int y, int z)
    {        
        //Coordinates[0] = x;
        //Coordinates[1] = y;
        //Coordinates[2] = z;
        //_V3Coordinates = new Vector3(x,y,z);
    }
    public int[] GetCoordinates()
    {
        return null;
    }
    public void SetResource(Resource _r)//Отключить к херам
    {
        Resource = _r;
        //Debug.Log(transform.name + " resource set to " + _r.Name);
    }
    public void Show()//Отключить к херам
    {
        GetComponent<MeshRenderer>().material = Resource.Material;
    }
    public void Remove()
    {
        //GetComponentInParent<Brick_intreraction_script>().DeleteBrick(Coordinates[0], Coordinates[1], Coordinates[2]);
    }
    public void SetZoneId(ushort _i = 0)
    {
        _ZoneId = _i;
    }
    public void CorrectVisibility()
    {
        if (HasNeighbours)
        {
            if (NeighboursCount == 6)
            {
                gameObject.SetActive(false);
                Log.Notice("Brick", "Kernel " + gameObject.transform.parent.name + ", " + gameObject.name + " has 6 neghbours. Deactivating");
                return;
            }
            if (NeighboursCount == 5 && gameObject.transform.position.y == 0)//Если у блока все соседи на месте, или у нижнего блока 5 соседей, то деактивируем
            {
                gameObject.SetActive(false);
                Log.Notice("Brick", "Kernel " + gameObject.transform.parent.name + ", " + gameObject.name + " has 5 neghbours. Deactivating");
                return;
            }
            Log.Notice("Brick", "Kernel " + gameObject.transform.parent.name + ", " + gameObject.name + " has " + NeighboursCount + " neighbours. Its not enough neghbours to deactivate it");
            gameObject.SetActive(true);
            return;
        }
        else
        {
            Log.Notice("Brick", "Kernel " + gameObject.transform.parent.name + ", " + gameObject.name + " has no neghbours and will not be deactivated");
            gameObject.SetActive(true);
        }
    }
    public void CorrectVisibilityAround()
    {
        Log.Notice("Brick", "Kernel " + gameObject.transform.parent.name + ", " + gameObject.name + " Correcting visibility around " + gameObject.name + ". Coordinates: " + gameObject.transform.position);
        if (HasNeighbours)
            foreach (Brick _b in Neighbours)
            {
                _b.CorrectVisibility();
            }
        else
            Log.Notice("Brick", "Kernel " + gameObject.transform.parent.name + ", " + gameObject.name + " has no neighbours");
        if (gameObject.transform.position.y == 0)
            CorrectVisibility();
    }
    public IEnumerator CorrectVisibilityAroundWithDelay(float Delay = 0)
    {
        yield return new WaitForSeconds(Delay);
        CorrectVisibilityAround();
    }
}