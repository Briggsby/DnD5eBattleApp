using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BugsbyEngine;

namespace DnD5eBattleApp;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    EngManager engManager;
    DnDManager dndManager;
    Library library;
    ContextMenuTextureSet contextMenuTextureSet;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        SetInitialVariables();
        SetCommands();

        base.Initialize();

    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        EngManager.spriteBatch = _spriteBatch;

        // TODO: use this.Content to load your game content here

        // TODO: This should be done as a test or separate script, and take folder as a parameter
        SchemaHandler.ExtractSchemas("E:/Programming/DnD5eBattleApp/DnDEngine/Libraries/Schemas");
    
        //LoadDebugMenuContent();
        LoadBoardTextures();
        LoadBlockTextures();
        LoadContextMenuTextures(); //also default font
        LoadStatDisplay();
        LoadDisplayOptions();
        LoadScrollMenuTextures();
        Creature.BaseCommonTexture = Content.Load<Texture2D>("base");
        dndManager = new DnDManager(GetMonsterTextures(), contextMenuTextureSet, boardTextureSet);



        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        //ScrollMenuTest();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);

        // TODO: Add your update logic here
        engManager.Update(gameTime);
        dndManager.Update();

        //DebugMenuCheck();
        LayBlocks();

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here


        base.Draw(gameTime);

        engManager.BeginSpriteBatch();
        engManager.Draw();
        _spriteBatch.End();
    }

    public void SetInitialVariables()
    {
        engManager = new EngManager(_spriteBatch, _graphics, new Vector2(800, 600), true);
        editingMode = false;
        layingGround = false;
    }

    #region Controls

    public void SetCommands()
    {
    }

    #endregion
    
    #region Placing Characters

    #endregion

    #region Editing Mode

    public bool editingMode;
    public bool layingGround;
    public List<Texture2D> blockTextures;
    public int blockTextureCount = 3;
    public int activeTexture = 0;
    public int placeSize = 1;
    public Vector2 blockSize = new Vector2(50, 50);

    public void LoadBlockTextures()
    {
        blockTextures = new List<Texture2D>();
        for (int i = 0; i < blockTextureCount; i++)
        {
            string textureFileName = "blockTexture" + i.ToString();
            blockTextures.Add(Content.Load<Texture2D>(textureFileName));
        }
    }

    public void LayBlocks()
    {
        if (!layingGround || !editingMode)
        {
            return;
        }
        if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.LeftMouseDown))
        {
            if ((!EngManager.controls.mouseOverContextMenu && EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.MouseActive)))
            {
            }
        }

        if (EngManager.controls.currentMouseState.ScrollWheelValue > EngManager.controls.previousMouseState.ScrollWheelValue)
        {
            activeTexture++;
            if (activeTexture >= blockTextureCount)
            {
                activeTexture = 0;
            }
        }
    }

    #endregion

    #region SquareBoard
    public BoardTextureSet boardTextureSet;

    public GameObject boardObj;

    public void LoadBoardTextures()
    {
        boardTextureSet = new BoardTextureSet();
        boardTextureSet.tileTexture = Content.Load<Texture2D>("boardTile");
        boardTextureSet.tileHoverTexture = Content.Load<Texture2D>("hoverBoardSquareOverlay");
    }

    #endregion

    
    void ScrollMenuTest()
    {
        ContextMenuTemplate scrollTestTemplate = new ContextMenuTemplate();
        scrollTestTemplate.tags = new List<List<string>>() { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
        scrollTestTemplate.texts = new List<string>() { "Test", "Test2", "Test3", "Test4", "Test5", "Test6", "Test7", "Test8", "Test9", "Test10", "Test11", "Test12", "Test13", "Test14", "Test15", "Test16" };
        scrollTestTemplate.textures = ButtonTextures.FromList(contextMenuTextureSet.baseTextures);
        scrollTestTemplate.numberOfColumns = 1;

        List<ButtonTextures> scrollTextures = new List<ButtonTextures>(new ButtonTextures[5]);
        scrollTextures[(int)ScrollAreaTextures.ArrowDown] = new ButtonTextures(Content.Load<Texture2D>("scrollDownArrow"), Content.Load<Texture2D>("scrollDownArrowInactive"), Content.Load<Texture2D>("scrollDownArrowInactive"), Content.Load<Texture2D>("scrollDownArrowInactive"), Content.Load<Texture2D>("scrollDownArrowInactive"));
        scrollTextures[(int)ScrollAreaTextures.ArrowUp] = new ButtonTextures( Content.Load<Texture2D>("scrollUpArrow"), Content.Load<Texture2D>("scrollUpArrowInactive"), Content.Load<Texture2D>("scrollUpArrowInactive"), Content.Load<Texture2D>("scrollUpArrowInactive"), Content.Load<Texture2D>("scrollUpArrowInactive") );
        scrollTextures[(int)ScrollAreaTextures.Background] = new ButtonTextures();
        scrollTextures[(int)ScrollAreaTextures.Scroller] = new ButtonTextures();
        scrollTextures[(int)ScrollAreaTextures.ScrollerBackground] = new ButtonTextures();

        ScrollMenuTemplate scrollMenuTemplate = new ScrollMenuTemplate(scrollTestTemplate);
        scrollMenuTemplate.downArrow = scrollTextures[(int)ScrollAreaTextures.ArrowDown];
        scrollMenuTemplate.upArrow = scrollTextures[(int)ScrollAreaTextures.ArrowUp];
        scrollMenuTemplate.numberOfButtons = 6;

        //new ScrollArea(scrollTestTemplate, new List<Vector2>() { new Vector2(500, 450), new Vector2(50, 30), new Vector2(50, 130) }, scrollTextures, new Vector2(100, 20), new Vector2(100, 20), 4);
        //new ScrollMenu(scrollMenuTemplate, new Vector2(500, 500));
        new DropDownMenu(scrollMenuTemplate, new Vector2(500, 300), scrollTestTemplate.textures, new Vector2(600, 100), "Test Dropdown");

    }

    #region Unit Textures

    public Dictionary<string, Texture2D> GetMonsterTextures()
    {
        // TODO: Sort out
        return new Dictionary<string, Texture2D>()
        {
            {"base",Content.Load<Texture2D>("base") },
            {"barbarian", Content.Load<Texture2D>("barbarian") }
        };
    }

    #endregion

    #region Stat Display

    public void LoadStatDisplay()
    {
        Dictionary<StatDisplay.StatsDisplay, Vector2> vectors = new Dictionary<StatDisplay.StatsDisplay, Vector2>()
        {
            {StatDisplay.StatsDisplay.Picture, new Vector2(-30, -40) },
            {StatDisplay.StatsDisplay.MaxHP, new Vector2(75, -245) },
            {StatDisplay.StatsDisplay.HPOutOfMax, new Vector2(75, -210 )},
            {StatDisplay.StatsDisplay.Name, new Vector2(-165, 20) },
            {StatDisplay.StatsDisplay.AmountMoved, new Vector2(75, -175) },
            {StatDisplay.StatsDisplay.StatStr, new Vector2(-135, 67) },
            {StatDisplay.StatsDisplay.StatDex, new Vector2(-135, 103) },
            {StatDisplay.StatsDisplay.StatCon, new Vector2(-135, 140) },
            {StatDisplay.StatsDisplay.StatInt, new Vector2(61, 67) },
            {StatDisplay.StatsDisplay.StatWis, new Vector2(61, 103) },
            {StatDisplay.StatsDisplay.StatCha, new Vector2(61, 140) },
        };

        StatDisplay.SetInfo(location: new Vector2(900, 400) , texture: Content.Load<Texture2D>("StatMenu") , statLocations: vectors, font : EngManager.defaultFont, textSize: new Vector2(1.8f, 1.8f) , pictureSize: new Vector2(200,200), color: Color.Red);
    }

    #endregion

    #region Option display 
    DisplayOptionsTextureSet defaultDisplayOptionsTextureSet;

    public void LoadDisplayOptions()
    {
        defaultDisplayOptionsTextureSet = new DisplayOptionsTextureSet(Content.Load<Texture2D>("DisplayOptionsBox"));
        defaultDisplayOptionsTextureSet.positions = new List<Vector2>(new Vector2[Enum.GetNames(typeof(DisplayOptionsParts)).Length]);
                            
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.Body] = new Vector2(800,300);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.CancelButton] = new Vector2(0,150);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.CancelButtonSize] = new Vector2(100, 50);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.NoButton] = new Vector2(200, 100);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.NoButtonSize] = new Vector2(75, 50);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.OptionScrollMenu] = new Vector2(0, 100);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.OptionScrollMenuSize] = new Vector2(300, 75);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.Question] = new Vector2(0,-100);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.YesButton] = new Vector2(-200, 100);
        defaultDisplayOptionsTextureSet.positions[(int)DisplayOptionsParts.YesButtonSize] = new Vector2(75, 50);
                            
        defaultDisplayOptionsTextureSet.yesButtonTextures = new List<Texture2D>(contextMenuTextureSet.baseTextures);
        defaultDisplayOptionsTextureSet.noButtonTextures = new List<Texture2D>(contextMenuTextureSet.baseTextures);
        defaultDisplayOptionsTextureSet.cancelButtonTextures = new List<Texture2D>(contextMenuTextureSet.baseTextures);
        defaultDisplayOptionsTextureSet.fonts = new List<SpriteFont>( new SpriteFont[Enum.GetNames(typeof(DisplayOptionsParts)).Length] );
        defaultDisplayOptionsTextureSet.fonts[(int)DisplayOptionsParts.Question] = EngManager.defaultFont;
        defaultDisplayOptionsTextureSet.scrollMenuTextures = new List<Texture2D>() { };

        DisplayOptions.baseDisplayOptionsTextureSet = defaultDisplayOptionsTextureSet;

        DisplayOptionsTextureSet opportunityAttackSet = new DisplayOptionsTextureSet(defaultDisplayOptionsTextureSet);
        opportunityAttackSet.cancelText = "Cancel Move";

        OpportunityAttackOption.opportunityAttackTextureSet = opportunityAttackSet;
        OldFeat.optionsDisplayTextures = opportunityAttackSet;
    }

    #endregion

    #region ContextMenu Textures

    public void LoadContextMenuTextures()
    {
        contextMenuTextureSet = new ContextMenuTextureSet();

        contextMenuTextureSet.baseFont = Content.Load<SpriteFont>("debugMenuFont");

        EngManager.defaultFont = contextMenuTextureSet.baseFont;

        contextMenuTextureSet.baseTextures = new List<Texture2D>(new Texture2D[Enum.GetNames(typeof(ButtonTexture)).Length]);
        contextMenuTextureSet.baseTextures[(int)ButtonTexture.BaseTexture] = Content.Load<Texture2D>("debugMenuBaseTexture");


        contextMenuTextureSet.baseTextures[(int)ButtonTexture.HoveredTexture] = Content.Load<Texture2D>("debugMenuMouseOverTexture");
        contextMenuTextureSet.baseTextures[(int)ButtonTexture.PressedTexture] = Content.Load<Texture2D>("debugMenuClickedTexture");
        contextMenuTextureSet.baseTextures[(int)ButtonTexture.PressedHoveredTexture] = Content.Load<Texture2D>("debugMenuClickedMouseOverTexture");
        contextMenuTextureSet.baseTextures[(int)ButtonTexture.InactiveTexture] = Content.Load<Texture2D>("debugMenuClickedTexture");

        contextMenuTextureSet.blankTileTextures = new List<Texture2D>(contextMenuTextureSet.baseTextures);
    }

    #endregion

    #region Scroll Menu
    List<Texture2D> scrollMenuTextures;

    public void LoadScrollMenuTextures()
    {
        
    }

    #endregion

    #region Unloading
    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
        // TODO: Unload any non ContentManager content here
    }

    #endregion

    /*
    #region debug


    #region DebugMenu in game

    public class DebugMenu : ContextMenu
    {
        Game1 game;

        public DebugMenu(Game1 game, Vector2 position, List<Texture2D> textures, List<string> texts, SpriteFont font, List<List<string>> tags) : base(position, textures, texts, font, tags, null)
        {
            this.game = game;
        }

        public override void ButtonPress(List<string> tags, ContextMenuItem contextMenuItem)
        {
            base.ButtonPress(tags, contextMenuItem);

            foreach (string t in tags)
            {
                if (t == "StartEditingMode")
                {
                    game.editingMode = true;
                }
                if (t == "CancelEditingMode")
                {
                    game.editingMode = false;
                }
                if (t == "LaySquareBoard")
                {
                }
                if (t == "DestroyBoard")
                {
                }
                if (t == "LayGround")
                {
                    game.layingGround = !game.layingGround;
                }
            }


            game.currentDebugMenu.DestroyAndChildren();
            game.SpawnDebugMenu(game.currentDebugMenu.transform.Position());
            game.debugMenuJustSwitched = true;
        }
    }

    #region Loading Content

    public Texture2D debugMenuBaseTexture;
    public Texture2D debugMenuMouseOverTexture;
    public Texture2D debugMenuClickedTexture;
    public Texture2D debugMenuClickedOverTexture;
    public Texture2D debugMenuInactiveTexture;
    public List<Texture2D> debugMenuTextures;

    public SpriteFont debugMenuFont;

    public ContextMenuTemplate debugMenuTemplate;

    public void LoadDebugMenuContent()
    {
        debugMenuBaseTexture = Content.Load<Texture2D>("debugMenuBaseTexture");
        debugMenuMouseOverTexture = Content.Load<Texture2D>("debugMenuMouseOverTexture");
        debugMenuClickedTexture = Content.Load<Texture2D>("debugMenuClickedTexture");
        debugMenuClickedOverTexture = Content.Load<Texture2D>("debugMenuClickedMouseOverTexture");
        debugMenuInactiveTexture = Content.Load<Texture2D>("debugMenuClickedTexture");

        debugMenuTextures = new List<Texture2D>() { debugMenuBaseTexture, debugMenuMouseOverTexture, debugMenuClickedTexture, debugMenuClickedOverTexture, debugMenuInactiveTexture };

        debugMenuFont = Content.Load<SpriteFont>("debugMenuFont");

        debugMenuTemplate = new ContextMenuTemplate();
        debugMenuTemplate.textures = new List<Texture2D>(debugMenuTextures);
        debugMenuTemplate.textures[0] = debugMenuBaseTexture;
        debugMenuTemplate.textures[1] = debugMenuMouseOverTexture;
        debugMenuTemplate.textures[2] = debugMenuClickedTexture;
        debugMenuTemplate.textures[3] = debugMenuClickedOverTexture;
        debugMenuTemplate.textures[4] = debugMenuInactiveTexture;
        debugMenuTemplate.font = debugMenuFont;

    }

    #endregion

    #region Spawning Debug Menus

    public ContextMenu currentDebugMenu;
    public bool debugMenuJustSwitched = false;

    public void DebugMenuCheck()
    {

        #region Mouse

        if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.RightMouseJustDown))
        {
            WorkOutDebugMenuOptions();
            if (currentDebugMenu != null)
            {
                currentDebugMenu.DestroyAndChildren();
            }
            SpawnDebugMenu(EngManager.controls.MousePosition);
        }
        if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.LeftMouseJustDown) && currentDebugMenu != null && !currentDebugMenu.hitbox.CheckMouseOver())
        {
            if (!debugMenuJustSwitched)
            {
                currentDebugMenu.DestroyAndChildren();
                currentDebugMenu = null;
            }
        }

        #endregion

        if (debugMenuJustSwitched)
        {
            debugMenuJustSwitched = false;
        }
    }

    public void SpawnDebugMenu(Vector2 position)
    {
        WorkOutDebugMenuOptions();
        if (currentDebugMenu != null)
        {
            currentDebugMenu.DestroyAndChildren();
        }
        currentDebugMenu = new DebugMenu(this, position, debugMenuTextures, debugMenuTexts, debugMenuFont, debugMenuTags);
        debugMenuTemplate.texts = debugMenuTexts;
        debugMenuTemplate.tags = debugMenuTags;
        currentDebugMenu.GiveChildMenus("TestChildMenu", debugMenuTemplate);
    }

    #endregion

    #region Debug Menu Options
    public List<string> debugMenuTexts;
    public List<List<string>> debugMenuTags;

    public void WorkOutDebugMenuOptions()
    {
        debugMenuTexts = new List<string>();
        debugMenuTags = new List<List<string>>();

        if (editingMode)
        {
            debugMenuTexts.Add("Cancel Editing Mode");
            debugMenuTags.Add(new List<string>() { "CancelEditingMode" });

            if (boardObj == null)
            {
                debugMenuTexts.Add("Lay Square Board");
                debugMenuTags.Add(new List<string>() { "LaySquareBoard" });
            }

            if (boardObj != null)
            {
                debugMenuTexts.Add("Destroy Board");
                debugMenuTags.Add(new List<string>() { "DestroyBoard" });
            }

            debugMenuTexts.Add("Test Child Menu");
            debugMenuTags.Add(new List<string>() { "TestChildMenu" });

            if (boardObj != null)
            {
                if (!layingGround)
                {
                    debugMenuTexts.Add("Lay Ground");
                    debugMenuTags.Add(new List<string>() { "LayGround" });
                }
                else
                {
                    debugMenuTexts.Add("Stop Laying Ground");
                    debugMenuTags.Add(new List<string>() { "LayGround" });
                }
            }
        }
        else
        {
            debugMenuTexts.Add("Start Editing Mode");
            debugMenuTags.Add(new List<string>() { "StartEditingMode" });
        }

        WorkOutDebugMenuOptionsBoardChoices();
    }

    public void WorkOutDebugMenuOptionsBoardChoices()
    {
    }

    #endregion

    #endregion
    */

}
