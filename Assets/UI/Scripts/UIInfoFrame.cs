using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfoFrame {

    public static string scr = "UIInfoFrame";

    private List<UIInfoComponent> Content = new List<UIInfoComponent>();
    private GameObject _FrameParent;
    private int _MaxRowsCount, _MaxCollsCount;
    private Sack ContentSource;
    private UIInfoActualizer Actualizer;

    public UIInfoFrame(GameObject FrameParent, Sack Bagage)
    {        
        _FrameParent = FrameParent;
        ContentSource = Bagage;
        ComputeMaxRowsColls();
        Log.Notice(scr, "Start loading sack infoframe. Max colls - " + _MaxCollsCount + ", max rows - " + _MaxRowsCount);
        ShowContent();
        Actualizer = _FrameParent.AddComponent<UIInfoActualizer>();
        Actualizer.Initialize(Actualize);
        Log.Notice(scr, "Infoframe loaded");
    }
    public void Actualize()
    {
        ClearContent();
        ShowContent();
        ////Делим на 2 алгоритма. Когда на экране больше инфы, чем в объекте и когда в объекте больше инфы, чем на экране
        //if (ContentSource._Resources.Count <= Content.Count)
        //{
        //    //В интерфейсе выведено больше записей, чем есть в объекте
        //    for (int i = 0; i < ContentSource._Resources.Count; i++)
        //    {

        //    }
        //}Короче, пока делаем тупо, а потом делаем умно
    }
    public void Destroy()
    {
        Actualizer.Pause();
        ClearContent();
        MonoBehaviour.Destroy(Actualizer);
    }

    private void ClearContent()
    {
        //Log.Notice(scr, "Clearing content");
        foreach (GameObject _go in Functions.GetAllChildren(_FrameParent))
        {
            //Log.Notice(scr, "Destroying " + _go.name);
            MonoBehaviour.Destroy(_go);
        }
        Content.Clear();
    }
    private void ShowContent()
    {
        List<SackCell>.Enumerator ContentEnumerator = ContentSource._Resources.GetEnumerator();
        while (ContentEnumerator.MoveNext())
        {
            int j = 0;
            //Log.Notice(UIInfoFrame.scr, "Load row " + j);
            for (int i = 0; i < _MaxRowsCount; i++)
            {
                AddUIComponent(ContentEnumerator.Current.Description, j, i);
                if (!ContentEnumerator.MoveNext())
                {
                    break;
                }
            }
            j++;
        }
    }
    private void ComputeMaxRowsColls()
    {
        _MaxCollsCount = Mathf.FloorToInt((_FrameParent.GetComponent<RectTransform>().rect.width/*-conts with roll size*/ - UISettings.FrameComponentsInterval) / (UISettings.ComponentWidth + UISettings.FrameComponentsInterval));
        _MaxRowsCount = Mathf.FloorToInt((_FrameParent.GetComponent<RectTransform>().rect.height /*-conts with roll size*/ - UISettings.FrameComponentsDistance) / (UISettings.ComponentHeight + UISettings.FrameComponentsDistance));
    }
    private void AddUIComponent(NameValue nameValue, int PostitionX, int PositionY)
    {
        GameObject _go = new GameObject();
        _go.name = nameValue.Name;
        _go.transform.SetParent(_FrameParent.transform, false);
        UIInfoComponent _uic = _go.AddComponent<UIInfoComponent>();
        Content.Add(_uic);
        _uic.Position = new Vector2(UISettings.FrameComponentsInterval + (UISettings.FrameComponentsInterval + UISettings.ComponentWidth) * PostitionX + UISettings.ComponentWidth / 2, UISettings.FrameComponentsDistance + (UISettings.FrameComponentsDistance + UISettings.ComponentHeight) * PositionY + UISettings.ComponentHeight / 2);
        _uic.Size = new Vector2(UISettings.ComponentWidth, UISettings.ComponentHeight);
        _uic.Content = nameValue;
    }
}