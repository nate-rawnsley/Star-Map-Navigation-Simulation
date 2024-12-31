using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

//Script that spawns the stars and routes in the galaxy.
//Attributes are assigned from sliders, through CanvasScript.
public class GalaxyGenerator : MonoBehaviour {
    [SerializeField] private GameObject starModel;
    [SerializeField] private GameObject line;

    public int starCount = 25;
    public float starRadius = 2;
    //The stars are generated in a cuboid shape, within these specified values.
    public Vector2 xRange = new Vector2(0, 40);
    public Vector2 yRange = new Vector2(0, 40);
    public Vector2 zRange = new Vector2(0, 40);

    public string seed;

    public List<GameObject> starList = new List<GameObject>();
    public List<GameObject> lineList = new List<GameObject>();

    public Vector2 scaleRange = new Vector2(1, 1);

    private GameObject galaxy;
    private GameObject lines;

    public float starLinkRadius;
    public int starLinkMax;
    public int linkRandomRange;
    public bool useScale;

    public Vector2[] bounds = new Vector2[3];
    public Vector3 centre;

    private CanvasScript canvasScript;

    [SerializeField] private TextAsset namesList;
    private string[] names;
    public Vector2 nameChar = new Vector2(1, 10);
    public Vector2 nameCount = new Vector2(2, 3);

    public UnityEvent OnGenerate;

    private void Start() {
        galaxy = new GameObject("Galaxy");
        lines = new GameObject("Lines");
        lines.transform.SetParent(galaxy.transform, false);
        canvasScript = GameObject.Find("Canvas").GetComponent<CanvasScript>();

        //Star names are taken from the list "all_words.txt", located in Resources.
        //This file was supplied from CT4102 for an exercise, repurposed here.
        names = namesList.text.Split("\n");
    }
    
    private void SetSeed() {
        if (seed != "") {
            Random.InitState(Mathf.Abs(seed.GetHashCode()));
        } else {
            Random.InitState(Mathf.Abs((int)System.DateTime.Now.Ticks)); //https://discussions.unity.com/t/generating-a-good-random-seed/90719
        }
    }

    public void GenerateStars() {
        List<Star> starScriptList = new List<Star>();
        //Narrows down the list of names to those within the specified character limits.
        List<string> useNames = new List<string>();
        foreach (string name in names) {
            if (name.Length >= nameChar.x && name.Length <= nameChar.y + 1) {
                useNames.Add(name);
            }
        }
        //Removes any existing stars or lines.
        if (starList.Count > 0) {
            for (int j = 0; j < starList.Count; j++) {
                Destroy(starList[j]);
            }
            for (int k = 0; k < lineList.Count; k++) {
                Destroy(lineList[k]);
            }
            starList.Clear();
            lineList.Clear();
        }
        bounds[0] = new Vector3(xRange.y, xRange.x);
        bounds[1] = new Vector3(yRange.y, yRange.x);
        bounds[2] = new Vector3(zRange.y, zRange.x);
        SetSeed();
        for (int i = 1; i < starCount + 1; i++) {   
            Vector3 starPos = new Vector3();

            bool newPos = false;
            int checkCount = 0;
            //Randomly generates a position for each star, and checks it against the already-assigned positions.
            //If the position is closer than the specified radius between stars, it attempts to find a new position.
            //The process is run a maximum of 10 times, to prevent an infinite loop.
            while (!newPos && checkCount < 10) {
                newPos = true;
                checkCount++;
                starPos.x = Random.value * (xRange.y - xRange.x) + xRange.x;
                starPos.y = Random.value * (yRange.y - yRange.x) + yRange.x;
                starPos.z = Random.value * (zRange.y - zRange.x) + zRange.x;
                foreach (var star in starList) {
                    //SqrMagnitude is used instead of distance for performance - compares against the square of the radius.
                    if (Vector3.SqrMagnitude(starPos - star.transform.position) < starRadius * starRadius) {
                        newPos = false;
                        break;
                    }
                }
            }
            
            //The upper and lower bounds are kept track of for the camera script.
            OverwriteBounds(starPos.x, 0);
            OverwriteBounds(starPos.y, 1);
            OverwriteBounds(starPos.z, 2);

            GameObject currentStar = Instantiate(starModel, starPos, Quaternion.identity, galaxy.transform);
            string name = "";
            //Concatenates a random number of the specified words (in the specified range of numbers).
            bool newName = false;
            int nameLoop = 0;
            while (newName == false && nameLoop < 10) {
                newName = true;
                for (int j = 0; j < Random.Range(nameCount.x - 1, nameCount.y); j++) {
                    string namePart = useNames[Random.Range(0, useNames.Count)];
                    namePart = char.ToUpper(namePart[0]) + namePart.Substring(1);
                    name += namePart + " ";
                }
                foreach (var star in starList) {
                    if (name == star.name) {
                        newName = false;
                    }
                }
            }
            currentStar.name = name;
            Star starScript = currentStar.AddComponent<Star>();
            currentStar.GetComponent<Renderer>().material.color = Random.ColorHSV(0, 1, 0.4f, 1, 0.75f, 1);
            starScript.starName = currentStar.name;
            float starScale = Random.value * (scaleRange.y - scaleRange.x) + scaleRange.x;
            currentStar.transform.localScale = currentStar.transform.localScale * starScale;
            starScript.scale = starScale;

            starList.Add(currentStar);
            starScriptList.Add(starScript);
        }
        DijkstraSimplified.SetGalaxyStarList(starScriptList);
        centre = new Vector3((bounds[0].x + bounds[0].y) / 2,
            (bounds[1].x + bounds[1].y) / 2,
            (bounds[2].x + bounds[2].y) / 2);
        //Triggers the CameraScript to run NewGenerate.
        OnGenerate.Invoke();
        canvasScript.ClearPath();
    }

    private void OverwriteBounds(float value, int index) {
        if (value < bounds[index].x) {
            bounds[index].x = value;
        } else if (value > bounds[index].y) {
            bounds[index].y = value;
        }
    }

    public void GenerateRoutes() {
        if (lineList.Count > 0) {
            for (int i = 0; i < lineList.Count; i++) {
                Destroy(lineList[i]);
            }
            lineList.Clear();
        }
        foreach (GameObject star in starList) {
            star.GetComponent<Star>().starRoutes.Clear();
        }
        //Goes through every star in the list to add links.
        foreach (var star1 in starList) {
            //Checks each star against each other star to see if they are within range (or just adds them if no specified range).
            Dictionary<GameObject, float> starsInRange = new Dictionary<GameObject, float>();
            foreach (var star2 in starList) {
                if (star1 != star2) {
                    float distance = Vector3.SqrMagnitude(star2.transform.position - star1.transform.position);
                    // if radius is specified, get stars within that radius. else, get all stars
                    if (distance < starLinkRadius * starLinkRadius || starLinkRadius <= 0) {
                        starsInRange[star2] = distance;
                    }
                }
            }
            if (starLinkMax <= 0) { // if there is no max links, make links between all available stars
                foreach (var star2 in starsInRange.Keys) {
                    CreateLink(star1, star2, starsInRange[star2]);
                }
            } else {
                if (starsInRange.Any()) {
                    ClosestLinks(star1, starsInRange);
                }
            }
        }
    }

    //Makes specified number of links between a star and the stars within its range.
    private void ClosestLinks(GameObject star, Dictionary<GameObject, float> starsInRange) {
        //If the star already has links from other stars, reduces the number of new ones that can be made.
        int maxNewLinks = starLinkMax - star.GetComponent<Star>().starRoutes.Count;
        int loopNum = starsInRange.Count < maxNewLinks ? starsInRange.Count : maxNewLinks;
        int randomRange = linkRandomRange > starsInRange.Count - 1 ? starsInRange.Count - 1 : linkRandomRange;
        var sortedStars = starsInRange.OrderBy(x => x.Value);
        Star thisStar = star.GetComponent<Star>();
        SetSeed();
        for (int i = 0; i < loopNum; i++) {
            bool found = false;
            GameObject starToAdd = null;
            int index = 0;
            if (linkRandomRange > 0) { // if a random range is supplied, get a random start index within that range
                index = Random.Range(0, randomRange - 1);
            }
            int initialIndex = index;
            //Iterates through the list until it finds one that isn't maxxed out on routes or already connected to this star.
            while (!found) {
                GameObject starToCheck = sortedStars.ElementAt(index).Key;
                Star starScriptToCheck = starToCheck.GetComponent<Star>();
                if (starToCheck.GetComponent<Star>().starRoutes.Count >= starLinkMax) {
                    starsInRange[starToCheck] = -1;
                }
                if (starsInRange[starToCheck] == -1 || starScriptToCheck.starRoutes.ContainsKey(thisStar)) {
                    index++;
                    if (randomRange > 0) {
                        index %= randomRange;
                    }
                    if (index == initialIndex || index >= starsInRange.Count) {
                        found = true;
                    }
                } else {
                    starToAdd = starToCheck;
                    found = true;
                }
            }
            if (starToAdd != null) {
                CreateLink(star, starToAdd, Mathf.Sqrt(starsInRange[starToAdd]));
            }
        }
    }

    private void CreateLink(GameObject star1, GameObject star2, float distance) {
        Star star1Script = star1.GetComponent<Star>();
        Star star2Script = star2.GetComponent<Star>();
        if (star1Script.starRoutes.ContainsKey(star2Script)) {
            return;
        }
        //If set to true, it changes route cost based on the scale of each star.
        //Big -> Small is more costly, Small -> Big is cheaper (proportional to the difference).
        if (useScale) {
            star1Script.starRoutes[star2Script] = distance * (star1Script.scale / star2Script.scale)  ;
            star2Script.starRoutes[star1Script] = distance * (star2Script.scale / star1Script.scale);
        } else {
            star1Script.starRoutes[star2Script] = distance;
            star2Script.starRoutes[star1Script] = distance;
        }
        var lineObj = Instantiate(line, Vector3.zero, Quaternion.identity, lines.transform);
        lineObj.name = $"{star1Script.starName} -> {star2Script.starName}";
        lineList.Add(lineObj);
        var lr = lineObj.GetComponent<LineRenderer>();
        Vector3[] linePoints = { star1.transform.position, star2.transform.position };
        lr.SetPositions(linePoints);
        lineObj.GetComponent<Line>().SetSizeValues(star1Script, star2Script, useScale);
    }

    //Called from CameraScript. Resets each line's appearance, when a new path is being selected.
    public void StartPath() {
        foreach (Transform line in lines.transform) {
            line.GetComponent<Line>().SetToDefault();
        }
    }

    //Called from CameraScript, uses DijkstraSimplified to calculate the shortest path between two stars.
    //Sets each line in the route to a marker (red, bigger).
    public void CalcPath(Star startStar, Star endStar) {
        List<Star> path = DijkstraSimplified.FindPath(startStar, endStar);
        canvasScript.DisplayPath(path);
        for (int i = 0; i < path.Count; i++) {
            if (i > 0) {
                Transform line = lines.transform.Find($"{path[i].starName} -> {path[i - 1].starName}");
                if (line == null) {
                    line = lines.transform.Find($"{path[i - 1].starName} -> {path[i].starName}");
                }
                line.GetComponent<Line>().SetToMarker();
            }
        }
    }   
}
