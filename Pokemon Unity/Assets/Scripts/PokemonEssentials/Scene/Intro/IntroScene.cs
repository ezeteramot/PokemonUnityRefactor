using System;
using System.Collections;
using System.Collections.Generic;
using PokemonUnity;
using PokemonEssentials.Interface;
using PokemonEssentials.Interface.Field;
using PokemonEssentials.Interface.Item;
using PokemonEssentials.Interface.PokeBattle;
using PokemonEssentials.Interface.PokeBattle.Effects;
using PokemonEssentials.Interface.EventArg;
using PokemonEssentials.Interface.Screen;
using UnityEngine;
using UnityEngine.UI;

namespace PokemonUnity.Interface.UnityEngine
{
        [RequireComponent(typeof(IntroSceneToolkitAdapter))]
        public class IntroScene : EventScene, IIntroEventScene
        {
		public override int Id { get { return (int)Scenes.TextEntry; } }
		/// <summary>
		/// Array of images to display before Title/Splash Card
		/// </summary>
		public global::UnityEngine.Sprite[] pics;
		/// <summary>
		/// Active/Current Image being displayed in Scene
		/// </summary>
		public global::UnityEngine.UI.Image pic;
		/// <summary>
		/// flashing "Press Start" picture
		/// </summary>
		public global::UnityEngine.UI.Image pic2;
		/// <summary>
		/// Background Image used to compliment "Press Start" display. (Title Card)
		/// </summary>
		public global::UnityEngine.Sprite splash;
		/// <summary>
		/// Image used for "Press Start" display.
		/// </summary>
		public global::UnityEngine.Sprite start;
		/// <summary>
		/// Id used for Pics array
		/// </summary>
		public int index;
		/// <summary>
		/// Timer for Picture display
		/// </summary>
		public int Timer;
                private IAudioObject title_bgm;
                private IntroSceneToolkitAdapter toolkitAdapter;
                private CanvasGroup picCanvasGroup;
                private CanvasGroup pic2CanvasGroup;

                private const float FramesPerSecond = 60f;

                // Start is called before the first frame update
                void Start()
                {
                        toolkitAdapter = GetComponent<IntroSceneToolkitAdapter>();
                        toolkitAdapter?.EnsureInitialized();
                        initialize();
                        //StartCoroutine(main()); //While Loop to run indefinitely...
                }

		// Update is called once per frame
		void Update()
		{
			base.update();
			//Every action will be executed once, and the script ends with `update()` which will pause and wait for brief moment.
			splashUpdate();
		}

		//public void initialize(pics, splash, viewport= null)
		public IIntroEventScene initialize()
		{
			//super(null);
			base.initialize(null);
			//@pics = pics;
			//@splash = splash;
                        //@pic = addImage(0, 0, "");
                        //@pic.moveOpacity(0, 0, 0); // fade to opacity 0 in 0 frames after waiting 0 frames
                        SetImageAlphaImmediate(pic, 0f);
                        //@pic2 = addImage(0, 322, ""); // flashing "Press Enter" picture
                        SetImageAlphaImmediate(pic2, 0f);
                        //@pic2.moveOpacity(0, 0, 0);
                        toolkitAdapter?.SetSplashOpacity(0f);
                        toolkitAdapter?.SetPromptOpacity(0f);
			@index = 0;
			//data_system = LoadRxData("Data/System");
			if (Game.GameData is IGameAudioPlay gap)
			{
				//gap.BGMPlay(data_system.title_bgm);
				gap.BGMPlay(Game.GameData.DataSystem.title_bgm);
				gap.BGMPlay(title_bgm);
			}
			openPic();
			return this;
		}

		public void openPic()
		{
			//onCTrigger.clear();
			ClearOnTriggerA();
			//onUpdate.clear();
			ClearOnUpdate();
                        //@pic.name = "Graphics/Titles/" + @pics[@index];
                        @pic.sprite = @pics[@index];
                        toolkitAdapter?.SetSplashSprite(@pics[@index]);
                        //@pic.moveOpacity(15, 0, 255); // fade to opacity 255 in 15 frames after waiting 0 frames
                        FadeImage(pic, 255f, 15f);
                        toolkitAdapter?.FadeSplash(1f, FramesToSeconds(15f));
			pictureWait();
			Timer = 0; // reset the timer
			//onUpdate.set(method(:timer)); // call timer every frame
			//if (OnUpdate) timer();
			OnUpdateEvent += IntroScene_onUpdate_Timer;
			//onCTrigger.set(method(:closePic)); // call closePic when C key is pressed
			//if (OnATrigger) closePic();
			OnATriggerEvent += IntroScene_onATrigger_Pic;
		}

		public void timer()
		{
			@Timer += 1;
			if (@Timer > 80)
			{
				@Timer = 0;
				closePic(); // Close the picture
			}
		}

		public void closePic()
		{
			//onCTrigger.clear();
			ClearOnTriggerA();
			//onUpdate.clear();
			ClearOnUpdate();
                        //@pic.moveOpacity(15, 0, 0);
                        FadeImage(pic, 0f, 15f);
                        toolkitAdapter?.FadeSplash(0f, FramesToSeconds(15f));
			//set image alpha to 0
			pictureWait();
			@index += 1; // Move to the next picture
			if (@index >= @pics.Length)
			{
				//if index is at end, then change scene to splash scene
				openSplash();
			}
			else
			{
				//else next image
				openPic();
			}
		}

		public void openSplash()
		{
			//onCTrigger.clear();
			ClearOnTriggerA();
			//onUpdate.clear();
			ClearOnUpdate();
                        //@pic.name = "Graphics/Titles/" + @splash;
                        @pic.sprite = @splash;
                        toolkitAdapter?.SetSplashSprite(@splash);
                        //@pic.moveOpacity(15, 0, 255); // fade to opacity 255 in 15 frames after waiting 0 frames
                        FadeImage(pic, 255f, 15f);
                        toolkitAdapter?.FadeSplash(1f, FramesToSeconds(15f));
                        //@pic2.name = "Graphics/Titles/start";
                        @pic2.sprite = @start;
                        toolkitAdapter?.SetPromptSprite(@start);
                        //@pic2.moveOpacity(15, 0, 255); // fade to opacity 255 in 15 frames after waiting 0 frames
                        FadeImage(pic2, 255f, 15f);
                        toolkitAdapter?.FadePrompt(1f, FramesToSeconds(15f));
			pictureWait();
			//onUpdate.set(method(:splashUpdate));  // call splashUpdate every frame
			//if (onUpdate) splashUpdate();
			OnUpdateEvent += IntroScene_onUpdate_Splash;
			//onCTrigger.set(method(:closeSplash)); // call closeSplash when C key is pressed
			//if (onATrigger) closeSplash();
			OnUpdateEvent += IntroScene_onATrigger_Splash;
		}

		public void splashUpdate()
		{
			#region Coroutine Tween Looping Animation
			@Timer += 1;
			if (@Timer >= 80) @Timer = 0;
			float cycle = Mathf.PingPong(@Timer, 80f) / 80f;
			float promptAlpha = 1f - Mathf.Abs(0.5f - cycle) * 2f;
			SetImageAlphaImmediate(@pic2, promptAlpha);
			toolkitAdapter?.SetPromptOpacity(promptAlpha);
			#endregion
			// Can be whatever combination of buttons you design in your game
			if (PokemonUnity.Input.press(PokemonUnity.Input.DOWN) &&
				PokemonUnity.Input.press(PokemonUnity.Input.A) &&
				PokemonUnity.Input.press(PokemonUnity.Input.CTRL))
			{
				closeSplashDelete();
			}
		}

		public void closeSplash()
		{
			//onCTrigger.clear();
			ClearOnTriggerA();
			//onUpdate.clear();
			ClearOnUpdate();
			//  Play random cry
			IAudioObject cry = null;
			//IAudioObject cry = CryFile(1 + Core.Rand.Next(Species.maxValue));
			if (Game.GameData is IGameUtility gu) gu.CryFile((Pokemons)(1 + Core.Rand.Next(Core.PokemonIndexLimit)));
			if (cry != null && Game.GameData is IGameAudioPlay gap) gap.SEPlay(cry, 80, 100);
			//  Fade out
                        //@pic.moveOpacity(15, 0, 0);
                        FadeImage(pic2, 0f, 15f);
                        toolkitAdapter?.FadePrompt(0f, FramesToSeconds(15f));
                        //@pic2.moveOpacity(15, 0, 0);
                        FadeImage(pic, 0f, 15f);
                        toolkitAdapter?.FadeSplash(0f, FramesToSeconds(15f));
			if (Game.GameData is IGameAudioPlay gap1) gap1.BGMStop(1.0f);
			pictureWait();
			//scene.dispose(); // Close the scene
			(this as IDisposable).Dispose(); // Close the scene
			//Destroy scene and load next scene
			//IPokemonLoadScene sscene = new PokemonLoadScene();
			//IPokemonLoad sscreen = new PokemonLoad(sscene);
			ILoadScene sscene = GameManager.current.game.Scenes.Load;
			ILoadScreen sscreen = GameManager.current.game.Screens.Load.initialize(sscene);
			sscreen.StartLoadScreen();
		}

		/// <summary>
		/// Load the scene that gives player the option to delete their Saved game state
		/// </summary>
		public void closeSplashDelete()
		{
			//onCTrigger.clear();
			ClearOnTriggerA();
			//onUpdate.clear();
			ClearOnUpdate();
			IAudioObject cry = null;
			//  Play random cry
			//IAudioObject cry = CryFile(1 + Core.Rand.Next(Species.maxValue));
			if (Game.GameData is IGameUtility gu) gu.CryFile((Pokemons)(1 + Core.Rand.Next(Core.PokemonIndexLimit)));
			if (cry != null && Game.GameData is IGameAudioPlay gap) gap.SEPlay(cry, 80, 100);
			//  Fade out
                        //@pic.moveOpacity(15, 0, 0);
                        FadeImage(pic, 0f, 15f);
                        toolkitAdapter?.FadeSplash(0f, FramesToSeconds(15f));
                        //@pic2.moveOpacity(15, 0, 0);
                        FadeImage(pic2, 0f, 15f);
                        toolkitAdapter?.FadePrompt(0f, FramesToSeconds(15f));
			if (Game.GameData is IGameAudioPlay gap1) gap1.BGMStop(1.0f);
			pictureWait();
			//scene.dispose(); // Close the scene
			(this as IDisposable).Dispose(); // Close the scene
			//Destroy scene and load next scene
			//IPokemonLoadScene sscene = new PokemonLoadScene();
			//IPokemonLoad sscreen = new PokemonLoad(sscene);
			ILoadScene sscene = GameManager.current.game.Scenes.Load;
			ILoadScreen sscreen = GameManager.current.game.Screens.Load.initialize(sscene);
			sscreen.StartDeleteScreen();
		}

		/// <summary>
		/// call timer every frame
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void IntroScene_onUpdate_Timer(object sender, System.EventArgs e)
		{
			timer();
		}

		/// <summary>
		/// call splashUpdate every frame
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void IntroScene_onUpdate_Splash(object sender, System.EventArgs e)
		{
			splashUpdate();
		}

		/// <summary>
		/// call closePic when Action key is pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void IntroScene_onATrigger_Pic(object sender, System.EventArgs e)
		{
			closePic();
		}

		/// <summary>
		/// call closeSplash when Action key is pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
                protected virtual void IntroScene_onATrigger_Splash(object sender, System.EventArgs e)
                {
                        closePic();
                }

                private void FadeImage(global::UnityEngine.UI.Image target, float targetAlpha, float durationFrames)
                {
                        CanvasGroup canvasGroup = EnsureCanvasGroup(target);
                        if (canvasGroup == null)
                        {
                                return;
                        }

                        float normalizedTarget = targetAlpha > 1f ? targetAlpha / 255f : targetAlpha;
                        float durationSeconds = FramesToSeconds(durationFrames);
                        if (durationSeconds <= 0f)
                        {
                                canvasGroup.alpha = Mathf.Clamp01(normalizedTarget);
                                return;
                        }

                        LeanTween.alphaCanvas(canvasGroup, Mathf.Clamp01(normalizedTarget), durationSeconds);
                }

                private void SetImageAlphaImmediate(global::UnityEngine.UI.Image target, float normalizedAlpha)
                {
                        CanvasGroup canvasGroup = EnsureCanvasGroup(target);
                        if (canvasGroup != null)
                        {
                                canvasGroup.alpha = Mathf.Clamp01(normalizedAlpha);
                        }
                }

                private CanvasGroup EnsureCanvasGroup(global::UnityEngine.UI.Image target)
                {
                        if (target == null)
                        {
                                return null;
                        }

                        if (target == pic)
                        {
                                if (picCanvasGroup == null)
                                {
                                        picCanvasGroup = GetOrAddCanvasGroup(target);
                                }
                                return picCanvasGroup;
                        }

                        if (target == pic2)
                        {
                                if (pic2CanvasGroup == null)
                                {
                                        pic2CanvasGroup = GetOrAddCanvasGroup(target);
                                }
                                return pic2CanvasGroup;
                        }

                        return GetOrAddCanvasGroup(target);
                }

                private static CanvasGroup GetOrAddCanvasGroup(global::UnityEngine.UI.Image target)
                {
                        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
                        if (canvasGroup == null)
                        {
                                canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
                        }
                        return canvasGroup;
                }

                private static float FramesToSeconds(float frames)
                {
                        if (frames <= 0f)
                        {
                                return 0f;
                        }

                        return frames / FramesPerSecond;
                }

                public override void Refresh()
                {
                        //Not used in this scene...
                }

		public override void Display(string v)
		{
			//Not used in this scene...
		}

		public override bool DisplayConfirm(string v)
		{
			//Not used in this scene...
			return false;
		}
	}
}