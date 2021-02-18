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

    Sound bounce;
    Sound playerHit;

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

        bounce = new Sound("sounds/ball-hits-ground.mp3");
        playerHit = new Sound("sounds/player-hit.mp3");
    }

    void Update()
    {
        HandleState();
        ScreenBorderCollisionCheck();

        //Console.WriteLine(velocity.y);
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
    bool standStill = false;
    void HandleIdleState()
    {
        if (pickedUp == true)
            SetState(BallState.PICKED_UP);
        if (standStill == true)
            velocity.y = 0;
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

    public void MoveBall(float throwPowerX, float throwPowerY)
    {
        if (player == "player_1")
        {
            velocity = new Vec2(throwPowerX, throwPowerY);
        }
        else if (player == "player_2")
        {
            velocity = new Vec2(-throwPowerX, throwPowerY);
        }
    }

    public int speedY = 1;

    /// <summary>
    /// Takes care of Ball Gravity
    /// </summary>
    void Gravity()
    {
        velocity.y += speedY;
        hasLanded = false;
        Step();

        foreach (GameObject other in GetCollisions())
        {
            if (other != hitbox)
            {
                if (position.y + radius > other.y)
                {
                    if (velocity.y < 0.6f)
                    {
                        speedY = 0;
                        bounce.Play(true);
                        standStill = true;
                    }
                    else if(speedY != 0)
                    {
                        speedY = 1;
                        position.y = other.y - radius;
                        velocity.y = -BOUNCINESS * velocity.y;
                        bounce.Play();
                        standStill = false;
                    }
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

    void CheckPlayerIsDefeated(Player pPlayer)
    {
        if (pPlayer.GetLives() == 0)
        {
            if (pPlayer.state == Player.PlayerSelection.PLAYER_ONE)
            {
                pPlayer.MakePlayer1Defeated();
            }
            else if (pPlayer.state == Player.PlayerSelection.PLAYER_TWO)
            {
                pPlayer.MakePlayer2Defeated();
            }

            pPlayer.LateDestroy();
            pPlayer = null;
            LateDestroy();
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
                _player.DeductAmountLives(1);

                CheckPlayerIsDefeated(_player);

                hitPlayer = false;

                playerHit.Play();
            }
            else if (_player.state == Player.PlayerSelection.PLAYER_TWO && state == BallState.THROWN)
            {
                pickedUp = false;
                thrown = false;
                _player.DeductAmountLives(1);

                CheckPlayerIsDefeated(_player);

                hitPlayer = false;

                playerHit.Play();
            }
        }
    }
}