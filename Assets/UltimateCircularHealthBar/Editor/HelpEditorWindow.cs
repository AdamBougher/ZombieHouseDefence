using UnityEngine;
using UnityEditor;

namespace RengeGames.HealthBars.Editors {
    [InitializeOnLoad]
    public class HelpWindow : EditorWindow {
        private bool m_ShowHelpOnStartup = true;
        private static Texture2D s_CoverImage;
        private static string s_CoverImagePath = string.Empty;
        private static string s_ReadmePath = string.Empty;

        const string k_ShowHelpOnStartup = "cpb-showHelpOnStartup";
        const string k_HelpWindowShown = "cpb-helpWindowShown";

        static HelpWindow() {
            EditorApplication.update += OnStartup;
            EditorApplication.quitting += OnQuitting;
        }

        private static void OnStartup() {
            if (!Application.isPlaying) {
                bool shouldShow = EditorPrefs.GetInt(k_ShowHelpOnStartup, 1) > 0;
                bool helpWindowShown = EditorPrefs.GetInt(k_HelpWindowShown, 0) > 0;

                if (!shouldShow || helpWindowShown)
                    return;

                EditorPrefs.SetInt(k_HelpWindowShown, 1);
                Init();
            }
            EditorApplication.update -= OnStartup;
        }

        private static void OnQuitting() {
            EditorPrefs.SetInt(k_HelpWindowShown, 0);
            EditorApplication.quitting -= OnQuitting;
        }

        [MenuItem("Window/Circular Progress Bars/Help", false)]
        private static void Init() {
            HelpWindow window = (HelpWindow)GetWindow(typeof(HelpWindow), true, "Procedural Circular Progress Bars Pro");
            window.minSize = new Vector2(640, 420);
            window.maxSize = new Vector2(640, 420);
            window.Show();
        }

        private void OnGUI() {
            using (new GUILayout.HorizontalScope()) {
                if (!s_CoverImage) {
                    if (s_CoverImagePath == string.Empty) {
                        var paths = AssetDatabase.FindAssets("rg-coverImage t:texture2d");
                        if (paths.Length != 0)
                            s_CoverImagePath = AssetDatabase.GUIDToAssetPath(paths[0]);
                    }
                    s_CoverImage = AssetDatabase.LoadAssetAtPath<Texture2D>(s_CoverImagePath);
                }
                if (s_CoverImage) {
                    GUI.DrawTexture(new Rect(0, 0, 205, 420), s_CoverImage, ScaleMode.StretchToFill, true, 0);
                    GUILayout.Space(205);
                }

                using (new GUILayout.VerticalScope()) {
                    var headerStyle = new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 16 };
                    var secondHeaderStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, wordWrap = true };
                    var textStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, margin = new RectOffset(25, 25, 0, 0), wordWrap = true };
                    
                    GUILayout.Space(32);
                    GUILayout.Label("Procedural Circular Progress Bars Pro", headerStyle);

                    GUILayout.Space(32);

                    GUILayout.Label("Thank you for purchasing Procedural Circular Progress Bars Pro! ", secondHeaderStyle);
                    GUILayout.Space(10);
                    GUILayout.Label("For instructions on how to use this asset, take a look at the included Readme. Feel free to ask for help/support over on the Discord or shoot an email to support@rengegames.com as usual.", textStyle);

                    GUILayout.FlexibleSpace();

                    int buttonWidth = 200;
                    int buttonHeight = 30;
                    using (new GUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 14, richText = true };
                        if (GUILayout.Button(new GUIContent(){text = "<i>YOU</i> Get <b>Procedural Progress Bars</b> for <b>50% off!</b>", tooltip = "https://u3d.as/2XdK"}, buttonStyle, GUILayout.Width(buttonWidth * 1.7f), GUILayout.Height(buttonHeight * 1.2f))) {
                            Application.OpenURL("https://u3d.as/2XdK");
                        }
                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.FlexibleSpace();

                    using (new GUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Open Readme", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
                            if (s_ReadmePath == string.Empty) {
                                var paths = AssetDatabase.FindAssets("glob:\"**/UltimateCircularHealthBar/README.pdf\"");
                                if (paths.Length != 0)
                                    s_ReadmePath = AssetDatabase.GUIDToAssetPath(paths[0]);
                                //Remove "Assets" from the path, since it is already included in datapath
                                s_ReadmePath = s_ReadmePath.Remove(0, 6);
                            }
                            Application.OpenURL("file:///" + Application.dataPath + s_ReadmePath);
                        }
                        GUILayout.FlexibleSpace();
                    }

                    using (new GUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(){text = "Support Discord", tooltip = "https://discord.gg/ewAueXSZ3V"}, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
                            Application.OpenURL("https://discord.gg/ewAueXSZ3V");
                        }
                        GUILayout.FlexibleSpace();
                    }

                    using (new GUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(){text = "Support E-mail", tooltip = "mailto:support@rengegames.com"}, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
                            Application.OpenURL("mailto:support@rengegames.com");
                        }
                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.HelpBox("You can reopen this window any time in 'Window > Procedural Circular Progress Bars > Help'", MessageType.Info);
                    GUILayout.Space(10);
                    using (new GUILayout.HorizontalScope()) {
                        EditorGUILayout.LabelField("PCPB 8.9.7", EditorStyles.boldLabel);

                        m_ShowHelpOnStartup = EditorPrefs.GetInt(k_ShowHelpOnStartup, 1) > 0;
                        m_ShowHelpOnStartup = EditorGUILayout.ToggleLeft("Show this window at startup", m_ShowHelpOnStartup);
                        EditorPrefs.SetInt(k_ShowHelpOnStartup, m_ShowHelpOnStartup ? 1 : 0);
                    }

                }
            }
        }
    }
}
