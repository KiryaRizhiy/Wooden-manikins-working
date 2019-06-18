using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour {

    private static Color _Gray = new Color(0.75f, 0.75f, 0.75f, 1f), _Dark = new Color(0.5f, 0.5f, 0.5f, 1f), _Bright = new Color(1.25f, 1.25f, 1.25f, 1f), _Green = new Color(0.30f, 0.85f, 0.30f, 1f);

    public static void HighLight(GameObject obj,bool ComplexObject = true)
    {
        if (Settings.ComplexObjectsTags.Contains(obj.tag)&&ComplexObject)
            foreach (GameObject _obj in Functions.GetAllChildren(obj.transform.parent.gameObject))
                HighLight(_obj, false);
        else
        if (obj.GetComponent<MeshRenderer>() != null)
            if (obj.GetComponent<MeshRenderer>().material.color != _Dark && obj.GetComponent<MeshRenderer>().material.color != _Bright)
                obj.GetComponent<MeshRenderer>().material.color = _Gray;
    }
    public static void UnHighLight(GameObject obj, bool ComplexObject = true)
    {
        if (Settings.ComplexObjectsTags.Contains(obj.tag) && ComplexObject)
            foreach (GameObject _obj in Functions.GetAllChildren(obj.transform.parent.gameObject))
                UnHighLight(_obj, false);
        else
        if (obj.GetComponent<MeshRenderer>() != null)
            if (obj.GetComponent<MeshRenderer>().material.color != _Dark && obj.GetComponent<MeshRenderer>().material.color != _Bright)
                obj.GetComponent<MeshRenderer>().material.color = Color.white;
    }
    public static void Pick(GameObject obj, bool ComplexObject = true)
    {
        if (Settings.ComplexObjectsTags.Contains(obj.tag) && ComplexObject)
            foreach (GameObject _obj in Functions.GetAllChildren(obj.transform.parent.gameObject))
                Pick(_obj, false);
        else
        if (obj.GetComponent<MeshRenderer>() != null)
            obj.GetComponent<MeshRenderer>().material.color = _Dark;
    }
    public static void UnPick(GameObject obj, bool ComplexObject = true)
    {
        if (Settings.ComplexObjectsTags.Contains(obj.tag) && ComplexObject)
            foreach (GameObject _obj in Functions.GetAllChildren(obj.transform.parent.gameObject))
                UnPick(_obj, false);
        else
        if (obj.GetComponent<MeshRenderer>() != null)
            obj.GetComponent<MeshRenderer>().material.color = Color.white;
    }
    public static void BrightPick(GameObject obj, bool ComplexObject = true)
    {
        if (Settings.ComplexObjectsTags.Contains(obj.tag) && ComplexObject)
            foreach (GameObject _obj in Functions.GetAllChildren(obj.transform.parent.gameObject))
                BrightPick(_obj, false);
        else
        if (obj.GetComponent<MeshRenderer>() != null)
            obj.GetComponent<MeshRenderer>().material.color = _Bright;
    }
    public static void Phantomize(GameObject obj, bool ComplexObject = true)
    {
        if (Settings.ComplexObjectsTags.Contains(obj.tag) && ComplexObject)
            foreach (GameObject _obj in Functions.GetAllChildren(obj.transform.parent.gameObject))
                Phantomize(_obj,false);
        else
        if (obj.GetComponent<MeshRenderer>() != null)
            obj.GetComponent<MeshRenderer>().material.color = _Green;
    }
}