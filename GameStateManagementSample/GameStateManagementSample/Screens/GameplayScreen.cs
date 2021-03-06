#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using GameStateManagement;
using ChaseCameraSample;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        KeyboardState lastKeyboardState = new KeyboardState();
        GamePadState lastGamePadState1 = new GamePadState();
        GamePadState lastGamePadState2 = new GamePadState();
        MouseState lastMousState = new MouseState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();
        MouseState currentMouseState = new MouseState();


        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;

        Ship ship,ship2;
        ChaseCamera camera,camera2;
        Viewport topViewport, bottomViewport;

        EnvironmentMapEffect envEffect1;
        EnvironmentMapEffect envEffect2;
        TextureCube textureCube1;
        TextureCube textureCube2;

        Vector3 ship1Pos, ship2Pos;

        //AUDIO STUFF
        AudioEmitter shipEmit1, shipEmit2;
        AudioListener shipListen1, shipListen2;
        Cue FxCue;
        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;
        AudioCategory acSFX;
        AudioCategory acMusic;
        bool musicPlaying = false;

        Model rockModel;
        Model skyBoxModel;
        Model shipModel,shipModel2;
        Model groundModel;
        Model cubeModel;
        Model bulletModel;

        Vector3[] rockPos = new Vector3[100];
        
        bool cameraSpringEnabled = true;
        bool camera2SpringEnabled = true;

        

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            shipEmit1 = new AudioEmitter();
            shipEmit2 = new AudioEmitter();

            

            // Create the chase camera
            camera = new ChaseCamera();
            camera2 = new ChaseCamera();

            // Set the camera offsets
            camera.DesiredPositionOffset = new Vector3(0.0f, 2000.0f, 3500.0f);
            camera.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);

            // Set camera perspective
            camera.NearPlaneDistance = 10.0f;
            camera.FarPlaneDistance = 10000000.0f;

            // Set the camera offsets
            camera2.DesiredPositionOffset = new Vector3(0.0f, 2000.0f, 3500.0f);
            camera2.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);

            // Set camera perspective
            camera2.NearPlaneDistance = 10.0f;
            camera2.FarPlaneDistance = 10000000.0f;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                topViewport = ScreenManager.GraphicsDevice.Viewport;
                bottomViewport = ScreenManager.GraphicsDevice.Viewport;

                if (ScreenManager.ScreenHorizontal == true)
                {
                    topViewport.Height = topViewport.Height / 2;
                    bottomViewport.Height = bottomViewport.Height / 2;
                    bottomViewport.Y = topViewport.Height;

                    camera.AspectRatio = (float)ScreenManager.GraphicsDevice.Viewport.Width /
                        (ScreenManager.GraphicsDevice.Viewport.Height / 2);
                    camera2.AspectRatio = (float)ScreenManager.GraphicsDevice.Viewport.Width /
                        (ScreenManager.GraphicsDevice.Viewport.Height / 2);

                    camera.FieldOfView = MathHelper.ToRadians(45);
                    camera2.FieldOfView = MathHelper.ToRadians(45);
                }
                else
                {
                    topViewport.Width = topViewport.Width / 2;
                    bottomViewport.Width = bottomViewport.Width / 2;
                    bottomViewport.X = topViewport.Width;

                    camera.AspectRatio = (float)(ScreenManager.GraphicsDevice.Viewport.Width / 2) /
                        ScreenManager.GraphicsDevice.Viewport.Height;
                    camera2.AspectRatio = (float)(ScreenManager.GraphicsDevice.Viewport.Width / 2) /
                        ScreenManager.GraphicsDevice.Viewport.Height;

                    camera.FieldOfView = MathHelper.ToRadians(60);
                    camera2.FieldOfView = MathHelper.ToRadians(60);
                }

                gameFont = content.Load<SpriteFont>("gamefont");
                rockModel = content.Load<Model>("Rock");
                shipModel = content.Load<Model>("SpaceShip1");
                shipModel2 = content.Load<Model>("SpaceShip2");
                groundModel = content.Load<Model>("Ground");
                cubeModel = content.Load<Model>("cube");
                bulletModel = content.Load<Model>("Cone");
                skyBoxModel = content.Load<Model>("Space_SkyBox");
                audioEngine = ScreenManager.AudioEngine;
                soundBank = ScreenManager.SoundBank;
                waveBank = ScreenManager.WaveBank;
                acSFX = audioEngine.GetCategory("SFX");
                acMusic = audioEngine.GetCategory("Music");

                // Environmental Map Effect for Player 1
                envEffect1 = new EnvironmentMapEffect(ScreenManager.GraphicsDevice);
                envEffect1.Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4, ScreenManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
                envEffect1.View = Matrix.CreateLookAt(
                    new Vector3(2, 3, 32), Vector3.Zero, Vector3.Up);
                textureCube1 = new TextureCube(ScreenManager.GraphicsDevice, 256, false, SurfaceFormat.Color);
                Color[] facedata1 = new Color[256 * 256];
                for (int i = 0; i < 6; i++)
                {
                    envEffect1.Texture = content.Load<Texture2D>("skybox" + i.ToString());
                    envEffect1.Texture.GetData<Color>(facedata1);
                    textureCube1.SetData<Color>((CubeMapFace)i, facedata1);
                }
                envEffect1.Texture = (shipModel.Meshes[0].Effects[0] as EnvironmentMapEffect).Texture;
                envEffect1.EnvironmentMap = textureCube1;
                envEffect1.EnableDefaultLighting();
                envEffect1.EnvironmentMapAmount = 1.0f;
                envEffect1.FresnelFactor = 1.0f;
                envEffect1.EnvironmentMapSpecular = Vector3.Zero;

                // Environmental Map Effect for Player 2
                envEffect2 = new EnvironmentMapEffect(ScreenManager.GraphicsDevice);
                envEffect2.Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4, ScreenManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);
                envEffect2.View = Matrix.CreateLookAt(
                    new Vector3(2, 3, 32), Vector3.Zero, Vector3.Up);
                textureCube2 = new TextureCube(ScreenManager.GraphicsDevice, 256, false, SurfaceFormat.Color);
                Color[] facedata2 = new Color[256 * 256];
                for (int i = 0; i < 6; i++)
                {
                    envEffect2.Texture = content.Load<Texture2D>("skybox" + i.ToString());
                    envEffect2.Texture.GetData<Color>(facedata2);
                    textureCube2.SetData<Color>((CubeMapFace)i, facedata2);
                }
                envEffect2.Texture = (shipModel2.Meshes[0].Effects[0] as EnvironmentMapEffect).Texture;
                envEffect2.EnvironmentMap = textureCube2;
                envEffect2.EnableDefaultLighting();
                envEffect2.EnvironmentMapAmount = 1.0f;
                envEffect2.FresnelFactor = 1.0f;
                envEffect2.EnvironmentMapSpecular = Vector3.Zero;

                //audioEngine = ScreenManager.AudioEngine;
                //soundBank = ScreenManager.SoundBank;
                //waveBank = ScreenManager.WaveBank;

                if (ScreenManager.AudioEnabled == true)
                {
                    acSFX.SetVolume(ScreenManager.SFXVolume);
                    acMusic.SetVolume(ScreenManager.AudioVolume);
                }
                else
                {
                    acSFX.SetVolume(0);
                    acMusic.SetVolume(0);
                }

                FxCue = soundBank.GetCue("ShotFx");
                //FxCue.Apply3D(shipListen1, shipEmit1);

                ship1Pos = new Vector3(10000,350,10000);
                ship2Pos = new Vector3(100, 350, 100);

                // Create shiplllllllllllllllllllllllllllll
                ship = new Ship(ScreenManager.GraphicsDevice,ship1Pos,soundBank);
                ship2 = new Ship(ScreenManager.GraphicsDevice,ship2Pos,soundBank);
                //ship2.Position = new Vector3(100, 100, 100);

                RandomRockSpawner();

                UpdateCameraChaseTarget(ship,camera);
                UpdateCameraChaseTarget(ship2,camera2);

                camera.Reset();
                camera2.Reset();

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }

        private void RandomRockSpawner()
        {
            for (int i = 0; i < rockPos.Length; i++)
            {
                float randX = random.Next(-500000, 500000);
                float randY = random.Next(-500000, 500000);
                float randZ = random.Next(-500000, 500000);

                if (randX >= ship.Position.X - 500 && randX <= ship.Position.X)
                    randX -= 500;
                else if (randX >= ship.Position.X && randX <= ship.Position.X + 500)
                    randX += 500;

                if (randY >= ship.Position.Y - 500 && randY <= ship.Position.Y)
                    randY -= 500;
                else if (randY >= ship.Position.Y && randY <= ship.Position.Y + 500)
                    randY += 500;

                if (randZ >= ship.Position.Z - 500 && randZ <= ship.Position.Z)
                    randZ -= 500;
                else if (randZ >= ship.Position.Z && randZ <= ship.Position.Z + 500)
                    randZ += 500;

                if ((randX >= ship.Position.X - 500 && randX <= ship.Position.X) && (randY >= ship.Position.Y - 500 && randY <= ship.Position.Y) && (randZ >= ship.Position.Z - 500 && randZ <= ship.Position.Z) ||
                    (randX >= ship2.Position.X - 500 && randX <= ship2.Position.X) && (randY >= ship2.Position.Y - 500 && randY <= ship2.Position.Y) && (randZ >= ship2.Position.Z - 500 && randZ <= ship2.Position.Z))
                {
                    randX -= 500;
                }
                else if ((randX >= ship.Position.X + 500 && randX <= ship.Position.X) && (randY >= ship.Position.Y + 500 && randY <= ship.Position.Y) && (randZ >= ship.Position.Z + 500 && randZ <= ship.Position.Z) ||
                        (randX >= ship2.Position.X + 500 && randX <= ship2.Position.X) && (randY >= ship2.Position.Y + 500 && randY <= ship2.Position.Y) && (randZ >= ship2.Position.Z + 500 && randZ <= ship2.Position.Z))
                {
                    randX += 500;
                }

                rockPos[i] = new Vector3(randX, randY, randZ);
            }
        }

        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

            if (ScreenManager.ScreenHorizontal == true)
            {
                topViewport.Height *= 2;
                topViewport.Y = 0;
                ScreenManager.GraphicsDevice.Viewport = topViewport;
            }
            else
            {
                topViewport.Width *= 2;
                topViewport.X = 0;
                ScreenManager.GraphicsDevice.Viewport = topViewport;
            }

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                    lastKeyboardState = currentKeyboardState;
                    //lastGamePadState = currentGamePadState;
                    lastMousState = currentMouseState;

#if WINDOWS_PHONE
            currentKeyboardState = new KeyboardState();
#else
                    currentKeyboardState = Keyboard.GetState();
#endif

                for (PlayerIndex p = PlayerIndex.One; p <= PlayerIndex.Two; p++)
                {
                    switch(p)
                    {
                        case PlayerIndex.One: lastGamePadState2 = currentGamePadState;
                            break;
                        case PlayerIndex.Two: lastGamePadState1 = currentGamePadState;
                            break;
                    }

                    currentGamePadState = GamePad.GetState(p);
                    currentMouseState = Mouse.GetState();

                    bool touchTopLeft = currentMouseState.LeftButton == ButtonState.Pressed &&
                            lastMousState.LeftButton != ButtonState.Pressed &&
                            currentMouseState.X < ScreenManager.GraphicsDevice.Viewport.Width / 10 &&
                            currentMouseState.Y < ScreenManager.GraphicsDevice.Viewport.Height / 10;

                    //TEST
                    //shipEmit1.Position = ship.Position;
                    //shipListen1.Position = camera.Position;


#if Windows
                    // Pressing the A button or key toggles the spring behavior on and off
                    if (lastKeyboardState.IsKeyUp(Keys.A) &&
                        (currentKeyboardState.IsKeyDown(Keys.A)) ||
                        (lastGamePadState1.Buttons.A == ButtonState.Released &&
                        currentGamePadState.Buttons.A == ButtonState.Pressed) ||
                        touchTopLeft)
                    {
                        switch(p)
                        {
                            case PlayerIndex.One: camera2SpringEnabled = !camera2SpringEnabled;
                                break;
                            case PlayerIndex.Two: cameraSpringEnabled = !cameraSpringEnabled;
                                break;
                        }
                    }

                    lastGamePadState1 = currentGamePadState;
#else
                    switch(p)
                    {
                        case PlayerIndex.One:
                        if (lastGamePadState1.Buttons.A == ButtonState.Released &&
                            currentGamePadState.Buttons.A == ButtonState.Pressed)
                        {
                            camera2SpringEnabled = !camera2SpringEnabled;
                        }
                        break;
                        case PlayerIndex.Two:
                        if (lastGamePadState2.Buttons.A == ButtonState.Released &&
                            currentGamePadState.Buttons.A == ButtonState.Pressed)
                        {
                            cameraSpringEnabled = !cameraSpringEnabled;
                        }
                        break;
                    }
#endif
                }

                if (!musicPlaying)
                {
                    ScreenManager.MusicCue = soundBank.GetCue("GameMusic");
                    ScreenManager.MusicCue.Play();
                    musicPlaying = true;
                }
                if (!ScreenManager.MusicCue.IsPlaying)
                {
                    musicPlaying = false;
                    ScreenManager.MusicCue.Stop(AudioStopOptions.Immediate);
                }

                // Reset the ship on R key or right thumb stick clicked
                if (currentKeyboardState.IsKeyDown(Keys.R) ||
                    currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
                {
                    ship.Reset(ship1Pos);
                    camera.Reset();
                }

                // Update the ship
                ship.Update(gameTime, shipModel, cubeModel,bulletModel,ship2.World,2);
                ship2.Update(gameTime, shipModel, cubeModel, bulletModel, ship.World,1);

                // Update the camera to chase the new target
                UpdateCameraChaseTarget(ship,camera);
                UpdateCameraChaseTarget(ship2,camera2);

                // The chase camera's update behavior is the springs, but we can
                // use the Reset method to have a locked, spring-less camera
                if (cameraSpringEnabled)
                    camera.Update(gameTime);
                else
                    camera.Reset();

                if (camera2SpringEnabled)
                    camera2.Update(gameTime);
                else
                    camera2.Reset();

                //One of the ships dies
                if (ship.shipHealth <= 0 || ship2.shipHealth <= 0)
                {
                    ScreenManager.MusicCue.Stop(AudioStopOptions.Immediate);
                    ScreenManager.AudioEngine.Update();
                    ScreenManager.MainMenu.Stop(AudioStopOptions.Immediate);
                    ScreenManager.MainMenu = ScreenManager.SoundBank.GetCue("WinMusic");
                    ScreenManager.MainMenu.Play();
                    ScreenManager.AddScreen(new GameOverScreen(),PlayerIndex.One);
                }

            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {

            }
        }

        private void UpdateCameraChaseTarget(Ship ships,ChaseCamera camera)
        {
            camera.ChasePosition = ships.Position;
            camera.ChaseDirection = ships.Direction + ships.Up/5;
            camera.Up = ships.Up;
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.DarkBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            ScreenManager.GraphicsDevice.Viewport = bottomViewport;

            ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            for (int i = 0; i < ship.bullets.Length; i++)
            {
                if (ship.bullets[i].isAlive)
                {
                    DrawModel(bulletModel, Matrix.CreateScale(10) * Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)) * ship.bullets[i].World, camera);
                }
            }
            for (int i = 0; i < ship2.bullets.Length; i++)
            {
                if (ship2.bullets[i].isAlive)
                {
                    DrawModel(bulletModel, Matrix.CreateScale(10) * Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)) * ship2.bullets[i].World, camera);
                }
            }
            if (ScreenManager.shipChosenbool2 == false)         
            {
                DrawModel(shipModel2, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship2.World, envEffect2, camera);
                if (ScreenManager.shipChosenbool1 == false)
                {
                    DrawModel(shipModel2, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship.World, envEffect2, camera);
                }
                if (ScreenManager.shipChosenbool1 == true)
                {
                    DrawModel(shipModel, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship.World, envEffect1, camera);
                }
            }
            else if (ScreenManager.shipChosenbool2 == true)
            {
                DrawModel(shipModel, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship2.World, envEffect1, camera);
                if (ScreenManager.shipChosenbool1 == false)
                {
                    DrawModel(shipModel2, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship.World, envEffect2, camera);
                }
                if (ScreenManager.shipChosenbool1 == true)
                {
                    DrawModel(shipModel, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship.World, envEffect1, camera);
                }

            }
            //DrawModel(shipModel, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(10) * ship2.World, camera);
            DrawModel(skyBoxModel, Matrix.CreateScale(10000) * Matrix.Identity, camera);
            for (int i = 0; i < rockPos.Length; i++)
            {
                DrawModel(rockModel, Matrix.CreateScale(100) * Matrix.CreateTranslation(rockPos[i]), camera);
            }
            // DrawModel(groundModel, Matrix.Identity, camera);
            //DrawModel(shipModel, Matrix.CreateTranslation(50, 50, 100) * Matrix.CreateScale(10));

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "Health : " + ship2.shipHealth, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 50, 40), Color.White);
            //DrawString(gameFont, "Power  : " + (ship.bullets.Length - ship.currentBullet), new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width - 200, 40), Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            ScreenManager.GraphicsDevice.Viewport = topViewport;
            
            ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            for (int i = 0; i < ship.bullets.Length; i++)
            {
                if (ship.bullets[i].isAlive)
                {
                    DrawModel(bulletModel, Matrix.CreateScale(10) * Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)) * ship.bullets[i].World, camera2);
                }
            }
            for (int i = 0; i < ship2.bullets.Length; i++)
            {
                if (ship2.bullets[i].isAlive)
                {
                    DrawModel(bulletModel, Matrix.CreateScale(10) * Matrix.CreateRotationY(MathHelper.ToRadians(90.0f)) * ship2.bullets[i].World, camera2);
                }
            }
            if (ScreenManager.shipChosenbool1 == false)
            {
                DrawModel(shipModel2, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship.World, envEffect2, camera2);
                if (ScreenManager.shipChosenbool2 == true)
                {
                    DrawModel(shipModel, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship2.World, envEffect1, camera2);
                }
                if (ScreenManager.shipChosenbool2 == false)
                {
                    DrawModel(shipModel2, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship2.World, envEffect2, camera2);
                }
            }
            else if (ScreenManager.shipChosenbool1 == true)
            {
                DrawModel(shipModel, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship.World, envEffect1, camera2);
                if (ScreenManager.shipChosenbool2 == true)
                {
                    DrawModel(shipModel, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship2.World, envEffect1, camera2);
                }
                if (ScreenManager.shipChosenbool2 == false)
                {
                    DrawModel(shipModel2, Matrix.CreateRotationY(MathHelper.ToRadians(-90.0f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90.0f)) * Matrix.CreateScale(25) * ship2.World, envEffect2, camera2);
                }
            }
            DrawModel(skyBoxModel, Matrix.CreateScale(10000) * Matrix.Identity, camera2);
            for (int i = 0; i < rockPos.Length; i++)
            {
                DrawModel(rockModel, Matrix.CreateScale(100) * Matrix.CreateTranslation(rockPos[i]), camera2);
            }
            //DrawModel(groundModel, Matrix.Identity, camera2);
            //DrawModel(shipModel, Matrix.CreateTranslation(50, 50, 100) * Matrix.CreateScale(10));

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, "Health : " + ship.shipHealth, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X+50, 40), Color.White);
            //spriteBatch.DrawString(gameFont, "Power  : " + (ship2.bullets.Length - ship2.currentBullet), new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width - 200, 40), Color.White);

            spriteBatch.End();

            ScreenManager.GraphicsDevice.Viewport = ScreenManager.OriginalViewport;

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawModel(Model m, Matrix world, EnvironmentMapEffect be, ChaseCamera camera)
        {
            foreach (ModelMesh mm in m.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    be.View = camera.View;
                    be.Projection = camera.Projection;
                    be.World = world;
                    ScreenManager.GraphicsDevice.SetVertexBuffer(mmp.VertexBuffer, mmp.VertexOffset);
                    ScreenManager.GraphicsDevice.Indices = mmp.IndexBuffer;
                    be.CurrentTechnique.Passes[0].Apply();
                    ScreenManager.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList, 0, 0,
                        mmp.NumVertices, mmp.StartIndex, mmp.PrimitiveCount);
                }
            }
        }

        private void DrawModel(Model model, Matrix world,ChaseCamera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.SpecularColor = Color.WhiteSmoke.ToVector3();
                    ///effect.SpecularPower = 100.0f;
                    //effect.FogEnabled = true;
                    //effect.FogColor = Color.White.ToVector3();
                    //effect.FogStart = 999999.0f;
                    //effect.FogEnd = 1000000.0f;
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    // Use the matrices provided by the chase camera
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                mesh.Draw();
            }
        }
        #endregion
    }
}
