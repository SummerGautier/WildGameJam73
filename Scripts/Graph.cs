using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Graph : Node2D
{
    private static Curve2D _curve = new Curve2D();

    public override void _Ready()
    {
    }

    public override void _Draw()
    {
        if (_curve.GetBakedPoints().Length > 0)
        {
            GD.Print("curve is available");
            _DrawCurve();  
        }
    }

    private void _DrawCurve()
    {
        DrawPolyline(_curve.GetBakedPoints(), new Color(1, 1, 0), 2);
    }

    public override void _Process(double delta)
    {
        this.QueueRedraw();
    }



    public void PopulateCurve2D(Vector2[] data)
    {
        Vector2[] points = (Vector2[])data.Clone();
        _curve = new Curve2D();

        float distance = 0;

        // calculate first point
        Vector2 directionBeginPoint = points[0].DirectionTo(points[1]);
        _curve.AddPoint(points[0], -directionBeginPoint * distance, directionBeginPoint * distance);

        // calculate middle points
        for (int i = 1; i < points.Length - 1; i++)
        {
            Vector2 directionToPoint = points[i - 1].DirectionTo(points[i + 1]);
            _curve.AddPoint(points[i], -directionToPoint * distance, directionToPoint * distance);
        }

        //calculate last point
        Vector2 directionEndPoint = points[points.Length - 1].DirectionTo(points[points.Length - 2]);
        _curve.AddPoint(points[0], -directionEndPoint * distance, directionEndPoint * distance);
    }

}