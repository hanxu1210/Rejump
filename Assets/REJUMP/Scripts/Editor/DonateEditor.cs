using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Donate))]
public class DonateEditor : Editor {

    private const string rate_url = "http://u3d.as/pRi";

    Donate donate
    {
        get
        {
            return (Donate)target;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical("Box");
        GUILayout.Label("Current Coins Count: " + PlayerPrefs.GetInt("Coins"));
        EditorGUILayout.BeginHorizontal();
        donate.Coins = EditorGUILayout.IntField("Coins To Add", donate.Coins);
        if (GUILayout.Button("DONATE", EditorStyles.toolbarButton))
            donate.DonateCoins();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("CLEAN PLAYER PREFS", EditorStyles.toolbarButton))
            PlayerPrefs.DeleteAll();
        GUILayout.Space(5);
        if (GUILayout.Button("RATE ASSET", EditorStyles.toolbarButton))
            Application.OpenURL(rate_url);
        EditorGUILayout.EndVertical();
    }
}
