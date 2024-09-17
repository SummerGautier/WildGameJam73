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
        this.TopLevel = true;
        this.GlobalPosition = new Vector2(0, 0);
    }

    public override void _Draw()
    {
        if (_curve.GetBakedPoints().Length > 0)
        {
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
        for (int i = 0; i < points.Length; i++)
        {
            _curve.AddPoint(points[i]);
        }
    }

}