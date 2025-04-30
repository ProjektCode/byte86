using UnityEngine;

public class SceneLoader : MonoBehaviour {
    [SerializeField] private SceneField sceneToLoad;

    public void LoadScene() {
        sceneToLoad.LoadScene();
    }

    public void QuitGame(){
        Application.Quit();
    }
}
