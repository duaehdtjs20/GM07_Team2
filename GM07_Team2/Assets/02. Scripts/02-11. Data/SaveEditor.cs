using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveManager saveManager = (SaveManager)target;
        bool hasSaveData = saveManager.HasSaveData();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Save Data", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("상태", hasSaveData ? "저장 파일 있음" : "저장 파일 없음");
        EditorGUILayout.LabelField("경로");
        EditorGUILayout.SelectableLabel(saveManager.SavePath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 중 삭제하면 현재 데이터가 다시 저장될 수 있습니다. 플레이를 종료한 뒤 삭제하는 것을 권장합니다.", MessageType.Warning);
        }

        using (new EditorGUI.DisabledScope(!hasSaveData))
        {
            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.45f, 0.45f);

            if (GUILayout.Button("저장 데이터 삭제", GUILayout.Height(30)))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "저장 데이터 삭제",
                    "SaveData.json을 삭제하시겠습니까?\n삭제한 데이터는 복구할 수 없습니다.",
                    "삭제",
                    "취소");

                if (confirmed)
                {
                    saveManager.DeleteSaveData();
                    Repaint();
                }
            }

            GUI.backgroundColor = previousColor;
        }
    }
}
