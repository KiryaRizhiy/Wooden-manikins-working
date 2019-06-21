using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
public class GameLoader : MonoBehaviour, IPointerClickHandler
{
    private Dropdown GameSelector;
    void Start()
    {
        GameSelector = transform.parent.GetChild(2).GetComponent<Dropdown>();
        foreach (Game _g in SaveLoad.LoadAllGames().SavedGames)
        {
            GameSelector.options.Add(new Dropdown.OptionData(_g.GetId()));
            Debug.Log("Option " + _g.GetId() + " added");
        }
    }
    public void OnPointerClick(PointerEventData EventData)
    {
        if (GameSelector.value == -1)
        {
            Debug.LogError("Game not selected!");
            return;
        }
        Settings.LaunchMode = GameLaunchMode.LoadGame;
        SaveLoad.LoadGame(GameSelector.value);
    }
}