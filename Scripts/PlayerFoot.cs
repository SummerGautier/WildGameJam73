using Godot;
using System;

public partial class PlayerFoot : Area2D
{
	[Signal]
	public delegate void PlayerCollidedWithBrickEventHandler();
    [Signal]
    public delegate void PlayerCollidedWithAssemblerEventHandler();

    private CollisionLine _collisionLine;
    [Export]
    private float _collisionLineWidth = 15f;

    public override void _Ready()
    {
        this.AreaEntered += OnBaseCollision;
        this._collisionLine = new CollisionLine(_collisionLineWidth);
        GetTree().Root.CallDeferred("add_child", this._collisionLine);
    }

    public override void _Process(double delta)
    {
        if (!this._collisionLine.IsActive()) // if we're not jumping, we are on the assembler
        {
            EmitSignal(SignalName.PlayerCollidedWithAssembler);
        }
    }

    public void OnBaseCollision(Area2D area)
	{
        if (area.IsInGroup("BrickFloor"))
        {
            if (area.OverlapsArea(this._collisionLine) || !this._collisionLine.IsActive())
            {
                EmitSignal(SignalName.PlayerCollidedWithBrick);
                area.AddToGroup("BrickBroken", true);
            }
            return;
        }
        if (area.IsInGroup("BrickBody"))
        {
            if (area.OverlapsArea(this._collisionLine))
            {
                area.AddToGroup("BrickBroken", true);
                EmitSignal(SignalName.PlayerCollidedWithBrick);
            }
        }
    }

    public void OnPlayerJump(Vector2 position)
    {
        this._SetCollisionLine(position.Y);
        this._collisionLine.Activate();
    }

    public void OnPlayerJumpLanded()
    {
        this._SetCollisionLine(DisplayServer.WindowGetSize().Y);
        this._collisionLine.DeActivate();
    }

    private void _SetCollisionLine(float y_axis)
    {
        this._collisionLine.UpdateCollisionAxis(y_axis);
    }

    private partial class CollisionLine : Area2D
    {
        private CollisionShape2D _collisionShape;

        private float _height;
        private float _width;
        private float _y_axis;
        private bool _active = false;

        private Color _purple = new Color(0.62f, 0.12f, 0.94f, 0.3f);
        public CollisionLine(float line_width)
        {
            _width = DisplayServer.WindowGetSize().X;
            _height = line_width;

            // collision bounding box
            _collisionShape = new CollisionShape2D();
            _collisionShape.DebugColor = _purple;

            RectangleShape2D boundingBox = new RectangleShape2D();
            boundingBox.Size = new Vector2(_width, _height); //landing line should expand the width of the screen;

            _collisionShape.Position = new Vector2(_width / 2, Vector2.Zero.Y); // shift the middle of the shape to the mid-point of the screen
            _collisionShape.Shape = boundingBox;

            //assign child objects
            this.AddChild(_collisionShape);

            //set position
            this.UpdateCollisionAxis(DisplayServer.WindowGetSize().Y);
        }

        public void UpdateCollisionAxis(float y_axis)
        {
            // offset y-axis by the jumper's height and the height of the landing line
            this._y_axis = y_axis;
            //Only move part of the way there, to allow player to escape, avoiding premature collision
            this.GlobalPosition = new Vector2(Vector2.Zero.X, this._y_axis);
        }

        public override void _Process(double delta)
        {
        }

        public float GetSurfaceOfCollisionLine()
        {
            //calculate the y_axis of the top of the landing line
            return this.GlobalPosition.Y;
        }
        public bool IsActive()
        {
            return this._active;
        }
        public void Activate()
        {
            this._active = true;
        }
        public void DeActivate()
        {
            this._active = false;
        }
    }
}
