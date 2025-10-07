using UnityEngine;
using UnityEngine.UIElements;

namespace PokemonUnity.Interface.UnityEngine
{
        /// <summary>
        /// Bridges the legacy intro scene implementation with Unity's UI Toolkit runtime.
        /// </summary>
        [DisallowMultipleComponent]
        [RequireComponent(typeof(UIDocument))]
        public class IntroSceneToolkitAdapter : MonoBehaviour
        {
                [SerializeField]
                [Tooltip("Resource path (under a Resources folder) to the UXML asset that defines the intro layout.")]
                private string visualTreeResourcePath = "UI/Intro/IntroSplash";

                [SerializeField]
                [Tooltip("Name of the VisualElement that displays the splash/background sprite.")]
                private string splashElementName = "intro-splash";

                [SerializeField]
                [Tooltip("Name of the VisualElement that shows the press start prompt sprite.")]
                private string promptElementName = "intro-start-prompt";

                [SerializeField]
                [Tooltip("Default scale mode when a PanelSettings asset is generated at runtime.")]
                private PanelScaleMode runtimePanelScaleMode = PanelScaleMode.ScaleWithScreenSize;

                [SerializeField]
                [Range(0f, 1f)]
                [Tooltip("Match value for the dynamically created PanelSettings asset.")]
                private float runtimePanelMatch = 0.5f;

                private UIDocument document;
                private VisualElement splashElement;
                private VisualElement promptElement;
                private bool initialized;
                private float currentSplashOpacity;
                private float currentPromptOpacity;
                private int splashTweenId = -1;
                private int promptTweenId = -1;

                /// <summary>
                /// Ensures the underlying UIDocument is configured and the expected elements are available.
                /// Safe to call multiple times.
                /// </summary>
                public void EnsureInitialized()
                {
                        if (initialized)
                        {
                                return;
                        }

                        document = GetComponent<UIDocument>();
                        if (document.panelSettings == null)
                        {
                                var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                                panelSettings.scaleMode = runtimePanelScaleMode;
                                panelSettings.match = runtimePanelMatch;
                                panelSettings.referenceDpi = 96f;
                                panelSettings.referenceResolution = new Vector2Int(1920, 1080);
                                document.panelSettings = panelSettings;
                        }

                        if (document.visualTreeAsset == null && !string.IsNullOrEmpty(visualTreeResourcePath))
                        {
                                var asset = Resources.Load<VisualTreeAsset>(visualTreeResourcePath);
                                if (asset != null)
                                {
                                        document.visualTreeAsset = asset;
                                }
                                else
                                {
                                        Debug.LogWarningFormat(this, "IntroSceneToolkitAdapter could not locate UXML asset at Resources path '{0}'.", visualTreeResourcePath);
                                }
                        }

                        var root = document.rootVisualElement;
                        if (root == null)
                        {
                                Debug.LogWarning("IntroSceneToolkitAdapter could not access the UIDocument root visual element. UI Toolkit intro will be skipped.");
                                return;
                        }

                        splashElement = root.Q<VisualElement>(splashElementName);
                        promptElement = root.Q<VisualElement>(promptElementName);

                        if (splashElement == null)
                        {
                                Debug.LogWarningFormat(this, "IntroSceneToolkitAdapter could not find a VisualElement named '{0}'.", splashElementName);
                        }

                        if (promptElement == null)
                        {
                                Debug.LogWarningFormat(this, "IntroSceneToolkitAdapter could not find a VisualElement named '{0}'.", promptElementName);
                        }

                        if (splashElement != null)
                        {
                                splashElement.style.opacity = 0f;
                        }

                        if (promptElement != null)
                        {
                                promptElement.style.opacity = 0f;
                        }

                        currentSplashOpacity = 0f;
                        currentPromptOpacity = 0f;
                        initialized = true;
                }

                public void SetSplashSprite(Sprite sprite)
                {
                        EnsureInitialized();
                        if (splashElement == null)
                        {
                                return;
                        }

                        splashElement.style.backgroundImage = sprite != null
                                ? new StyleBackground(sprite)
                                : new StyleBackground { keyword = StyleKeyword.Null };
                }

                public void SetPromptSprite(Sprite sprite)
                {
                        EnsureInitialized();
                        if (promptElement == null)
                        {
                                return;
                        }

                        promptElement.style.backgroundImage = sprite != null
                                ? new StyleBackground(sprite)
                                : new StyleBackground { keyword = StyleKeyword.Null };
                }

                public void SetSplashOpacity(float normalizedOpacity)
                {
                        EnsureInitialized();
                        CancelSplashTween();
                        currentSplashOpacity = Mathf.Clamp01(normalizedOpacity);
                        if (splashElement != null)
                        {
                                splashElement.style.opacity = currentSplashOpacity;
                        }
                }

                public void SetPromptOpacity(float normalizedOpacity)
                {
                        EnsureInitialized();
                        CancelPromptTween();
                        currentPromptOpacity = Mathf.Clamp01(normalizedOpacity);
                        if (promptElement != null)
                        {
                                promptElement.style.opacity = currentPromptOpacity;
                        }
                }

                public void FadeSplash(float targetOpacity, float durationSeconds)
                {
                        EnsureInitialized();
                        if (splashElement == null)
                        {
                                return;
                        }

                        if (durationSeconds <= 0f)
                        {
                                SetSplashOpacity(targetOpacity);
                                return;
                        }

                        CancelSplashTween();
                        var tween = LeanTween.value(gameObject, currentSplashOpacity, Mathf.Clamp01(targetOpacity), durationSeconds);
                        splashTweenId = tween.uniqueId;
                        tween.setOnUpdate((float value) =>
                        {
                                currentSplashOpacity = value;
                                splashElement.style.opacity = value;
                        });
                        tween.setOnComplete(() => splashTweenId = -1);
                }

                public void FadePrompt(float targetOpacity, float durationSeconds)
                {
                        EnsureInitialized();
                        if (promptElement == null)
                        {
                                return;
                        }

                        if (durationSeconds <= 0f)
                        {
                                SetPromptOpacity(targetOpacity);
                                return;
                        }

                        CancelPromptTween();
                        var tween = LeanTween.value(gameObject, currentPromptOpacity, Mathf.Clamp01(targetOpacity), durationSeconds);
                        promptTweenId = tween.uniqueId;
                        tween.setOnUpdate((float value) =>
                        {
                                currentPromptOpacity = value;
                                promptElement.style.opacity = value;
                        });
                        tween.setOnComplete(() => promptTweenId = -1);
                }

                private void CancelSplashTween()
                {
                        if (splashTweenId >= 0)
                        {
                                LeanTween.cancel(splashTweenId);
                                splashTweenId = -1;
                        }
                }

                private void CancelPromptTween()
                {
                        if (promptTweenId >= 0)
                        {
                                LeanTween.cancel(promptTweenId);
                                promptTweenId = -1;
                        }
                }
        }
}
