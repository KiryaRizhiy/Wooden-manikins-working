using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoComponent : MonoBehaviour {

    public NameValue Content
    {
        get
        {
            return _Content;
        }
        set
        {
            _textContainer.text = value.Name + " :\n" + value.Value;
            _Content = value;
        }
    }
    public Vector2 Position
    {
        get
        {
            return _textContainer.GetComponent<RectTransform>().anchoredPosition;
        }
        set
        {
            _textContainer.GetComponent<RectTransform>().anchoredPosition = value;
        }
    }
    public Vector2 Size
    {
        get
        {
            return _textContainer.rectTransform.sizeDelta;
}
        set
        {
            _textContainer.rectTransform.sizeDelta = value;
        }
    }
    
    private NameValue _Content;

    private Text _textContainer;
    void Awake()
    {
        _textContainer = gameObject.AddComponent<Text>();
        RectTransform _rt = _textContainer.GetComponent<RectTransform>();
        _rt.anchorMax = Vector2.zero;
        _rt.anchorMin = Vector2.zero;
        _textContainer.color = Color.white;
        _textContainer.font = Settings.MenuTextFont;
        //_textContainer.rectTransform.si
    }
}