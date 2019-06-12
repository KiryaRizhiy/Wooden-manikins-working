using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class GameLoader : MonoBehaviour, IPointerClickHandler {

    private Dropdown GameSelector;

	void Start () {
        SaveLoad.LoadAllGames();
        GameSelector = transform.parent.GetChild(2).GetComponent<Dropdown>();
        foreach (Game _g in SaveLoad.SavedGames)
        {
            GameSelector.options.Add(new Dropdown.OptionData(_g.id.ToString()));
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
