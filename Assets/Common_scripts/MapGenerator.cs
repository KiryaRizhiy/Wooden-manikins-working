using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator {

    private static string scr = "MapGenerator";

    //public static void BuildKernelLandscape(Kernel Kernel)
    //{
    //    float[,] HeightMatrix = new float[Settings.KernelSize, Settings.KernelSize];
    //    float PerlinNoizeResult, _LocalMaxHeight = 0f;
    //    for (int i = 0; i < Settings.KernelSize; i++) // случайно заполняем матрицу высот
    //        for (int j = 0; j < Settings.KernelSize; j++)
    //        {
    //            PerlinNoizeResult = Mathf.PerlinNoise((i + Kernel.KernelCoordinates.x * Settings.KernelSize) / (Settings.MapLandscapeScale * Settings.KernelSize), (j + Kernel.KernelCoordinates.y * Settings.KernelSize) / (Settings.MapLandscapeScale * Settings.KernelSize)) * Settings.MapHeight;
    //            Log.Notice(scr, "For coordinates " + (i + Kernel.KernelCoordinates.x * Settings.KernelSize) + ":" + (j + Kernel.KernelCoordinates.y * Settings.KernelSize) + " computed height " + PerlinNoizeResult);
    //            if (PerlinNoizeResult > _LocalMaxHeight)
    //                _LocalMaxHeight = PerlinNoizeResult;
    //            HeightMatrix[i, j] = PerlinNoizeResult;
    //        }
    //    List<Brick[,]> Result = new List<Brick[,]>();
    //    Vector3 KernelShift = Kernel.gameObject.transform.position;

    //    for (int h = 0; h < Settings.WorldHeight; h++)//Создаем объекты        
    //        for (int w = 0; w < Settings.KernelSize; w++)
    //            for (int l = 0; l < Settings.KernelSize; l++)
    //                if (HeightMatrix[w, l] > h)
    //                {
    //                    if (Settings.MapSoilLevel - (Settings.MapHeight - h) + Random.Range(-2, 2) > 0) //Если до уровня почвы еще не добрались
    //                        Kernel.AddBrick(w, h, l, false, Links.Resources.GetResource(2));//Создаем камень
    //                    else//Иначе
    //                        Kernel.AddBrick(w, h, l, false, Links.Resources.GetResource(1));//Создаем землю
    //                }
    //}
    public static IEnumerator LandscapeGenerator(Kernel _k)
    {
        float[,] HeightMatrix = new float[Settings.KernelSize, Settings.KernelSize];
        float PerlinNoizeResult, _LocalMaxHeight = 0;
        for (int i = 0; i < Settings.KernelSize; i++) // случайно заполняем матрицу высот
            for (int j = 0; j < Settings.KernelSize; j++)
            {
                PerlinNoizeResult = 1 + Mathf.PerlinNoise(Settings.NoizeShift + (i + _k.KernelCoordinates.x * Settings.KernelSize) / (Settings.MapLandscapeScale * Settings.KernelSize), Settings.NoizeShift + (j + _k.KernelCoordinates.y * Settings.KernelSize) / (Settings.MapLandscapeScale * Settings.KernelSize)) * (Settings.MapHeight - 1);
                Log.Notice(scr, "For coordinates " + (i + _k.KernelCoordinates.x * Settings.KernelSize) + ":" + (j + _k.KernelCoordinates.y * Settings.KernelSize) + " computed height " + PerlinNoizeResult);
                if (PerlinNoizeResult > _LocalMaxHeight)
                    _LocalMaxHeight = PerlinNoizeResult;
                HeightMatrix[i, j] = PerlinNoizeResult;
            }
        //yield return new WaitForEndOfFrame();
        //List<Brick[,]> Result = new List<Brick[,]>();
        //Vector3 KernelShift = _k.gameObject.transform.position;
        bool _visibility = true;

        for (int h = Mathf.RoundToInt(_LocalMaxHeight); h >=0; h--)//Создаем объекты
        {
            for (int w = 0; w < Settings.KernelSize; w++)
            {
                for (int l = 0; l < Settings.KernelSize; l++)
                    if (HeightMatrix[w, l] > h)
                    {
                        if (h == 0)//Для нижних блоков
                            _visibility = !(Settings.Vector3Neighbours.FindAll(nb =>
                                (nb.x + w) >= 0
                                && (nb.z + l) >= 0
                                && (nb.y + h) >= 0
                                && (nb.x + w) < Settings.KernelSize
                                && (nb.z + l) < Settings.KernelSize
                                && (nb.y + h) < Settings.KernelSize).FindAll(subnb =>
                                    HeightMatrix[Mathf.RoundToInt(subnb.x+w), Mathf.RoundToInt(subnb.z+l)] > subnb.y + h).Count == 5);//Считаем, сколько соседей будет у текущего блока, не выходящих за границы Kernel-я
                        if (h>0)
                            _visibility = !(Settings.Vector3Neighbours.FindAll(nb =>
                                (nb.x + w) >= 0
                                && (nb.z + l) >= 0
                                && (nb.y + h) >= 0
                                && (nb.x + w) < Settings.KernelSize
                                && (nb.z + l) < Settings.KernelSize
                                && (nb.y + h) < Settings.KernelSize).FindAll(subnb =>
                                    HeightMatrix[Mathf.RoundToInt(subnb.x + w), Mathf.RoundToInt(subnb.z + l)] > subnb.y + h).Count == 6);
                        GameObject _b;
                        ////Времянка для теста
                        //Resource _r = Resources.GetResourceInPoint(new Vector3(w, h, l));
                        //if (_r.Type != 2)
                        //    _b = _k.AddBrick(w, h, l, _visibility, _r).gameObject;
                        //else
                        //    _b = new GameObject();
                        //if (Resources.GetResourceInPoint(new Vector3(w, h, l)).Type != 2) // Времянка. Подсвечиваем ресурсы
                        //    _visibility = true;
                        _b = _k.AddBrick(w, h, l, (_visibility || /*Времянка. Подсвечиваем ресурсы */ Resources.GetResourceInPoint(new Vector3(w, h, l)).Type != 2), Resources.GetResourceInPoint(new Vector3(w, h, l))).gameObject;                        
                        if (_visibility && w > 0 && h > 0 && l > 0 && h < _LocalMaxHeight && w < Settings.KernelSize - 1 && l < Settings.KernelSize - 1)
                            if (Random.Range(0, h * h) == 0)
                            {
                                GameObject _t = MonoBehaviour.Instantiate(Settings.TreePrefab, new Vector3(w, h, l) + _k.gameObject.transform.position + Vector3.left * Random.Range(-0.3f, 0.3f) + Vector3.forward * Random.Range(-0.3f, 0.3f), Quaternion.identity); //Сажаем деревья
                                _t.transform.SetParent(_b.transform);
                                _t.name = "Tree";
                            }
                    }
                yield return new WaitForSeconds(0.2f);
            }
            if (Time.deltaTime > 0.5f)
                yield return new WaitForSeconds(0.3f);
        }
    }
}
