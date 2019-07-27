using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Map {

    private static string scr = "Map";
    private static bool _mapBuilt
    {
        get
        {
            foreach (Kernel _k in Kernels)
                if (!_k.buildingOver)
                    return false;
            return true;
        }
    }
    public static List<Kernel> Kernels = new List<Kernel>();
    private static NavMeshSurface Navigation
    {
        get
        {
            if (Kernels[0].CentralVisibleBrick.GetComponent<NavMeshSurface>() != null)
                return Kernels[0].CentralVisibleBrick.GetComponent<NavMeshSurface>();
            else
            {
                return Kernels[0].CentralVisibleBrick.gameObject.AddComponent<NavMeshSurface>();
            }
        }
    }

    public static IEnumerator InitializeMap()
    {
        for (int i = -Settings.BasicMapRadius; i <= Settings.BasicMapRadius; i++)
            for (int j = -Settings.BasicMapRadius; j <= Settings.BasicMapRadius; j++)
            {
                AddKernel(new Vector2(i, j));
                yield return new WaitForEndOfFrame();
            }
        while (!_mapBuilt)
            yield return new WaitForEndOfFrame();
        Navigation.BuildNavMesh();
        Debug.Log("Map building is over");
    }

    public static void AddKernel(Vector3 Coordinates)
    {
        if (!CoordinatesIntoTheMap(Coordinates))
            AddKernel(ToKernelCoordinates(Coordinates));
        else
            Log.Warning(scr, "Trying to add already existing kernel! Coordinates :" + Coordinates + ", kernel: " + ToKernelCoordinates(Coordinates));
    }
    public static Brick GetBrick(Vector3 Coordinates)
    {
        if (BrickExists(Coordinates))
            return Kernels.Find(x => x.KernelBrickExists(Coordinates)).GetKernelBrick(Coordinates);
        else
            return null;
    }
    public static bool BrickExists(Vector3 Coordinates)
    {
        if (!CoordinatesIntoTheMap(Coordinates))
            return false;
         return GetKernel(Coordinates).KernelBrickExists(Coordinates);
    }
    public static bool CoordinatesIntoTheMap(Vector3 Coordinates)
    {
        return GetKernel(Coordinates) != null;
    }
    public static Brick AddBrick(Vector3 Coordinates, Resource Resource)
    {
        return GetKernel(Coordinates).AddBrick(Coordinates, Resource);
    }
    public static void RemoveBrick(Vector3 Coordinates)
    {
        ZoneController.RemoveComponent(Map.GetBrick(Coordinates).gameObject);
        GetKernel(Coordinates).RemoveKernelBrick(Coordinates);
    }
	private static void AddKernel(Vector2 KernelCoordinates)
    {
        Vector3 BasicObjectCoordinates = new Vector3(KernelCoordinates.x * Settings.KernelSize, 0, KernelCoordinates.y * Settings.KernelSize);
        GameObject _obj = new GameObject();
        _obj.transform.position = BasicObjectCoordinates;
        Resources.ExtendDepositsGrid(KernelCoordinates);
        _obj.name = "Kernel " + KernelCoordinates.x + ":" + KernelCoordinates.y;
        Kernels.Add(_obj.AddComponent<Kernel>());
    }
    private static Kernel GetKernel(Vector3 Coordinates)
    {
        Vector2 _kernelcoords = ToKernelCoordinates(Coordinates);
        if (Kernels.Find(x => x.KernelCoordinates == _kernelcoords) == null)
        {
            Log.Notice(scr, "No kernel, containing block " + Coordinates + ", found. Kernel " + _kernelcoords + " required");
            return null;
        }
        else
            return Kernels.Find(x => x.KernelCoordinates == _kernelcoords);
    }
    private static Vector2 ToKernelCoordinates(Vector3 V3Coordinates)
    {
        Vector2 _kernelCoords;
        _kernelCoords.x = Mathf.RoundToInt((V3Coordinates.x - Settings.KernelSize / 2) / (Settings.KernelSize));
        _kernelCoords.y = Mathf.RoundToInt((V3Coordinates.z - Settings.KernelSize / 2) / (Settings.KernelSize));
        return _kernelCoords;
    }
}