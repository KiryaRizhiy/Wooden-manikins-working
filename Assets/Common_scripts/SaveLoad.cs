using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveLoad{

    //Сохранение карты мира
    private static List<Map> SavedMaps = new List<Map>(); //Список сохраенных карт

    //Сохранение игры
    public static List<Game> SavedGames = new List<Game>(); //Список сохраненных игр

    public static void Save()
    {
        LoadAllGames();
        if (SavedGames.Find(x => x.id == Settings.CurrentGame.id) != null)//Если такая карта уже сохранена
            SavedGames[SavedGames.FindIndex(x => x.id == Settings.CurrentGame.id)] = Settings.CurrentGame;
        else
            SavedGames.Add(Settings.CurrentGame);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + Settings.GameSavingPath);
        bf.Serialize(file, SaveLoad.SavedGames);
        file.Close();
        Debug.Log("Game " + Settings.CurrentGame.id + " saved");
        SaveMap();
    }

    public static void LoadAllGames()
    {
        if (File.Exists(Application.persistentDataPath + Settings.GameSavingPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + Settings.GameSavingPath, FileMode.Open);
            SaveLoad.SavedGames = (List<Game>)bf.Deserialize(file);
            file.Close();
        }
    }
    public static void LoadGame(int GameIndex)
    {
        Settings.CurrentGame = SavedGames[GameIndex];
        LoadAllMaps();
        Map.CurrentMap = SavedMaps[GameIndex];
        SavedMaps.Clear();
        SavedGames.Clear();
    }
    public static void SaveMap()
    {
        LoadAllMaps();
        if (SavedMaps.Find(x => x.MapName == Map.CurrentMap.MapName) != null)//Если такая карта уже сохранена
            SavedMaps[SavedMaps.FindIndex(x => x.MapName == Map.CurrentMap.MapName)] = Map.CurrentMap; //Пересохраняем
        else
            SavedMaps.Add(Map.CurrentMap);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + Settings.MapSavingPath);
        bf.Serialize(file, SaveLoad.SavedMaps);
        file.Close();
        Debug.Log("Map " + Map.CurrentMap.MapName + " saved");
    }
    public static void LoadAllMaps()
    {
        if (File.Exists(Application.persistentDataPath + Settings.MapSavingPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + Settings.MapSavingPath, FileMode.Open);
            SaveLoad.SavedMaps = (List<Map>)bf.Deserialize(file);
            file.Close();
        }
    }
}
