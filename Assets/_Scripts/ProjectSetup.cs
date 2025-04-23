#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;

using static System.IO.Path;
using static UnityEditor.AssetDatabase;

public static class ProjectSetup {

    //Imports what store packages I will use the most
    [MenuItem("Tools/_Projekt/Setup/Import Essential Assets", priority=-1)]
    static void ImportEssentials() {
        //ex) Assets.ImportAsset("Package_Name.unitypackage", "Folder/SubFolder");
        Assets.ImportAsset("Better Hierarchy.unitypackage", "Toaster Head/Editor ExtensionsUtilities");
        Assets.ImportAsset("DOTween Pro.unitypackage", "Demigiant/Editor ExtensionsVisual Scripting");
        Assets.ImportAsset("Editor Auto Save.unitypackage", "IntenseNation/Editor ExtensionsUtilities");
        Assets.ImportAsset("Feel.unitypackage", "More Mountains/ScriptingEffects");
        Assets.ImportAsset("Hot Reload Edit Code Without Compiling.unitypackage", "The Naughty Cult/Editor ExtensionsUtilities");
        //Assets.ImportAsset("Modern UI Pack.unitypackage", "Michsky/ScriptingGUI");
        Assets.ImportAsset("Graphy - Ultimate FPS Counter - Stats Monitor Debugger.unitypackage", "Tayx/ScriptingGUI");
        Assets.ImportAsset("NaughtyAttributes.unitypackage", "Denis Rizov/Editor ExtensionsUtilities");
        Assets.ImportAsset("PlayerPrefs Editor.unitypackage", "BG Tools/Editor ExtensionsUtilities");
        Assets.ImportAsset("Mulligan Renamer.unitypackage", "Red Blue Games/Editor ExtensionsUtilities");
        MoveFolders();
    }

    //Import my most used unity packages
    [MenuItem("Tools/_Projekt/Setup/Import Essential Packages", priority = -1)]
    public static void ImportPackages() {
        string[] packages = {
            "com.unity.textmeshpro"
        };

        Packages.InstallPackages(packages);
    }

    //Create my ideal folder structure
    [MenuItem("Tools/_Projekt/Setup/Folders/Create Folders", priority = -1)]
    public static void CreateFolders() {
        Folders.Create("Animation", "Materials", "Models", "Models/Prefabs", "_Scripts", "Art", "Input", "SFX", "Plugins");
        Refresh();
        Folders.Delete("TutorialInfo");
        Refresh();

        const string PATH_TO_INPUT_ACTIONS = "Assets/InputSystem_Actions.inputactions";
        const string PATH_TO_URP_GLOBAL_SETTINGS = "Assets/UniversalRenderPipelineGlobalSettings.asset";
        const string PATH_TO_DEFAULT_VOLUME_PROFILE = "Assets/DefaultVolumeProfile.asset";
        
        string destination = "Assets/";
        MoveAsset(PATH_TO_INPUT_ACTIONS, destination + "Input/InputSystem_Actions.inputactions");
        MoveAsset(PATH_TO_URP_GLOBAL_SETTINGS, destination + "Settings/UniversalRenderPipelineGlobalSettings.asset");
        MoveAsset(PATH_TO_DEFAULT_VOLUME_PROFILE, destination + "Settings/DefaultVolumeProfile.asset");

        const string PATH_TO_README = "Assets/Readme.asset";
        DeleteAsset(PATH_TO_README);
        Refresh();
    }

    //Moves all package folders into Plugins folder
    [MenuItem("Tools/_Projekt/Setup/Folders/Move Folders", priority = -1)]
    public static void MoveFolders() {
        if (AssetDatabase.IsValidFolder("Assets/Plugins")) {
            Folders.Move("Plugins", "Feel");
            Folders.Move("Plugins", "IntenseNation");
           // Folders.Move("Plugins", "Modern UI Pack");
            Folders.Move("Plugins", "TextMesh Pro");
            Folders.Move("Plugins", "PlayerPrefsEditor");
            Folders.Move("Plugins", "RedBlueGames");
            Folders.Move("Plugins", "NaughtyAttributes");
            Folders.Move("Plugins", "Graphy - Ultimate Stats Monitor");
            Refresh();
        } else {
            Debug.LogError("Plugins folder does not exist.");
        }
    }

    static class Assets {
        public static void ImportAsset(string asset, string folder) {
            string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string assetsFolder = Combine(basePath, "Unity/Asset Store-5.x"); //C:Users/<Username>/AppData/Roaming/Unity/Asset Store-5.x <Complete basefolder path to find assets
            UnityEditor.AssetDatabase.ImportPackage(Combine(assetsFolder, folder, asset), false);
        }
    }

    static class Packages {
        static AddRequest request;
        private static Queue<string> packagesToInstal = new();

        static async void StartNextPackageInstallation() {
            request = Client.Add(packagesToInstal.Dequeue());

            while (!request.IsCompleted) await Task.Delay(10);

            if (request.Status == StatusCode.Success) {
                Debug.Log("Installed " + request.Result.packageId);
            } else if (request.Status >= StatusCode.Failure) Debug.LogError(request.Error.message);

            if(packagesToInstal.Count > 0) {
                await Task.Delay(1000);
                StartNextPackageInstallation();
            }
        }

        public static void InstallPackages(string[] packages) {
            foreach (string package in packages) { 
                packagesToInstal.Enqueue(package);
            }

            if(packagesToInstal.Count > 0) {
                StartNextPackageInstallation();
            }

        }

    }

    static class Folders {

        public static void Delete(string folder) {
            string pathToDelete = $"Assets/{folder}";

            if(AssetDatabase.IsValidFolder(pathToDelete)) AssetDatabase.DeleteAsset(pathToDelete);

        }

        public static void Move(string newParent, string folderName) {
            string sourcePath = $"Assets/{folderName}";
            if (AssetDatabase.IsValidFolder(sourcePath)) {
                string destinationPath = $"Assets/{newParent}/{folderName}";
                string error = AssetDatabase.MoveAsset(sourcePath, destinationPath);

                if (!string.IsNullOrEmpty(error)) Debug.LogError($"Failed to move {folderName}: {error}");
                Debug.Log($"Moved {folderName} to {newParent}");
            }
        }

        static void CreateSubFolders(string rootPath, string folderHierarchy) {
            var folders = folderHierarchy.Split('/');
            var currentPath = rootPath;

            foreach (var folder in folders) {
                currentPath = Path.Combine(currentPath, folder);

                if (!Directory.Exists(currentPath)) Directory.CreateDirectory(currentPath);
            }
        }

        public static void Create(params string[] folders) {
            var fullPath = Path.Combine(Application.dataPath);

            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

            foreach (var folder in folders) {
                CreateSubFolders(fullPath, folder);
            }

        }
    }

    [MenuItem("Tools/_Projekt/Save Scene and Project %#s")] // Ctrl/Cmd + Shift + S
    private static void SaveAll() {
        // Save the currently active scene
        if (EditorSceneManager.SaveOpenScenes()) {
            Debug.Log("Scene(s) saved successfully.");
        }
        else
        {
            Debug.LogWarning("Scene save was canceled or failed.");
        }

        // Save the whole project (assets, settings)
        AssetDatabase.SaveAssets();
        Debug.Log("Project assets saved.");
    }

}

#endif