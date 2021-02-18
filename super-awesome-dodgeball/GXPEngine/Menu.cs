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
        LEVEL_SELECT_SCREEN
    }

    Button levelSelect;
    Button startGame;
    Sprite logo;
    Sprite levelSelectionScreen;
    Level level;

    float scaleSpeed = 0.03f;
    int speed = 15;

    MenuState state;

    public Menu() : base()
    {
        levelSelect = new Button("Press SPACE to start the game", -120);
        AddChild(levelSelect);
        levelSelect.x = (game.width) / 2;
        levelSelect.y = (game.height) / 3 * 2;

        state = MenuState.START_SCREEN;

        logo = new Sprite("img/objects/logo.png");
        logo.scaleX = 0f;
        logo.x = 0;
        logo.y = game.height / 3;
        AddChild(logo);
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
        }
    }

    void HandleStartScreenState()
    {
        ScaleEffect();
        MoveLogo();

        if (Input.GetKeyDown(Key.SPACE))
        {
            LevelSelect();
            state = MenuState.LEVEL_SELECT_SCREEN;
        }
    }

    void HandleLevelSelectState()
    {
        if (Input.GetKeyDown(Key.ONE) && startGame != null)
        {
            StartGame("levels/jungle-stage.tmx");
        }
        else if (Input.GetKeyDown(Key.TWO) && startGame != null)
        {
            StartGame("levels/cherry-blossom-stage.tmx");
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
        logo.LateDestroy();
        logo = null;
        levelSelect.LateDestroy();
        levelSelect = null;
        levelSelectionScreen = new Sprite("img/backgrounds/UI/ui.png");
        AddChild(levelSelectionScreen);
        startGame = new Button("Press 1 for Jungle Map OR Press 2 for Blossom Forrest Map", game.width / 2 - 300, game.height / 2 + 40);
        AddChild(startGame);
    }

    void StartGame(string levelFileName)
    {
        startGame.LateDestroy();
        startGame = null;
        levelSelectionScreen.LateDestroy();
        levelSelectionScreen = null;
        level = new Level(levelFileName);
        AddChild(level);
    }
}