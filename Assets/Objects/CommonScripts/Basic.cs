using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic : MonoBehaviour
{

    public static string scr { get { return "Basic"; } }

    public bool Busy;

    void Click()
    {
        if (Settings.ComplexObjectsTags.Contains(gameObject.tag) && transform.childCount == 0)//Если объект - часть другого сложного объекта и не имеет детей
            transform.parent.gameObject.SendMessage("Click");
        else
            Log.Notice(Basic.scr, gameObject.name + " clicked");
    }

    void Pick()
    {
        if (Settings.ComplexObjectsTags.Contains(gameObject.tag) && transform.childCount == 0)//Если объект - часть другого сложного объекта и не имеет детей
            transform.parent.gameObject.SendMessage("Pick");
        else
            Log.Notice(Basic.scr, gameObject.name + " picked");
    }

    void UnPick()
    {
        if (Settings.ComplexObjectsTags.Contains(gameObject.tag) && transform.childCount == 0)//Если объект - часть другого сложного объекта и не имеет детей
            transform.parent.gameObject.SendMessage("UnPick");
        else
            Log.Notice(Basic.scr, gameObject.name + " unpicked");
    }
}
