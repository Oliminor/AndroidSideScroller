using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(EnemySpawner)), CanEditMultipleObjects]
public class EnemySpawnerEditor : Editor
{
    EnemySpawner enemySpawner;

    private void OnEnable()
    {
        enemySpawner = (EnemySpawner)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(15);

        GUILayout.BeginHorizontal("Creative name");

        GUILayout.Label("Time in Seconds (Enemy)");

        EditorGUILayout.FloatField(enemySpawner.CalculateTotalEnemySpawnTime(enemySpawner.GetEnemySpawnerObjects(), enemySpawner.GetEnemySpawnRate()));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Creative name");

        GUILayout.Label("Time in Seconds (Obstacles)");

        EditorGUILayout.FloatField(enemySpawner.CalculateTotalEnemySpawnTime(enemySpawner.GetObstaclepawnerObjects(), enemySpawner.GetObstacleSpawnRate()));

        GUILayout.EndHorizontal();
    }
}
#endif