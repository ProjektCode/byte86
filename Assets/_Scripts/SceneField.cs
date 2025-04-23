using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneField
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif

    [SerializeField] private string sceneName = "";
    public string SceneName => sceneName;

    public static implicit operator string(SceneField sceneField) => sceneField.sceneName;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is empty. Assign a valid scene.");
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneNameProperty = property.FindPropertyRelative("sceneName");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (sceneAssetProperty != null)
        {
            sceneAssetProperty.objectReferenceValue = EditorGUI.ObjectField(position, sceneAssetProperty.objectReferenceValue, typeof(SceneAsset), false);

            if (sceneAssetProperty.objectReferenceValue != null)
            {
                sceneNameProperty.stringValue = (sceneAssetProperty.objectReferenceValue as SceneAsset)?.name ?? "";
            }
            else
            {
                sceneNameProperty.stringValue = ""; // Clear name if asset is removed
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
