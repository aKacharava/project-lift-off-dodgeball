using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Menu : GameObject
{
    enum MenuState
    {
        START_SCREEN,
        LEVEL_SELECT_SCREEN,
        VICTORY_SCREEN
    }

    Button levelSelect;
    Button startGame;
    Button victory;
    Button player1Controls;
    Button player2Controls;
    Sprite logo;
    Sprite levelSelectionScreen;
    Level level;
    Sound intro;
    Sound core;
    Sound winner;
    SoundChannel musicChannel;

    float scaleSpeed = 0.03f;
    int speed = 15;

    MenuState state;

    public Menu() : base()
    {
        levelSelect = new Button("Press SPACE to start the game", -120);
        levelSelect.x = (game.width) / 2;
        levelSelect.y = (game.height) / 3 * 2;
        levelSelect.EnableFade(true);
        AddChild(levelSelect);

        state = MenuState.START_SCREEN;

        logo = new Sprite("img/objects/logo.png");
        logo.scaleX = 0f;
        logo.x = 0;
        logo.y = game.height / 3;
        AddChild(logo);

        intro = new Sound("sounds/intro.mp3", false, true);
        core = new Sound("sounds/core-repeatable.mp3", true, true);
        winner = new Sound("sounds/winner.mp3", false, true);
        musicChannel = intro.Play();
    }

    void Update()
    {
        HandleState();
    }

    void HandleState()
    {
        switch (state)
        {
            case MenuState.START_SCREEN:
                HandleStartScreenState();
                break;
            case MenuState.LEVEL_SELECT_SCREEN:
                HandleLevelSelectState();
                break;
            case MenuState.VICTORY_SCREEN:
                HandleVictoryState();
                break;
        }
    }

    void HandleStartScreenState()
    {
        if (logo == null)
        {
            levelSelect = new Button("Press SPACE to start the game", -120);
            levelSelect.x = (game.width) / 2;
            levelSelect.y = (game.height) / 3 * 2;
            levelSelect.EnableFade(true);
            AddChild(levelSelect);

            logo = new Sprite("img/objects/logo.png");
            logo.scaleX = 0f;
            logo.x = 0;
            logo.y = game.height / 3;
            AddChild(logo);

            musicChannel = intro.Play();
        }

        if (musicChannel.IsPlaying == false)
        {
            musicChannel = core.Play();
        }

        ScaleEffect();
        MoveLogo();

        if (Input.GetKeyDown(Key.SPACE))
        {
            LevelSelect();
            state = MenuState.LEVEL_SELECT_SCREEN;
        }
    }

    bool introPlaying;

    void HandleLevelSelectState()
    {
        introPlaying = true;

        if (musicChannel.IsPlaying == false)
        {
            introPlaying = false;
            musicChannel = core.Play();
        }

        if (Input.GetKeyDown(Key.ONE) && startGame != null)
        {
            StartGame("levels/jungle-stage.tmx");
            state = MenuState.VICTORY_SCREEN;
        }
        else if (Input.GetKeyDown(Key.TWO) && startGame != null)
        {
            StartGame("levels/cherry-blossom-stage.tmx");
            state = MenuState.VICTORY_SCREEN;
        }
    }

    void HandleVictoryState()
    {
        if (musicChannel.IsPlaying == true && introPlaying == true)
        {
            introPlaying = false;
            musicChannel.Stop();
            musicChannel = core.Play();
        }

        if (victory == null)
        {
            if (Player.player1IsDefeated == true)
            {
                if (level != null)
                {
                    level.LateDestroy();
                    level = null;
                }

                if (musicChannel.IsPlaying == true)
                {
                    musicChannel.Stop();
                    musicChannel = winner.Play();
                }

                victory = new Button("Player 2 has won! - Press SPACE to reset the game", 400, 500, 14);
                AddChild(victory);
            }
            else if (Player.player2IsDefeated == true)
            {
                if (level != null)
                {
                    level.LateDestroy();
                    level = null;
                }

                if (musicChannel.IsPlaying == true)
                {
                    musicChannel.Stop();
                    musicChannel = winner.Play();
                }

                victory = new Button("Player 1 has won! - Press SPACE to reset the game", 400, 500, 14);
                AddChild(victory);
            }
        }
        else if(victory != null)
        {
            if (Input.GetKeyDown(Key.SPACE))
            {
                if (musicChannel.IsPlaying == true)
                {
                    musicChannel.Stop();
                }

                victory.LateDestroy();
                victory = null;
                state = MenuState.START_SCREEN;
            }
        }
    }

    void ScaleEffect()
    {
        logo.scaleX += scaleSpeed;

        if (logo.scaleX > 1)
        {
            scaleSpeed = 0;
            logo.scaleX = 1;
        }
    }

    void MoveLogo()
    {
        logo.x += speed;

        if (logo.x > game.width / 3 + 100)
        {
            speed = 0;
            logo.x = game.width / 3 + 100;
        }
    }

    void LevelSelect()
    {
        if (logo != null)
        {
            logo.LateDestroy();
            logo = null;
        }
        
        levelSelect.LateDestroy();
        levelSelect = null;

        levelSelectionScreen = new Sprite("img/backgrounds/UI/ui.png");
        AddChild(levelSelectionScreen);

        startGame = new Button("Press 1 for Jungle Map OR Press 2 for Blossom Forrest Map", 300, 160, 14);
        startGame.EnableFade(true);
        AddChild(startGame);

        player1Controls = new Button("Move - A/D | Jump - W | Shoot - SPACE", 50, 700, 9);
        player1Controls.EnableFade(false);
        AddChild(player1Controls);

        player2Controls = new Button("Move - LEFT/RIGHT | Jump - UP | Shoot - ENTER", 900, 710, 9);
        player2Controls.EnableFade(false);
        AddChild(player2Controls);
    }

    void StartGame(string levelFileName)
    {
        startGame.LateDestroy();
        startGame = null;

        levelSelectionScreen.LateDestroy();
        levelSelectionScreen = null;

        player1Controls.LateDestroy();
        player1Controls = null;

        player2Controls.LateDestroy();
        player2Controls = null;

        level = new Level(levelFileName);
        game.AddChild(level);
    }
}