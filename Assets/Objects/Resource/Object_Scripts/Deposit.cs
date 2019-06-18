using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit{

    public static string scr = "Deposit";

    private FigureParams _Params; //Параметры геометрической формы. Задаются рандомно в логике конструктора
    private Vector3 _Center;
    private CheckByCoordinates IsPointInFigure; //Делегат для вычисления принадлежности точки геометрической фигуре
    public Resource DepositResource { get; private set; } //Тип ресурса, находящегося в залежах

    public Deposit(Vector3 Center, Resource Resource)
    {
        DepositResource = Resource;        
        _Center = Center;
        switch (DepositResource.Figure)
        {
            case DepositFigure.Ellips:
                    {
                        IsPointInFigure = Functions.IsCoordinatesIntoEllips;
                        _Params = new FigureParams(Random.Range(4,7), Random.Range(4, 7), Random.Range(4, 7));
                        break;
                    }
            case DepositFigure.WideEllips:
                    {
                        IsPointInFigure = Functions.IsCoordinatesIntoEllips;
                        _Params = new FigureParams(Mathf.Pow(Random.Range(4, 7), Random.Range(2, 4)), Random.Range(4, 7), Mathf.Pow(Random.Range(4, 7), Random.Range(2, 4)));
                        break;
                    }
            case DepositFigure.Sphere:
            default:
                {
                    IsPointInFigure = Functions.IsCoordinatesIntoASphere;
                    _Params = new FigureParams(Random.Range(4, 7));
                    break;
                }
    }
        //Надо присваивать делегат в зависимости от параметров ресурса
    }

    public bool IsBrickInDeposit(Vector3 BrickCoordinates)
    {
        //Надо сложить в делегат ISPointInFigure координаты центра и параметры геометрической формы
        return IsPointInFigure.Invoke(BrickCoordinates,_Center,_Params);
    }
}
delegate bool CheckByCoordinates(Vector3 Coordinates, Vector3 Center, FigureParams Parameters);
public enum DepositFigure { Sphere, Ellips, WideEllips }
