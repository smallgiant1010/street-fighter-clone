using System;
using System.Collections.Generic;
using Godot;

public enum CharacterState
{
	IDLE,
	WALKING_FORWARDS,
	WALKING_BACKWARDS,
	JUMPING,
	CROUCHING,
}

public partial class Character1 : CharacterBody2D
{
	[ExportGroup("Physics Values")]
	[Export] private float Speed = 300.0f, JumpVelocity = -700.0f;
	[Export] private float Gravity = 2000.0f; // gravity is flipped since we negative goes up instead of down
	[ExportGroup("Dependencies")]
	[Export] private AnimatedSprite2D _animatedSprite;
	[Export] private Area2D _hurtBoxArea2D, _hitBoxArea2D;
	[Export] private GameMap _gameMap;
	private CollisionShape2D _hurtBoxCollisionShape, _hitBoxCollisionShape, _worldCollisionShape;
	private CharacterState characterState, lastCharacterState;
	private float leftBound, rightBound;
	private readonly Dictionary<CharacterState, CollisionMetaData> hurtBoxCollisionProperties = new();

	public override void _Ready()
	{
		Scale = new Vector2(5.5f, 5.5f);
		characterState = CharacterState.IDLE;
		lastCharacterState = characterState;
		_hurtBoxCollisionShape = _hurtBoxArea2D.GetNode<CollisionShape2D>("CollisionShape2D");
		_hitBoxCollisionShape = _hitBoxArea2D.GetNode<CollisionShape2D>("CollisionShape2D");
		_worldCollisionShape = GetNode<CollisionShape2D>("WorldCollisionShape2D");
		_gameMap.MapLoaded += OnMapLoaded;
		CreateCollisionProperties();
	}

    private void OnMapLoaded(int width, int height)
	{
		rightBound = (int)Mathf.Ceil(width / 2.0f);
		leftBound = -1 * rightBound;
	}

    public override void _Process(double delta)
	{
		PlayAnimation();
	}

	public override void _PhysicsProcess(double delta)
	{
		bool grounded = IsOnFloor();
		float XDirection = Input.GetAxis("Backwards", "Forwards");
		UpdateState(XDirection, grounded);
		HandleMovement((float)delta, XDirection, grounded);
		UpdateHurtBoxCollision();
	}

	private void UpdateHurtBoxCollision()
	{
		if (characterState == lastCharacterState) return;
		_hurtBoxCollisionShape.Position = hurtBoxCollisionProperties[characterState].PositionOffset;
		((RectangleShape2D)_hurtBoxCollisionShape.Shape).Size = hurtBoxCollisionProperties[characterState].Size;
		lastCharacterState = characterState;
	}

	private void CreateCollisionProperties()
	{
		CollisionMetaData idleData = new(-11.75f, 4.0f, 0f, 33.5f, 90.0f);
		hurtBoxCollisionProperties.Add(CharacterState.IDLE, idleData);

		CollisionMetaData walkingBackwardsData = new(2.0f, 4.0f, 0f, 26.5f, 90.0f);
		hurtBoxCollisionProperties.Add(CharacterState.WALKING_BACKWARDS, walkingBackwardsData);
		
		CollisionMetaData walkingForwardsData = new(3.0f, 4.0f, 0f, 22.0f, 90.0f);
		hurtBoxCollisionProperties.Add(CharacterState.WALKING_FORWARDS, walkingForwardsData);

		CollisionMetaData jumpData = new(-4.0f, 4.0f, 0f, 33.0f, 90.0f);
		hurtBoxCollisionProperties.Add(CharacterState.JUMPING, jumpData);

		CollisionMetaData crouchData = new(-0.5f, 18.5f, 0f, 39.0f, 63.0f);
		hurtBoxCollisionProperties.Add(CharacterState.CROUCHING, crouchData);
	}

	private void HandleMovement(float delta, float XDirection, bool grounded)
	{
		// GD.Print($"Global Position: ({GlobalPosition.X}, {GlobalPosition.Y})");
		if (characterState == CharacterState.CROUCHING) return;
		Vector2 InitialVelocity = Velocity;
		// handle physics
		if (Input.IsActionJustPressed("Jump") && grounded)
		{
			InitialVelocity.Y = JumpVelocity; // Setting V0
		}
		else if(grounded && InitialVelocity.Y > 0) // clamp the velocity to prevent snapping
		{
			InitialVelocity.Y = 0;
		}
		else if (!grounded)
		{
			InitialVelocity += new Vector2(0f, Gravity * delta); // V0 + at
		}
		InitialVelocity.X = Speed * XDirection; // x direction has no acceleration so it is not affected by time
		Velocity = InitialVelocity; // Vf = V0 + at
		MoveAndSlide();

		Vector2 CharacterSize = ((RectangleShape2D)_worldCollisionShape.Shape).Size;
		Vector2 CurrentPosition = GlobalPosition;
		CurrentPosition.X = Mathf.Clamp(CurrentPosition.X, leftBound + CharacterSize.X, rightBound - CharacterSize.X);
		GlobalPosition = CurrentPosition;
	}

	private void UpdateState(float XDirection, bool grounded)
	{
		// handle states
		if (!grounded)
		{
			characterState = CharacterState.JUMPING;
		}
		else
		{
			if (Input.IsActionPressed("Crouch"))
			{
				characterState = CharacterState.CROUCHING;
			}
			else if (Mathf.IsZeroApprox(XDirection))
			{
				characterState = CharacterState.IDLE;
			}
			else if (XDirection > 0f)
			{
				characterState = CharacterState.WALKING_FORWARDS;
			}
			else
			{
				characterState = CharacterState.WALKING_BACKWARDS;
			}
		}


	}

	private void PlayAnimation()
	{
		string animation;
		switch (characterState)
		{
			case CharacterState.WALKING_FORWARDS:
				animation = "walk_forwards";
				break;
			case CharacterState.WALKING_BACKWARDS:
				animation = "walk_backwards";
				break;
			case CharacterState.JUMPING:
				animation = "jump";
				break;
			case CharacterState.CROUCHING:
				animation = "crouch";
				break;
			case CharacterState.IDLE:
			default:
				animation = "idle";
				break;
		}

		if (_animatedSprite.Animation != animation)
		{
			_animatedSprite.Play(animation);
		}
	}
}
