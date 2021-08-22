using UnityEditor;
using UnityEngine;

public class UnityAnswers_CustomUnityEditorDuplicator : MonoBehaviour
{

    [MenuItem("Editor Extensions/Duplicate Selected and make it as a sibling #%d")]
    static void DoSomethingWithAShortcutkey()
    {
        GameObject duped = Instantiate(Selection.activeGameObject, Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation, Selection.activeGameObject.transform.parent);
        duped.transform.SetSiblingIndex(Selection.activeGameObject.transform.GetSiblingIndex() + 1);
    }
}


