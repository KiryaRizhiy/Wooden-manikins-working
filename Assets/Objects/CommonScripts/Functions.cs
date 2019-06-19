using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Functions : MonoBehaviour {

    //private static string scr = "Functions";

    //public static int[] ObjectToWorldCoordinates(Vector3 Coordinates)
    //{
    //    return new int[]{Mathf.RoundToInt(Coordinates.x + Settings.WorldLength / 2 - 0.5f),Mathf.RoundToInt(Coordinates.y + Settings.WorldHeight / 2 - 0.5f),Mathf.RoundToInt(Coordinates.z + Settings.WorldWidth / 2 - 0.5f)};
    //}
    /// <summary>
    /// Логирует XML узел
    /// </summary>
    /// <param name="Node">Узел</param>
    /// <param name="Caller">Имя вызывающего скрипта</param>
    /// <param name="_RecursionDegree">Уровень рекурсии (по умочланию 0)</param>
    public static void ReadXMLNode(XmlNode Node,string Caller, int _RecursionDegree = 0)
    {
        string _d = new string('.', _RecursionDegree);
        Log.Notice(Caller,_d + " " + Node.Name + " " + Node.InnerText);
        foreach (XmlNode _n in Node.ChildNodes)
            ReadXMLNode(_n, Caller, _RecursionDegree + 1);
    }
    public static Vector3 RightVector(Vector3 v)
    {
        return new Vector3(v.z, v.y, -1 * v.x);
    }
    public static Vector3 LeftVector(Vector3 v)
    {
        return new Vector3(-1 * v.z, v.y, v.x);
    }
    public static bool IsNodeNotEmpty(XmlNode Root, string Caller)
    {
        if (Log.CheckScriptLogging(Caller))
        {
            Log.Notice(Caller, "Under emptyness check element printed below:");
            Functions.ReadXMLNode(Root, Caller);
        }
        if (Root.HasChildNodes)
            foreach (XmlNode _n in Root.ChildNodes)
            {
                if (IsNodeNotEmpty(_n,Caller))
                    return true;
            }
        if (string.IsNullOrEmpty(Root.InnerText))
            return false;
        else
        {
            Log.Notice(Caller, "Not empty element is " + Root.Name + ": " + Root.InnerText);
            return true;
        }
    }
    public static List<GameObject> GetAllChildren(GameObject ParentObject)
    {
        List<GameObject> _Result = new List<GameObject>();
        for (int i = 0; i < ParentObject.transform.childCount; i++)
            _Result.Add(ParentObject.transform.GetChild(i).gameObject);
        return _Result;
    }
    public static void OrderVectors(Vector3 _v1, Vector3 _v2, out Vector3 _minv, out Vector3 _maxv)
    {
        _minv.x = Mathf.Min(_v1.x, _v2.x);
        _minv.y = Mathf.Min(_v1.y, _v2.y);
        _minv.z = Mathf.Min(_v1.z, _v2.z);
        _maxv.x = Mathf.Max(_v1.x, _v2.x);
        _maxv.y = Mathf.Max(_v1.y, _v2.y);
        _maxv.z = Mathf.Max(_v1.z, _v2.z);
    }
    public static bool IsCoordinatesIntoASphere(Vector3 Coordinates, Vector3 SphereCenter, FigureParams Parameters)
    {
        return Mathf.Pow(Coordinates.x - SphereCenter.x, 2) + Mathf.Pow(Coordinates.y - SphereCenter.y, 2) + Mathf.Pow(Coordinates.z - SphereCenter.z, 2) <= Parameters.Radius;
    }
    public static bool IsCoordinatesIntoEllips(Vector3 Coordinates, Vector3 EllipsCenter, FigureParams Parameters)
    {
        return 
            Mathf.Pow(Coordinates.x - EllipsCenter.x,2)/Parameters.XScale +
            Mathf.Pow(Coordinates.y - EllipsCenter.y,2)/Parameters.YScale +
            Mathf.Pow(Coordinates.z - EllipsCenter.z,2)/Parameters.ZScale
            <= 1;
    }
}