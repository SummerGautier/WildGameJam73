using Godot;
using System;

public partial class MovementSystem : Node
{
    // read-only variables
    [Export]
    private Player _player;

    [Export]
    private float _runSpeed = 500f;
    [Export]
    private float _jumpSpeed = 500f;

    [ExportGroup("Jump Properties")]
    [Export]
    float _jumpCurveWidth = 0.005f;
    [Export]
    float _jumpCurveHeight = 0.1f;

    [ExportGroup("Debug Settings")]
    [Export]
    private Graph _graph;

    // internal use-only
    private Vector2 _jumpStartPosition;
    private double _jumpPathDelta;
    private LandingLine _landingLine;

    private Cardinal _direction;

    // class state management
    private MovementType _currentMovement;
    private MovementType _previousMovement;
    private MovementType _nextMovement;

    // enums, flags, statics
    private enum MovementType
    {
        IDLE,
        JUMP,
        RUN
    }

    [Flags]
    public enum Cardinal
    {
        None = 0,
        Left,
        Right,
        Towards,
        Away,
    }

    public override void _Ready()
    {
        _jumpStartPosition = Vector2.Zero;
        _jumpPathDelta = 0;
        _currentMovement = MovementType.IDLE;
        
        _landingLine = new LandingLine(_player.GlobalPosition.Y, _player.PLAYER_HEIGHT, _player.PLAYER_FEET_WIDTH);
        _landingLine.UpdateLandingAxis(_player.GlobalPosition.Y);
        this.AddChild( _landingLine );

        _landingLine.AreaEntered += OnJumpLand;
        _graph = new Graph();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        switch (_currentMovement)
        {
            case MovementType.IDLE:
                if(_previousMovement != MovementType.IDLE)
                {
                    GD.Print("IDLE");
                }
                _previousMovement = MovementType.IDLE;
                break;
            case MovementType.JUMP:
                // check if jump just started
                if(_previousMovement != MovementType.JUMP)
                {
                    _DrawJumpPath();
                    GD.Print("JUMP");
                }
                // move towards jump target
                _FollowJumpCurve(delta);
                _previousMovement = MovementType.JUMP;
                break;
            case MovementType.RUN:
                if(_previousMovement != MovementType.RUN)
                {
                    GD.Print("RUN");
                }
                _FollowRunTarget(delta);
                _previousMovement = MovementType.RUN;
                break;

        }
    }

    /*
     * SIGNAL ACTION METHODS
     */

    private void _OnRunInput(MovementSystem.Cardinal  target_direction)
    {
        GD.Print("Run Input Registered");
        //only run if we're not jumping;
        if(_currentMovement != MovementType.JUMP)
        {
            _direction = target_direction;
            _currentMovement = MovementType.RUN;
        }
    }

    private void _OnJumpInput()
    {
        // only jump if we aren't already
        if (_currentMovement != MovementType.JUMP)
        {
            _currentMovement = MovementType.JUMP;
            _jumpStartPosition = _player.GlobalPosition;
            _jumpPathDelta = 0;
            _landingLine.UpdateLandingAxis(_player.GlobalPosition.Y);
        }
    }

    public void OnJumpLand(Area2D area)
    {
        if (_currentMovement == MovementType.JUMP)
        {
            _currentMovement = MovementType.IDLE;
            _player.GlobalPosition = new Vector2(_player.GlobalPosition.X, _landingLine.GetSurfaceOfLandingLine());
        }
    }


    /*
     * HELPER METHODS & CLASSES
     */

    private void _FollowRunTarget(double delta)
    {
        if(_previousMovement != MovementType.RUN)
        {
            GD.Print("direction" + _direction.ToString());
        }
        
        Vector2 velocity = Vector2.Zero;
        if (_direction.HasFlag(Cardinal.Right)) { velocity.X += 1; }
        if (_direction.HasFlag(Cardinal.Left)) { velocity.X -= 1; }
        if (_direction.HasFlag(Cardinal.Towards)) { velocity.Y += 1; }
        if (_direction.HasFlag(Cardinal.Away)) { velocity.Y -= 1; }

        velocity = velocity.Normalized() * _runSpeed;

        _player.Position += velocity * (float)delta;
        _player.Position = new Vector2(
            x: Mathf.Clamp(_player.Position.X, 0, 1920),
            y: Mathf.Clamp(_player.Position.Y, 0, 1080)
        );
        _currentMovement = MovementType.IDLE;
    }

    private void _FollowJumpCurve(double delta)
    {
        /*
         * Assuming the origin of the graph is the node's jump start position,
         * calculate the relative target position along the parabola,
         * add relative target vector ontop of starting vector to get target position
         * move player to target position
         */

        _jumpPathDelta += delta * _jumpSpeed; //fraction of how far along jump curve we are so far
        Vector2 pointOnJumpCurve = _GetPointOnJumpCurve((float)_jumpPathDelta);
        Vector2 targetPosition = _jumpStartPosition + pointOnJumpCurve;

        _player.Position = targetPosition;

    }

    private Vector2 _GetPointOnJumpCurve(float relative_delta) //x value where origin is jump start position
    {
        /** For more complex curves: 
         * //https://docs.godotengine.org/en/stable/tutorials/math/beziers_and_curves.html
         */
        float jump_curve_offset = (1 / _jumpCurveWidth);
        float delta = relative_delta - jump_curve_offset; // should start at the first x-intercept;

        Vector2 pointOnJumpCurve = Vector2.Zero;
        pointOnJumpCurve.Y = (_jumpCurveWidth * delta * delta) + (_jumpCurveHeight * delta) - jump_curve_offset;
        pointOnJumpCurve.X = relative_delta;

        return pointOnJumpCurve;
    }

    private partial class LandingLine : Area2D
    {
        private CollisionShape2D _collisionShape;

        private float _jumperHeight;
        private float _height;
        private float _width;
        private float _y_axis;
        

        private Color _purple = new Color(0.62f, 0.12f, 0.94f, 0.3f);
        public LandingLine(float start_y_axis, float jumper_height,  float foot_width)
        {
            _width = DisplayServer.WindowGetSize().X;
            _height = foot_width;
            _jumperHeight = jumper_height;

            // collision bounding box
            _collisionShape = new CollisionShape2D();
            _collisionShape.DebugColor = _purple;

            RectangleShape2D boundingBox = new RectangleShape2D();
            boundingBox.Size = new Vector2(_width, _height); //landing line should expand the width of the screen;

            _collisionShape.Position = new Vector2(_width/2, Vector2.Zero.Y); // shift the middle of the shape to the mid-point of the screen
            _collisionShape.Shape = boundingBox;

            //assign child objects
            this.AddChild(_collisionShape);

            //set position
            this.UpdateLandingAxis(start_y_axis);
        }

        public void UpdateLandingAxis(float y_axis)
        {
            // offset y-axis by the jumper's height and the height of the landing line
            this._y_axis = y_axis + (_jumperHeight / 2) + (_height / 2);
            //Only move part of the way there, to allow player to escape, avoiding premature collision
            this.GlobalPosition = new Vector2(Vector2.Zero.X, this._y_axis -_height);
        }

        public override void _Process(double delta)
        {
            this.GlobalPosition.MoveToward(new Vector2(0, _y_axis), (float)delta);
        }

        public float GetSurfaceOfLandingLine()
        {
            //calculate the y_axis of the top of the landing line
            return this.GlobalPosition.Y - (_jumperHeight/2) - (_height/2);
        }
    }


    /*
     * DEBUG HELPERS
     */
    private void _DrawJumpPath()
    {
        int number_points = (int)(1 / _jumpCurveWidth) * 100;
        // generate array of points
        Vector2[] pointsOnJumpCurve = new Vector2[number_points];
        float point_spacing = 0.1f;
        for (int i = 0; i < pointsOnJumpCurve.Length; i++)
        {
            Vector2 vector = _GetPointOnJumpCurve(i * point_spacing);
            pointsOnJumpCurve[i] = new Vector2(vector.X, vector.Y);
        }

        _graph.GlobalPosition = _player.GlobalPosition;
        _graph.TopLevel = true;
        _graph.PopulateCurve2D(pointsOnJumpCurve);
    }

}