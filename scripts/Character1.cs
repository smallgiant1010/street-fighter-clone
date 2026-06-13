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
	[Export] private float Speed = 300.0f;
	[Export] private float JumpVelocity = -400.0f;
	[Export] private float Gravity = 9.81f; // gravity is flipped since we negative goes up instead of down
	[Export] private AnimatedSprite2D _animatedSprite;
	[Export] private CollisionShape2D _collisionShape;
	private CharacterState characterState;
	private CharacterState lastCharacterState;
	private IDictionary<CharacterState, CollisionMetaData> collisionProperties;

	public override void _Ready()
	{
		Scale = new Vector2(5.5f, 5.5f);
		characterState = CharacterState.IDLE;
		lastCharacterState = characterState;
		collisionProperties = new Dictionary<CharacterState, CollisionMetaData>();
		CreateCollisionProperties();
	}

	public override void _Process(double delta)
	{
		PlayAnimation();
	}

	public override void _PhysicsProcess(double delta)
	{
		bool grounded = IsOnFloor();
		float XDirection = Input.GetAxis("Backwards", "Forwards");
		HandleMovement((float)delta, XDirection, grounded);
		UpdateState(XDirection, grounded);
		UpdateCollision();
	}

	private void UpdateCollision()
	{
		if (characterState == lastCharacterState) return;
		_collisionShape.Position = collisionProperties[characterState].PositionOffset;
		((RectangleShape2D)_collisionShape.Shape).Size = collisionProperties[characterState].Size;
		lastCharacterState = characterState;
	}

	private void CreateCollisionProperties()
	{
		CollisionMetaData idleData = new CollisionMetaData(-13.25f, 0f, 0f, 31.0f, 86.0f);
		collisionProperties.Add(CharacterState.IDLE, idleData);

		CollisionMetaData walkingBackwardsData = new CollisionMetaData(-2.0f, 0f, 0f, 26.5f, 86.0f);
		collisionProperties.Add(CharacterState.WALKING_BACKWARDS, walkingBackwardsData);
		
		CollisionMetaData walkingForwardsData = new CollisionMetaData(0f, 0f, 0f, 22.0f, 86.0f);
		collisionProperties.Add(CharacterState.WALKING_FORWARDS, walkingForwardsData);

		CollisionMetaData jumpData = new CollisionMetaData(1.0f, 0f, 0f, 55.0f, 54.0f);
		collisionProperties.Add(CharacterState.JUMPING, jumpData);

		CollisionMetaData crouchData = new CollisionMetaData(0f, 0f, 0f, 41.0f, 56.0f);
		collisionProperties.Add(CharacterState.CROUCHING, crouchData);
	}

	private void HandleMovement(float delta, float XDirection, bool grounded)
	{
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
