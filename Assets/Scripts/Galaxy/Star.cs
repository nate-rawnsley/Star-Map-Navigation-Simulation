using System.Collections.Generic;
using UnityEngine;

//A class used for holding information on each star, required for DijkstraSimplified.
public class Star : MonoBehaviour {
    public Dictionary<Star, float> starRoutes = new Dictionary<Star, float>();
    public string starName;
    public float scale;
}
