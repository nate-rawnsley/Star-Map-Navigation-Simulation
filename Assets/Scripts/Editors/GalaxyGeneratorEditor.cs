//using System.Collections;
//using UnityEngine;
//using UnityEditor;

//Editor script used for manually spawning the galaxy before the UI was implemented.
//Deactivated because of not being useful in the build and causing build errors.

//[CustomEditor(typeof(GalaxyGenerator))]
//public class GalaxyGeneratorEditor : Editor
//{
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();

//        GalaxyGenerator myScript = (GalaxyGenerator)target;
//        if (GUILayout.Button("Generate Stars")) {
//            myScript.GenerateStars();
//            myScript.GenerateRoutes();
//        }
//    }
//}

