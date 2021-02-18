using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;

public class Ball : AnimationSprite
{
    public enum BallState
    {
        IDLE,
        PICKED_UP,
        THROWN
    }

    const int NO_MOVEMENT_X = 0;
    const int NO_MOVEMENT_Y = 0;
    const int LEFT_BOUNDARY_SCREEN = 0;
    const int RIGHT_BOUNDARY_SCREEN = 1280;
    const int BOTTOM_BOUNDARY_SCREEN = 960;
    const float BOUNCINESS = 0.70f;
    const float FRICTION_MOVEMENT = 0.8f;

    int radius;
    bool pickedUp;
    bool thrown;
    bool hasLanded;
    bool hitPlayer;
    string player;

    Sprite hitbox;

    Vec2 position;
    Vec2 velocity;

    public BallState state;

    public Ball(Vec2 vPosition, float width, float height, BallState state = BallState.IDLE) : base("img/objects/ball-spritesheet.png", 4, 2, 8)
    {
        position = vPosition;
        velocity = new Vec2(0, 0);
        this.width = (int)width;
        this.height = (int)height;
        this.state = state;
        radius = 32;

        SetFrame(1);

        SetXY(position.x, position.y);

        hitbox = new Sprite("img/objects/colors.png");
        hitbox.SetOrigin(this.width / 2, this.height / 2);
        hitbox.x = this.width / 2;
        hitbox.y = this.height / 2;
        hitbox.width = this.width / 2;
        hitbox.height = this.height / 2;
        hitbox.alpha = 0f;
        AddChild(hitbox);
    }

    void Update()
    {
        HandleState();
        ScreenBorderCollisionCheck();
        //Console.WriteLine("Velocity:  X:" + velocity.x + " Y:" + velocity.y);
    }

    public void ballOnPlayer(string player)
    {
        this.player = player;
    }

    public void CarryingBall(bool isCarrying)
    {
        pickedUp = isCarrying;
    }

    public void ThrowingBall(bool isThrowing)
    {
        thrown = isThrowing;
    }

    public void PickUpBall()
    {
        this.LateDestroy();
    }

    void Step()
    {
        Move();
        UpdatePosition();
    }

    void Move()
    {
        position += velocity;

        ResolveCollision();
    }

    void ResolveCollision()
    {
        if (position.x - radius < LEFT_BOUNDARY_SCREEN)
        {
            position.x = LEFT_BOUNDARY_SCREEN + radius;
            velocity.x = -BOUNCINESS * velocity.x;
        }
        else if (position.x + radius > RIGHT_BOUNDARY_SCREEN)
        {
            position.x = RIGHT_BOUNDARY_SCREEN - radius;
            velocity.x = -BOUNCINESS * velocity.x;
        }

        if (position.y + radius > BOTTOM_BOUNDARY_SCREEN)
        {
            position.y = BOTTOM_BOUNDARY_SCREEN - radius;
            velocity.y = -BOUNCINESS * velocity.y;
        }
    }

    void UpdatePosition()
    {
        x = position.x;
        y = position.y;
    }

    void HandleState()
    {
        switch (state)
        {
            case BallState.IDLE:
                HandleIdleState();
                break;
            case BallState.PICKED_UP:
                HandlePickupState();
                break;
            case BallState.THROWN:
                HandleThrownState();
                break;
        }
    }

    void SetState(BallState newState)
    {
        if (state != newState)
            state = newState;
    }

    void HandleIdleState()
    {
        if (pickedUp == true)
            SetState(BallState.PICKED_UP);
        else
            Gravity();
    }

    void HandlePickupState()
    {
        velocity.y = 0;
        velocity.x = 0;

        if (thrown == true)
        {
            pickedUp = false;
            SetState(BallState.THROWN);
        }
    }

    void HandleThrownState()
    {
        if (hasLanded == true && hitPlayer == false)
        {
            thrown = false;
            SetState(BallState.IDLE);
        }

        Gravity();
    }

    public void MoveBall(float throwPower)
    {
        if (player == "player_1")
        {
            velocity = new Vec2(throwPower, 0);
        }
        else if (player == "player_2")
        {
            velocity = new Vec2(-throwPower, 0);
        }
    }

    /// <summary>
    /// Takes care of Ball Gravity
    /// </summary>
    void Gravity()
    {
        velocity.y += 1;
        hasLanded = false;
        Step();

        foreach (GameObject other in GetCollisions())
        {
            if (other != hitbox)
            {
                if (position.y + radius > other.y)
                {
                    position.y = other.y - radius;
                    velocity.y = -BOUNCINESS * velocity.y;
                }
                hasLanded = true;
            }
        }
    }

    /// <summary>
    /// Checks collision with border screen
    /// </summary>
    void ScreenBorderCollisionCheck()
    {
        if (position.x > RIGHT_BOUNDARY_SCREEN - this.width)
        {
            position.x = RIGHT_BOUNDARY_SCREEN - this.width;
            SetState(BallState.IDLE);
        }
        else if (position.x < LEFT_BOUNDARY_SCREEN)
        {
            position.x = LEFT_BOUNDARY_SCREEN;
            SetState(BallState.IDLE);
        }
    }

    void OnCollision(GameObject other)
    {
        if (other is Player)
        {
            Player _player = other as Player;
            hitPlayer = true;
            if (_player.state == Player.PlayerSelection.PLAYER_ONE && state == BallState.THROWN)
            {
                pickedUp = false;
                thrown = false;
                _player.playerState = Player.PlayerState.HIT;
                _player.DeductAmountLives(1);
                //_player.LateDestroy();
                //LateDestroy();
                //MyGame _myGame = game as MyGame;
                //_myGame.LateAddChild(new Player(235, 571, 64, 64, Player.PlayerSelection.PLAYER_ONE));
                //_myGame.LateAddChild(new Ball(new Vec2(800, 0), 48, 48));
                hitPlayer = false;
            }
            else if (_player.state == Player.PlayerSelection.PLAYER_TWO && state == BallState.THROWN)
            {
                pickedUp = false;
                thrown = false;
                _player.playerState = Player.PlayerState.HIT;
                _player.DeductAmountLives(1);
                //_player.LateDestroy();
                //LateDestroy();
                //MyGame _myGame = game as MyGame;
                //_myGame.LateAddChild(new Player(986, 571, 64, 64, Player.PlayerSelection.PLAYER_TWO));
                //_myGame.LateAddChild(new Ball(new Vec2(200, 0), 48, 48));
                hitPlayer = false;
            }
        }
    }
}