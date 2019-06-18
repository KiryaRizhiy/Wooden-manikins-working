using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UISaveGame : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick(PointerEventData EventData)
    {
        SaveLoad.Save();
        //SceneManager.LoadScene("Main_menu", LoadSceneMode.Single);
    }
}
