using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Player : AnimationSprite
{
    public enum PlayerSelection
    {
        PLAYER_ONE,
        PLAYER_TWO
    }

    public enum PlayerState
    {
        IDLE,
        MOVING,
        JUMPING,
        PICKED_UP_BALL,
        HIT
    }

    const int LEFT_HALF_BEGINNING = 0;
    const int LEFT_HALF_END = 620;
    const int RIGHT_HALF_BEGINNING = 660;
    const int RIGHT_HALF_END = 1280;
    const int NO_MOVEMENT_X = 0;
    const int NO_MOVEMENT_Y = 0;
    const float FRICTION_MOVEMENT = 0.8f;

    float speedX;
    float speedY;
    float jumpingPower;
    int animationDelay;
    int animationStep;
    int lives;
    bool moving;
    bool thrown;
    bool hasLanded;

    Sprite hitbox;

    public PlayerSelection state;
    public PlayerState playerState;

    public Player(float x, float y, float width, float height, PlayerSelection state = PlayerSelection.PLAYER_ONE) : base("img/objects/player-spritesheet.png", 4, 6, 24)
    {
        SetXY(x, y);
        this.width = (int)width;
        this.height = (int)height;
        this.state = state;
        playerState = PlayerState.IDLE;
        jumpingPower = -25f;
        lives = 1;

        SetPlayerFrame();

        hitbox = new Sprite("img/objects/colors.png");
        hitbox.alpha = 0f;
        hitbox.width = this.width / 2;
        hitbox.SetOrigin(this.width / 2, 0);
        hitbox.x = this.width / 2;
        AddChild(hitbox);
    }

    void Update()
    {
        HandleState();
        Movement();
        Gravity();
        CheckBoundariesPlayer();
    }

    public void DeductAmountLives(int amountLives)
    {
        lives -= amountLives;
    }

    public 

    void HandleState()
    {
        switch (playerState)
        {
            case PlayerState.IDLE:
                HandleIdleState();
                break;
            case PlayerState.MOVING:
                HandleMovingState();
                break;
            case PlayerState.JUMPING:
                HandleJumpingState();
                break;
            case PlayerState.PICKED_UP_BALL:
                HandlePickupState();
                break;
        }
    }

    void SetState(PlayerState newState)
    {
        if (playerState != newState)
            playerState = newState;
    }

    void HandleIdleState()
    {
        IdleAnimation();
        thrown = false;

        if (moving == true)
            SetState(PlayerState.MOVING);

        if (pickedUpBall == true)
            SetState(PlayerState.PICKED_UP_BALL);

        if (hasLanded == false)
            SetState(PlayerState.JUMPING);
    }

    void HandleMovingState()
    {
        MovingAnimation();

        if (hasLanded == false)
            SetState(PlayerState.JUMPING);

        if (moving == false == pickedUpBall == false)
            SetState(PlayerState.IDLE);

        if (pickedUpBall == true)
            SetState(PlayerState.PICKED_UP_BALL);
    }

    void HandlePickupState()
    {
        if (thrown == false)
        {
            if (state == PlayerSelection.PLAYER_ONE)
            {
                SetFrame(10);
            }
            else
            {
                SetFrame(22);
            }
        }
        else
        {
            if (state == PlayerSelection.PLAYER_ONE)
            {
                SetFrame(11);
            }
            else
            {
                SetFrame(22);
            }

            SetState(PlayerState.IDLE);
        }
    }

    void HandleJumpingState()
    {
        if (state == PlayerSelection.PLAYER_ONE)
            SetFrame(8);
        else
            SetFrame(20);

        if (hasLanded == true)
            SetState(PlayerState.IDLE);
    }

    void IdleAnimation()
    {
        if (playerState == PlayerState.IDLE && state == PlayerSelection.PLAYER_ONE)
        {
            animationDelay = 10;

            if (currentFrame >= 3)
            {
                SetFrame(0);
            }

            animationStep++;

            if (animationStep > animationDelay)
            {
                NextFrame();
                animationStep = 0;
            }
        }
        else
        {
            animationDelay = 10;

            if (currentFrame >= 15 || currentFrame < 12)
            {
                SetFrame(12);
            }

            animationStep++;

            if (animationStep > animationDelay)
            {
                NextFrame();
                animationStep = 0;
            }
        }
    }

    void MovingAnimation()
    {
        if (playerState == PlayerState.MOVING && state == PlayerSelection.PLAYER_ONE)
        {
            animationDelay = 6;

            if (currentFrame >= 7 || currentFrame <= 4)
            {
                SetFrame(4);
            }

            animationStep++;

            if (animationStep > animationDelay)
            {
                NextFrame();
                animationStep = 0;
            }
        }
        else if (playerState == PlayerState.MOVING && state == PlayerSelection.PLAYER_TWO)
        {
            animationDelay = 6;

            if (currentFrame >= 19 || currentFrame <= 16)
            {
                SetFrame(16);
            }

            animationStep++;

            if (animationStep > animationDelay)
            {
                NextFrame();
                animationStep = 0;
            }
        }
    }

    void SetPlayerFrame()
    {
        if (state == PlayerSelection.PLAYER_ONE)
        {
            SetFrame(0);
        }
        else
        {
            SetFrame(12);
            Mirror(true, false);
        }
    }

    void CheckBoundariesPlayer()
    {
        if (state == PlayerSelection.PLAYER_ONE && x < LEFT_HALF_BEGINNING)
        {
            x = 0;
        }
        else if (state == PlayerSelection.PLAYER_ONE && x > LEFT_HALF_END - this.width)
        {
            x = LEFT_HALF_END - this.width;
        }

        if (state == PlayerSelection.PLAYER_TWO && x < RIGHT_HALF_BEGINNING)
        {
            x = RIGHT_HALF_BEGINNING;
        }
        else if (state == PlayerSelection.PLAYER_TWO && x > RIGHT_HALF_END - this.width)
        {
            x = RIGHT_HALF_END - this.width;
        }
    }

    /// <summary>
    /// Takes care of Player movement
    /// </summary>
    void Movement()
    {
        if (state == PlayerSelection.PLAYER_ONE && Input.GetKey(Key.D) || state == PlayerSelection.PLAYER_TWO && Input.GetKey(Key.RIGHT))/// Move to right
        {
            moving = true;
            speedX += 3f;
            CheckIfMoving(speedX, NO_MOVEMENT_Y);
        }
        else if (state == PlayerSelection.PLAYER_ONE && Input.GetKey(Key.A) || state == PlayerSelection.PLAYER_TWO && Input.GetKey(Key.LEFT))/// Move to left
        {
            moving = true;
            speedX -= 3f;
            CheckIfMoving(speedX, NO_MOVEMENT_Y);
        }
        else
        {
            moving = false;
        }

        speedX *= FRICTION_MOVEMENT;
    }

    /// <summary>
    /// Checks if player is moving or not in the X and Y axis
    /// </summary>
    /// <param name="movementX"></param>
    /// <param name="movementY"></param>
    /// <returns></returns>
    private bool CheckIfMoving(float movementX, float movementY)
    {
        bool isSuccess = true;
        x += movementX;
        y += movementY;

        foreach (GameObject other in GetCollisions())
        {
            if (other == hitbox)
            {
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }
        }
        if (!isSuccess)
        {
            x -= movementX;
            y -= movementY;
        }
        return isSuccess;
    }

    /// <summary>
    /// Takes care of Player Gravity
    /// </summary>
    void Gravity()
    {
        speedY++;


        if (!CheckIfMoving(NO_MOVEMENT_X, speedY))
        {
            if (speedY > 0)
            {
                hasLanded = true;
                speedY = 0;
            }
        }

        if (hasLanded == true)
        {
            Jump();
        }
    }

    /// <summary>
    /// Makes the player jump when W key is pressed
    /// </summary>
    void Jump()
    {
        if (state == PlayerSelection.PLAYER_ONE && Input.GetKeyDown(Key.W) || state == PlayerSelection.PLAYER_TWO && Input.GetKeyDown(Key.UP))
        {
            speedY = jumpingPower;
            hasLanded = false;
        }
    }

    bool pickedUpBall = false;
    float throwPower = 25f;

    void OnCollision(GameObject other)
    {
        if (other is Ball)
        {
            Ball ball = other as Ball;
            if (pickedUpBall == false && ball.state == Ball.BallState.IDLE)
            {
                /// Pick up ball
                pickedUpBall = true;
                ball.PickUpBall();
                Ball _newBall = new Ball(new Vec2(0, 0), 48, 48);

                if (state == PlayerSelection.PLAYER_ONE)
                    _newBall.SetXY(-5, 0);
                else
                    _newBall.SetXY(15, 0);

                LateAddChild(_newBall);
                _newBall.CarryingBall(pickedUpBall);
            }
            else if (pickedUpBall == true)
            {
                /// Throw ball
                if (state == PlayerSelection.PLAYER_ONE && Input.GetKeyDown(Key.SPACE))
                {
                    ball.LateDestroy();
                    MyGame _myGame = game as MyGame;
                    Ball _thrownBall = new Ball(new Vec2(this.x + 80, this.y), 48, 48, Ball.BallState.THROWN);
                    _myGame.LateAddChild(_thrownBall);
                    _thrownBall.ballOnPlayer("player_1");
                    _thrownBall.MoveBall(throwPower);
                    playerState = PlayerState.IDLE;
                    pickedUpBall = false;
                    thrown = true;
                }
                else if (state == PlayerSelection.PLAYER_TWO && Input.GetKeyDown(Key.ENTER))
                {
                    ball.LateDestroy();
                    MyGame _myGame = game as MyGame;
                    Ball _thrownBall = new Ball(new Vec2(this.x - 40, this.y), 48, 48, Ball.BallState.THROWN);
                    _myGame.LateAddChild(_thrownBall);
                    _thrownBall.ballOnPlayer("player_2");
                    _thrownBall.MoveBall(throwPower);
                    playerState = PlayerState.IDLE;
                    pickedUpBall = false;
                    thrown = true;
                }
            }
        }
    }
}