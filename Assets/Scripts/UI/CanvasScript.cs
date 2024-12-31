using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

//The script that controls the behaviour of all the panels.
//Every slider update and button press has its behaviour linked to here with events.

public class CanvasScript : MonoBehaviour {
    private Transform genPanel;
    private Transform infoPanel;
    private Transform routePanel;
    private TextMeshProUGUI buttonText;
    private TextMeshProUGUI zoomSpeedText;
    private TextMeshProUGUI starNum;
    private TextMeshProUGUI starRadius;
    private TextMeshProUGUI scaleMin;
    private TextMeshProUGUI scaleMax;
    private TextMeshProUGUI xMin;
    private TextMeshProUGUI xMax;
    private TextMeshProUGUI yMin;
    private TextMeshProUGUI yMax;
    private TextMeshProUGUI zMin;
    private TextMeshProUGUI zMax;
    private TextMeshProUGUI charMin;
    private TextMeshProUGUI charMax;
    private TextMeshProUGUI wordMin;
    private TextMeshProUGUI wordMax;
    private TextMeshProUGUI linkRadiusText;
    private TextMeshProUGUI linkMaxText;
    private TextMeshProUGUI linkRandomText;
    private RectTransform pathContent;
    private TextMeshProUGUI startEndText;
    private TextMeshProUGUI routeText;
    private TextMeshProUGUI pathText;
    private TextMeshProUGUI maxDistText;
    private TextMeshProUGUI maxJumpText;
    private bool panelMoving;
    private bool open;
    private CameraScript cs;
    [SerializeField] private GalaxyGenerator gg;
    private string startStar = "";
    private Vector2 routeStartPos;
    private Vector2 pathStartPos;
    private float[] lengths = new float[3];

    //Retrieves all the text for the sliders' values to update when the slider is updated.
    private void Awake() {
        genPanel = transform.Find("Generator Panel");
        infoPanel = transform.Find("Info Panel");
        routePanel = transform.Find("Route Panel");
        buttonText = genPanel.Find("Show-Hide Button/Button Text").GetComponent<TextMeshProUGUI>();
        Transform genScroll = genPanel.Find("Scroll Menu/Viewport/Content");
        starNum = genScroll.Find("Star Parent/Star Number").GetComponent<TextMeshProUGUI>();
        starRadius = genScroll.Find("NoCollide Parent/Collide Number").GetComponent<TextMeshProUGUI>();
        scaleMin = genScroll.Find("Scale Parent/Scale Min").GetComponent<TextMeshProUGUI>();
        scaleMax = genScroll.Find("Scale Parent/Scale Max").GetComponent<TextMeshProUGUI>();
        xMin = genScroll.Find("CubeRange/XMin").GetComponent<TextMeshProUGUI>();
        xMax = genScroll.Find("CubeRange/XMax").GetComponent<TextMeshProUGUI>();
        yMin = genScroll.Find("CubeRange/YMin").GetComponent<TextMeshProUGUI>();
        yMax = genScroll.Find("CubeRange/YMax").GetComponent<TextMeshProUGUI>();
        zMin = genScroll.Find("CubeRange/ZMin").GetComponent<TextMeshProUGUI>();
        zMax = genScroll.Find("CubeRange/ZMax").GetComponent<TextMeshProUGUI>();
        charMin = genScroll.Find("Names Parent/Char Min").GetComponent<TextMeshProUGUI>();
        charMax = genScroll.Find("Names Parent/Char Max").GetComponent<TextMeshProUGUI>();
        wordMin = genScroll.Find("Names Parent/Word Min").GetComponent<TextMeshProUGUI>();
        wordMax = genScroll.Find("Names Parent/Word Max").GetComponent<TextMeshProUGUI>();
        linkRadiusText = genScroll.Find("Radius Parent/Radius Number").GetComponent<TextMeshProUGUI>();
        linkMaxText = genScroll.Find("Max Parent/Max Number").GetComponent<TextMeshProUGUI>();
        linkRandomText = genScroll.Find("Random Parent/Random Number").GetComponent<TextMeshProUGUI>();
        zoomSpeedText = transform.Find("Zoom Speed/Zoom Speed Text").GetComponent<TextMeshProUGUI>();
        pathContent = routePanel.Find("Scroll Menu/Viewport/Content").GetComponent<RectTransform>();
        startEndText = pathContent.Find("Start End Label").GetComponent<TextMeshProUGUI>();
        routeText = pathContent.Find("Route Label").GetComponent<TextMeshProUGUI>();
        pathText = pathContent.Find("Path Label").GetComponent<TextMeshProUGUI>();
        maxDistText = pathContent.Find("Max Parent/Max Number").GetComponent<TextMeshProUGUI>();
        maxJumpText = pathContent.Find("Jump Parent/Jump Number").GetComponent<TextMeshProUGUI>();
        cs = Camera.main.gameObject.GetComponent<CameraScript>();
        routeStartPos = routeText.transform.localPosition;
        pathStartPos = pathText.transform.localPosition;
        ShowHide();
    }

    //Smoothly shows and hides the panels so the user can view information or focus on the galaxy.
    public void ShowHide() {
        if (panelMoving) {
            return;
        }
        Vector2 genPanelStart = genPanel.position;
        Vector2 genPanelEnd = genPanel.position;
        genPanelEnd.x += open ? 450 : -450;
        Vector2 infoPanelStart = infoPanel.position;
        Vector2 infoPanelEnd = infoPanel.position;
        infoPanelEnd.y -= open ? 200 : -200;
        Vector2 routePanelStart = routePanel.position;
        Vector2 routePanelEnd = routePanel.position;
        routePanelEnd.x += open ? -500 : 500;
        open = !open;
        buttonText.text = open ? ">" : "<";
        panelMoving = true;
        StartCoroutine(MovePanels(genPanelStart, genPanelEnd, infoPanelStart, infoPanelEnd, routePanelStart, routePanelEnd));
    }

    //Uses my custom lerp scripts from Modes of Motion to make the panel movement fluid.
    private IEnumerator MovePanels(Vector2 genStart, Vector2 genEnd, Vector2 infoStart, Vector2 infoEnd, Vector2 routeStart, Vector2 routeEnd) {
        for (float t = 0; t < 1; t += Time.deltaTime) {
            float perc = Easing.Sine.Out(t);
            Vector2 tempPos = Mathn.Vec2Lerp(genStart, genEnd, perc);
            genPanel.position = tempPos;
            tempPos = Mathn.Vec2Lerp(infoStart, infoEnd, perc);
            infoPanel.position = tempPos;
            tempPos = Mathn.Vec2Lerp(routeStart, routeEnd, perc);
            routePanel.position = tempPos;
            yield return null;
        }
        panelMoving = false;
    }

    //Called when a text element of the Routes Panel is updated,
    //The approximate lengths of each text element in the Routes Panel is stored when the text is set.
    //Here, each element below the previous (and the panel height) is reset to accomodate the length of the above text.
    //This prevents the text going out of the scrolling range, as well as unnecessary space below the text.
    private void RepositionRoute() {
        Vector2 routePos = routeStartPos;
        routePos.y -= lengths[0];
        routeText.transform.localPosition = routePos;
        Vector2 pathPos = pathStartPos;
        pathPos.y -= lengths[0] + lengths[1];
        pathText.transform.localPosition = pathPos;
        float panelHeight = 400;
        panelHeight += lengths[0] + lengths[1] + lengths[2];
        pathContent.SetHeight(panelHeight);
    }

    //Called from CameraScript, displays the names of the start and end star.
    //The approximate length of the text is stored in lengths[0].
    public void DisplayStartEnd(string name) {
        int lines = 0;
        if (startStar != "") {
            startEndText.text = $"Start: {startStar}\nEnd: {name}";
            lines = 2 + (("Start: " + startStar).Length / 17) + (("End: " + name).Length / 16); 
        } else {
            startEndText.text = $"Start: {name}\nEnd:";
            lines = 2 + (("Start: " + name).Length / 17);
        }
        lengths[0] = lines * 20;
        //Fixes formatting errors when displaying text read from the names file.
        startEndText.text = startEndText.text.Replace("\r", "");
    }

    //Called from CameraScript when the start star has been selected.
    public void SetStartStar(string name) {
        startStar = name;
    }

    //Called from CameraScript when the path has been determined.
    //Sets the text of the path to tell the user that the path calculation is processing.
    //Stores the rough length in lengths[1].
    public void StartPath(Star star1, Star star2) {
        routeText.text = $"{star1.starName} -> {star2.starName}";
        routeText.text = routeText.text.Replace("\r", "");
        float lines = routeText.text.Length / 14;
        lengths[1] = lines * 25;
        pathText.text = "Calculating. Please wait!";
        RepositionRoute();
        startStar = "";
    }

    //Called from GalaxyGenerator when a new galaxy has been made.
    public void ClearPath() {
        routeText.text = "";
        pathText.text = "";
        startStar = "";
        lengths = new float[3];
        RepositionRoute();
    }

    //Called from GalaxyGenerator when the path has been calculated.
    //Displays every star on the way, the cost of each route, and the total cost and jumps.
    //Stores the rough length of the text in lengths[2].
    public void DisplayPath(List<Star> path) {
        string displayText = "";
        string costText = "";
        float totalCost = 0;
        int totalJumps = 0;
        int lines = 0;
        if (path.Count > 0) {
            for (int i = 0; i < path.Count; i++) {
                lines++;
                if (i == path.Count - 1) {
                    displayText += path[i].starName;
                    lines += path[i].starName.Length / 14;
                    break;
                } else {
                    float cost = path[i].starRoutes[path[i + 1]];
                    totalCost += cost;
                    string lineString = $"{path[i].starName} -> ({cost.ToString("n1")})\n";
                    displayText += lineString;
                    lines += lineString.Length / 14;
                    totalJumps++;
                }
            }
            displayText += $"\n\nTotal cost = {totalCost.ToString("n2")}\nTotal Jumps: {totalJumps}";
            lines += 3;
        } else {
            displayText = "No route detected.";
            lines = 1;
        }
        lengths[2] = lines * 25;
        pathText.text = displayText + "\n" + costText;
        pathText.text = pathText.text.Replace("\r", "");
        RepositionRoute();
    }

    //Slider, Button and Toggle behaviour below.
    //The value and passes it into the correct script. (Or actvates a function, if a button).
    //Sets the related text to the value, to 1 decimal place (if a slider with float value).

    public void ChangeStarNum(float value) {
        gg.starCount = (int)value;
        starNum.text = value.ToString();
    }

    public void ChangeStarRadius(float value) {
        gg.starRadius = value;
        starRadius.text = value.ToString("n1");
    }

    //If a minimum value is >= the related maximum, it is clamped to that value.
    public void ChangeScaleMin(float value) {
        if (value >= gg.scaleRange.y) {
            value = gg.scaleRange.y;
        }
        gg.scaleRange.x = value;
        scaleMin.text = value.ToString("n1");
    }

    //If a maximum value is <= the related minimum, it is clamped to that value.
    public void ChangeScaleMax(float value) {
        if (value <= gg.scaleRange.x) {
            value = gg.scaleRange.x;
        }
        gg.scaleRange.y = value;
        scaleMax.text = value.ToString("n1");
    }

    public void ChangeXMin(float value) {
        if (value >= gg.xRange.y) {
            value = gg.xRange.y;
        }
        gg.xRange.x = value;
        xMin.text = value.ToString("n1");
    }

    public void ChangeXMax(float value) {
        if (value <= gg.xRange.x) {
            value = gg.xRange.x;
        }
        gg.xRange.y = value;
        xMax.text = value.ToString("n1");
    }

    public void ChangeYMin(float value) {
        if (value >= gg.yRange.y) {
            value = gg.yRange.y;
        }
        gg.yRange.x = value;
        yMin.text = value.ToString("n1");
    }

    public void ChangeYMax(float value) {
        if (value <= gg.yRange.x) {
            value = gg.yRange.x;
        }
        gg.yRange.y = value;
        yMax.text = value.ToString("n1");
    }

    public void ChangeZMin(float value) {
        if (value >= gg.zRange.y) {
            value = gg.zRange.y;
        }
        gg.zRange.x = value;
        zMin.text = value.ToString("n1");
    }

    public void ChangeZMax(float value) {
        if (value <= gg.zRange.x) {
            value = gg.zRange.x;
        }
        gg.zRange.y = value;
        zMax.text = value.ToString("n1");
    }

    public void ChangeCharMin(float value) {
        if (value >= gg.nameChar.y) {
            value = gg.nameChar.y;
        }
        gg.nameChar.x = (int)value;
        charMin.text = value.ToString();
    }

    public void ChangeCharMax(float value) {
        if (value <= gg.nameChar.x) {
            value = gg.nameChar.x;
        }
        gg.nameChar.y = (int)value;
        charMax.text = value.ToString();
    }

    public void ChangeWordMin(float value) {
        if (value >= gg.nameCount.y) {
            value = gg.nameCount.y;
        }
        gg.nameCount.x = (int)value;
        wordMin.text = value.ToString();
    }

    public void ChangeWordMax(float value) {
        if (value <= gg.nameCount.x) {
            value = gg.nameCount.x;
        }
        gg.nameCount.y = (int)value;
        wordMax.text = value.ToString();
    }

    public void ChangeRange(float value) {
        gg.starLinkRadius = value;
        linkRadiusText.text = value.ToString("n1");
    }

    public void ChangeLinkMax(float value) {
        gg.starLinkMax = (int)value;
        if (value > gg.linkRandomRange && gg.linkRandomRange != 0) {
            ChangeLinkRandom(value);
        }
        linkMaxText.text = value.ToString();
    }

    public void ChangeLinkRandom(float value) {
        if (value < gg.starLinkMax && value != 0) {
            value = gg.starLinkMax;
        }
        gg.linkRandomRange = (int)value;
        linkRandomText.text = value.ToString();
    }

    public void ToggleScale(bool value) {
        gg.useScale = value;
    }

    public void ChangeSeed(string seed) {
        gg.seed = seed;
    }

    public void GenerateStars() {
        gg.GenerateStars();
    }

    public void GenerateRoutes() {
        gg.GenerateRoutes();
    }

    public void ChangeMaxPathDistance(float value) {
        DijkstraSimplified.maxTotalDistance = value;
        maxDistText.text = value.ToString("n1");
    }

    public void ChangeMaxJumpDistance(float value) {
        DijkstraSimplified.maxJumpDistance = value;
        maxJumpText.text = value.ToString("n1");
    }

    //Called from CameraScript when the user changes the speed of moving the camera.
    private void OnZoomRespeed() {
        zoomSpeedText.text = cs.zoomSpeed.ToString("F1");
    }
}
