using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour { // Сделать статиком

    //public void SetProfession()
    public GameObject FindAWorker()
    {
        for (int i = 0; i< transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<Unit>().CurrentProcessID != -1)
                continue;
            else
                return transform.GetChild(i).gameObject;
        }
        return null;
    }
}
