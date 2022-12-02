namespace Dreamteck.Splines.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using Dreamteck.Editor;
    using System.Reflection;

    [InitializeOnLoad]
    public static class PluginInfo
    {
        public static string version = "3.0.4";
        private static bool open = false;

        static PluginInfo()
        {
            if (open) return;
            bool showInfo = EditorPrefs.GetString("Dreamteck.Splines.Info.version", "") != version;
            if (!showInfo) return;
            EditorPrefs.SetString("Dreamteck.Splines.Info.version", version);
            EditorApplication.update += OpenWindowOnUpdate;
        }

        private static void OpenWindowOnUpdate()
        {
            EditorApplication.update -= OpenWindowOnUpdate;
            EditorWindow.GetWindow<WelcomeScreen>(true);      
            open = true;
        }
    }

    [InitializeOnLoad]
    public static class AddScriptingDefines
    {
        static AddScriptingDefines(){
            ScriptingDefineUtility.Add("DREAMTECK_SPLINES", EditorUserBuildSettings.selectedBuildTargetGroup, true);
        }
    }

    public class WelcomeScreen : WelcomeWindow
    {
        protected override Vector2 _windowSize => new Vector2(450, 650);
        private ModuleInstaller _tmproInstaller;
        private ModuleInstaller _playmakerInstaller;
        private ModuleInstaller _examplesInstaller;

        [MenuItem("Window/Dreamteck/Splines/Start Screen")]
        public static void OpenWindow()
        {
            GetWindow<WelcomeScreen>(true);
        }

        protected override void GetHeader()
        {
            header = ResourceUtility.EditorLoadTexture("Splines/Editor/Icons", "plugin_header");
        }

        public override void Load()
        {
            base.Load();
            SetTitle("Dreamteck Splines " + PluginInfo.version, "");
            panels = new WindowPanel[7];
            panels[0] = new WindowPanel("Home", true, 0.25f); 
            panels[1] = new WindowPanel("Changelog", false, panels[0], 0.25f);
            panels[2] = new WindowPanel("Learn", false, panels[0], 0.25f);
            panels[3] = new WindowPanel("Support", false, panels[0], 0.25f);
            panels[4] = new WindowPanel("Examples", false, panels[2], 0.25f);
            panels[5] = new WindowPanel("Playmaker", false, panels[0], 0.25f);
            panels[6] = new WindowPanel("Text Mesh Pro", false, panels[0], 0.25f);



            panels[0].elements.Add(new WindowPanel.Space(400, 10));
            panels[0].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "changelog", "What's new?", "See all new features, important changes and bugfixes in " + PluginInfo.version, new ActionLink(panels[1], panels[0])));
            panels[0].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "get_started", "Get Started", "Learn how to use Dreamteck Splines in a matter of minutes", new ActionLink(panels[2], panels[0])));
            panels[0].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "support", "Community & Support", "Got a problem or a feature request? Our support is here to help!", new ActionLink(panels[3], panels[0])));
            panels[0].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "rate", "Rate", "If you like Dreamteck Splines, please consider rating it on the Asset Store", new ActionLink("https://u3d.as/sLk")));
            panels[0].elements.Add(new WindowPanel.Space(400, 20));
            panels[0].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "playmaker", "Playmaker Actions", "Install Playmaker actions for Dreamteck Splines", new ActionLink(panels[5], panels[0])));
            panels[0].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "tmpro", "Text Mesh Pro Support", "Manage components for working with Text Mesh Pro", new ActionLink(panels[6], panels[0])));
            panels[0].elements.Add(new WindowPanel.Thumbnail("Splines/Editor/Icons", "forever", "Forever", "Creating endless runners has never been easier. Forever is here to change the game!", new ActionLink("https://u3d.as/1t9T")));
            panels[0].elements.Add(new WindowPanel.Space(400, 10));
            panels[0].elements.Add(new WindowPanel.Label("This window will not appear again automatically. To open it manually go to Window/Dreamteck/Splines/Start Screen", wrapText, new Color(1f, 1f, 1f, 0.5f), 400, 50));



            string path = ResourceUtility.FindFolder(Application.dataPath, "Dreamteck/Splines/Editor");
            string changelogText = "Changelog file not found.";
            if (Directory.Exists(path))
            {
                if (File.Exists(path + "/changelog.txt"))
                {
                    string[] lines = File.ReadAllLines(path + "/changelog.txt");
                    changelogText = "";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        changelogText += lines[i] + "\r\n";
                    }
                }
            }
            panels[1].elements.Add(new WindowPanel.Space(400, 20));
            panels[1].elements.Add(new WindowPanel.ScrollText(400, 500, changelogText));

            panels[2].elements.Add(new WindowPanel.Space(400, 10));
            panels[2].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "manual", "User Manual", "Read a thorough documentation of the whole package along with a list of API methods.", new ActionLink("https://dreamteck.io/plugins/splines/user_manual.pdf")));
            panels[2].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "tutorials", "Video Tutorials", "Watch a series of Youtube videos to get started.", new ActionLink("https://www.youtube.com/playlist?list=PLkZqalQdFIQ6zym8RwSWWl3PZJuUdvNK6")));
            panels[2].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "examples", "Examples", "Install example scenes", new ActionLink(panels[4], panels[2])));

            panels[3].elements.Add(new WindowPanel.Space(400, 10));
            panels[3].elements.Add(new WindowPanel.Thumbnail("Utilities/Editor/Images", "discord", "Discord Server", "Join our Discord community and chat with other developers who use Splines.", new ActionLink("https://discord.gg/bkYDq8v")));
            panels[3].elements.Add(new WindowPanel.Button(400, 30, "Contact Support", new ActionLink("https://dreamteck.io/dreamteck-splines-faq/"))); 

            panels[4].elements.Add(new WindowPanel.Space(400, 10));
            panels[4].elements.Add(new WindowPanel.Button(400, 30, "Install Examples", new ActionLink(InstallExamples)));
            panels[4].elements.Add(new WindowPanel.Button(400, 30, "Uninstall Examples", new ActionLink(UnInstallExamples)));

            panels[5].elements.Add(new WindowPanel.Space(400, 10));

            panels[6].elements.Add(new WindowPanel.Button(400, 30, "Install TMPro Support", new ActionLink(InstallTMPro)));
            panels[6].elements.Add(new WindowPanel.Button(400, 30, "Uninstall TMPro Support", new ActionLink(UninstallTMPro)));

            panels[5].elements.Add(new WindowPanel.Button(400, 30, "Install Actions", new ActionLink(InstallPlaymaker)));
            panels[5].elements.Add(new WindowPanel.Button(400, 30, "Uninstall Actions", new ActionLink(UninstallPlaymaker)));
            

            _playmakerInstaller = new ModuleInstaller("Splines", "PlaymakerActions");
            _playmakerInstaller.AddUninstallDirectory("Splines/PlaymakerActions");

            _examplesInstaller = new ModuleInstaller("Splines", "Examples");
            _examplesInstaller.AddUninstallDirectory("Splines/Examples");

            _tmproInstaller = new ModuleInstaller("Splines", "TMPro");
            _tmproInstaller.AddAssemblyLink("Splines", "Dreamteck.Splines", "Unity.TextMeshPro");
            _tmproInstaller.AddScriptingDefine("DREAMTECK_SPLINES_TMPRO");
            _tmproInstaller.AddUninstallDirectory("Splines/Components/TMPro");
            _tmproInstaller.AddUninstallDirectory("Splines/Editor/Components/TMPro");
        }

        private void InstallExamples()
        {
            _examplesInstaller.Install();
            panels[5].Back();
        }

        private void UnInstallExamples()
        {
            _examplesInstaller.Uninstall();
            panels[5].Back();
        }

        private void InstallTMPro()
        {
            _tmproInstaller.Install();
            panels[6].Back();
        }

        private void UninstallTMPro()
        {
            _tmproInstaller.Uninstall();
            panels[6].Back();
        }

        private void InstallPlaymaker()
        {
            _playmakerInstaller.Install();
            panels[4].Back();
        }        
        
        private void UninstallPlaymaker()
        {
            _playmakerInstaller.Uninstall();
            panels[4].Back();
        }

        private static void AddAssemblyReference(string dreamteckAssemblyName, string addedAssemblyName)
        {
            string localDir = ResourceUtility.FindFolder(Application.dataPath, "Dreamteck/Splines");
            var path = Path.Combine(Application.dataPath, localDir, dreamteckAssemblyName + ".asmdef");
            var data = "";
            using (var reader = new StreamReader(path))
            {
                data = reader.ReadToEnd();
            }

            var asmDef = AssemblyDefinition.CreateFromJSON(data);
            foreach (var reference in asmDef.references)
            {
                if (reference == addedAssemblyName) return;
            }

            ArrayUtility.Add(ref asmDef.references, addedAssemblyName);
            using (var writer = new StreamWriter(path, false))
            {
                writer.Write(asmDef.ToString());
            }
        }
    }

    [System.Serializable]
    public struct AssemblyDefinition
    {
        public string name;
        public string rootNamespace;
        public string[] references;
        public string[] includePlatforms;
        public string[] exludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public string[] versionDefines;
        public bool noEngineReferences;

        public static AssemblyDefinition CreateFromJSON(string json)
        {
            return JsonUtility.FromJson<AssemblyDefinition>(json);
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
