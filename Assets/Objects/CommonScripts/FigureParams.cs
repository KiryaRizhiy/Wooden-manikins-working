using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureParams{

    public float XScale { get; private set; }
    public float YScale { get; private set; }
    public float ZScale { get; private set; }
    public float Radius { get; private set; }

    public FigureParams(float xScale, float yScale, float zScale)
    {
        XScale = xScale;
        YScale = yScale;
        ZScale = zScale;
    }
    public FigureParams(float radius)
    {
        Radius = radius;
    }
}
