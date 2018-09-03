using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AP_Manager))]
public class AP_Manager_Editor : Editor {

	SerializedObject obj;
	AP_Manager script;

	SerializedProperty allowCreate;
	SerializedProperty allowModify;
	SerializedProperty printAllLogsOnQuit;

//	void OnEnable () {
		//		obj = new SerializedObject( target );
//	}

	public override void OnInspectorGUI ( ) {
		obj = new SerializedObject( target );
		obj.Update();
		script = (AP_Manager) target;
		allowCreate = obj.FindProperty("allowCreate");
		allowModify = obj.FindProperty("allowModify");
		printAllLogsOnQuit = obj.FindProperty("printAllLogsOnQuit");

		GUILayout.Space(4f);
		GUILayout.BeginHorizontal();
		GUILayout.Label( "", GUILayout.Width(30f) );
		if ( EditorApplication.isPlaying == true ) { GUI.enabled = false; }
		if ( GUILayout.Button( new GUIContent("Create Pool", "Create a new object pool. (Only in edit mode.)\n\n" +
			"This is merely a shortcut. A new pool can also be created from scratch, or by duplicating exsisting pools. " +
			"It's simply a child object with an MF_B_ObjectPool script."), EditorStyles.miniButton, GUILayout.MaxWidth(80f) ) ) {
			if ( EditorApplication.isPlaying == false ) { script.CreatePool(); }
		}
		if ( EditorApplication.isPlaying == true ) { GUI.enabled = true; }
		GUILayout.EndHorizontal();

		GUILayout.Label( "Runtime Options:", EditorStyles.boldLabel );
		GUILayout.Space(-4f);
		GUILayout.BeginHorizontal();
		GUILayout.Label( new GUIContent("Allow Create", "Allows the creation of object pools during runtime using MF_ObjectPool.Initialize()" ), GUILayout.Width(80f) );
		EditorGUILayout.PropertyField( allowCreate, GUIContent.none, GUILayout.Width(50f) );
		GUILayout.Label( new GUIContent("Allow Modify", "Allows the modification of exsisting object pools during runtime using MF_ObjectPool.Initialize()" ), GUILayout.Width(80f) );
		EditorGUILayout.PropertyField( allowModify, GUIContent.none, GUILayout.Width(50f) );
		GUILayout.EndHorizontal();

		GUILayout.Label( "Debug Options:", EditorStyles.boldLabel );
		GUILayout.Space(-4f);
		GUILayout.BeginHorizontal();
		GUILayout.Label( new GUIContent("Print All Logs On Quit", "When the scene is stopped, creates a report showing pool usage for all pools:\n\n" +
			"Start Size - Size of pool when beginning the scene.\n\n" +
			"Init Added - Number of objects added by InitializeSpawn() at runtime.\n\n" +
			"Grow Objects - Number of objects added with EMptyBehavior.Grow.\n\n" +
			"End Size - Total objects of this pool, active and inactive, at the time of the log report.\n\n" +
			"Failed Spawns - Number of Spawn() requests that didn't return a spawn.\n\n" +
			"Reused Objects - Number of times an object was reused before despawning normally.\n\n" +
			"Most Objects Active - The most items for this pool active at once." ), GUILayout.Width(120f) );
		EditorGUILayout.PropertyField( printAllLogsOnQuit, GUIContent.none, GUILayout.Width(30f) );
		if ( EditorApplication.isPlaying == false ) { GUI.enabled = false; }
		if ( GUILayout.Button( new GUIContent("Print All Logs", "Creates a report showing pool usage for all pools: (Only during play mode)\n\n" +
			"Start Size - Size of pool when beginning the scene.\n\n" +
			"Init Added - Number of objects added by InitializeSpawn() at runtime.\n\n" +
			"Grow Objects - Number of objects added with EMptyBehavior.Grow.\n\n" +
			"End Size - Total objects of this pool, active and inactive, at the time of the log report.\n\n" +
			"Failed Spawns - Number of Spawn() requests that didn't return a spawn.\n\n" +
			"Reused Objects - Number of times an object was reused before despawning normally.\n\n" +
			"Most Objects Active - The most items for this pool active at once."), EditorStyles.miniButton, GUILayout.MaxWidth(80f) ) ) {
			script.PrintAllLogs();
		}
		if ( EditorApplication.isPlaying == false ) { GUI.enabled = true; }
		GUILayout.EndHorizontal();

		obj.ApplyModifiedProperties();
	}
}


