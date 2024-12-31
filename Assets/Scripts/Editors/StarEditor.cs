//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.Linq;

//Editor script used for checking each star's routes, for debugging.
//Deactivated because of not being useful in the build and causing build errors.

//[CustomEditor(typeof(Star))]
//public class StarEditor : Editor
//{
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();

//        Star myScript = (Star)target;
//        foreach (var item in myScript.starRoutes) {
//            EditorGUILayout.LabelField(item.Key.starName, item.Value.ToString());
//        }
//    }
//}
