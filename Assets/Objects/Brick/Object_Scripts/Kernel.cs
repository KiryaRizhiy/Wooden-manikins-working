using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kernel : MonoBehaviour {

    private string scr = "Kernel";
    private List<Brick[,]> Layers = new List<Brick[,]>();
    public bool buildingOver { get; private set; }
    public Brick CentralVisibleBrick
    {
        get
        {
            return Layers.Find(x => x[0, 0].gameObject.activeInHierarchy)[0,0];
        }
    }
    public Vector2 KernelCoordinates
    {
        get
        {
            return new Vector2(gameObject.transform.position.x / Settings.KernelSize, gameObject.transform.position.z / Settings.KernelSize);
        }
    }

    void Start()
    {
        buildingOver = false;
        for (int h = 0; h < Settings.MapHeight; h++)
        {
            Layers.Add(new Brick[Settings.KernelSize, Settings.KernelSize]);
        }
        StartCoroutine(MapGenerator.LandscapeGenerator(this));
    }

    public void Built()
    {
        Debug.Log(gameObject.name + " built");
        buildingOver = true;
    }

    public Brick GetKernelBrick(Vector3 Coordinates)
    {
        Vector3 _coords = Coordinates - gameObject.transform.position;
        Log.Notice(scr,gameObject.name + " received request for block " + Coordinates);
        //if (_coords.y < 0 || _coords.y > Layers.Count - 1)
        //    return null;
        //if (_coords.x < 0 || _coords.x >= Settings.KernelSize-1)
        //    return null;
        //if (_coords.z < 0 || _coords.z >= Settings.KernelSize-1)
        //    return null;
        return Layers[Mathf.RoundToInt(_coords.y)][Mathf.RoundToInt(_coords.x), Mathf.RoundToInt(_coords.z)];
    }
    public bool KernelBrickExists(Vector3 Coordinates)
    {
        Vector3 _coords = Coordinates - gameObject.transform.position;
        if (_coords.y < 0 || _coords.y > Layers.Count - 1)
            return false;
        if (_coords.x < 0 || _coords.x > Settings.KernelSize-1)
            return false;
        if (_coords.z < 0 || _coords.z > Settings.KernelSize-1)
            return false;
        return Layers[Mathf.RoundToInt(_coords.y)][Mathf.RoundToInt(_coords.x), Mathf.RoundToInt(_coords.z)] != null;
    }
    public Brick AddBrick(int x, int y, int z, bool Visibility, Resource Resource = null)
    {
        if(x>=Settings.KernelSize||z>=Settings.KernelSize||y>=Settings.MapHeight)
        {
            Log.Warning(scr,"Invalid attempt to add brick to kernel " + gameObject.name + ". Invalid coordinates are: " + x + ":" + y + ":" +z);
            return null;
        }
        Layers[y][x, z] = Instantiate(Settings.BrickPrefab, new Vector3(x, y, z) + gameObject.transform.position, Quaternion.identity).GetComponent<Brick>();
        Layers[y][x, z].gameObject.transform.SetParent(gameObject.transform);        
        if (Resource != null)
            Layers[y][x, z].Resource = Resource;
        Layers[y][x, z].gameObject.name = "Brick " + Layers[y][x, z].Resource.Name + " " + x + ":" + y + ":" + z;
        Layers[y][x, z].gameObject.SetActive(Visibility);
        //if (Visibility && (x%(Settings.KernelSize-1)==0||z%(Settings.KernelSize-1)==0))//Если требуется сделать видимым объект на границе Kernel-ей
        //    Layers[y][x, z].StartCoroutine("CorrectVisibilityAroundWithDelay", 160); //Прогоняем его видимость по тяжелому алгоритму//Дерьмо какое то
        return Layers[y][x, z];
    }
    public Brick AddBrick(Vector3 Coordinates, Resource Resource)
    {
        Vector3 _coords = Coordinates - gameObject.transform.position;
        int x = Mathf.RoundToInt(_coords.x), y = Mathf.RoundToInt(_coords.y), z = Mathf.RoundToInt(_coords.z);
        if (x >= Settings.KernelSize || z >= Settings.KernelSize || y >= Settings.MapHeight)
        {
            Log.Warning(scr, "Invalid attempt to add brick to kernel " + gameObject.name + ". Invalid coordinates are: " + Coordinates);
            return null;
        }
        Layers[y][x, z] = Instantiate(Settings.BrickPrefab, Coordinates, Quaternion.identity).GetComponent<Brick>();
        Layers[y][x, z].gameObject.transform.SetParent(gameObject.transform);
        Layers[y][x, z].gameObject.name = "Brick " + x + ":" + y + ":" + z;
        Layers[y][x, z].Resource = Resource;
        Layers[y][x, z].CorrectVisibilityAround();
        return Layers[y][x, z];
    }
    public void RemoveKernelBrick(Vector3 Coordinates)
    {
        if (KernelBrickExists(Coordinates))
        {            
            Vector3 _coords = Coordinates - gameObject.transform.position;
            int x = Mathf.RoundToInt(_coords.x), y = Mathf.RoundToInt(_coords.y), z = Mathf.RoundToInt(_coords.z);
            Brick _tmpb = Layers[y][x, z];
            Layers[y][x, z] = null;
            _tmpb.CorrectVisibilityAround();
            Destroy(_tmpb.gameObject);
        }
    }
}