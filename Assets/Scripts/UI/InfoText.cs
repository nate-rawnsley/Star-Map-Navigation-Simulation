using UnityEngine;
using TMPro;

//Controls the text on the Info Panel at the bottom.
//The cameraScript passes the tag of any element hovered over and it switches displayed text based on the tag.
//If an object (star or route) is hovered over, it passes its name as well so the object can be found.
public class InfoText : MonoBehaviour {
    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        DisplayDefaultText();
    }

    public void ChangeText(string tag, GameObject obj = null) {
        //Information about stars and routes are displayed.
        //Stars display every route they have and routes display their costs either way.
        if (obj != null) {
            string name = obj.name;
            switch (tag) {
                case "Star":
                    Star script = obj.GetComponent<Star>();
                    string allRoutes = "";
                    var dict = script.starRoutes;
                    bool start = true;
                    foreach (var route in dict) {
                        if (start) {
                            start = false;
                        } else {
                            allRoutes += ", ";
                        }
                        allRoutes += $"{route.Key.starName} : {route.Value}";
                    }
                    text.text = name + "\nRoutes: " + allRoutes;
                    break;
                case "Route":
                    string[] starNames = name.Split(" -> ");
                    Star star1 = GameObject.Find("Galaxy/" + starNames[0]).GetComponent<Star>();
                    Star star2 = GameObject.Find("Galaxy/" + starNames[1]).GetComponent<Star>();
                    //If the route has different costs in either direction, they are displayed separately.
                    if (star1.starRoutes[star2] == star2.starRoutes[star1]) {
                        text.text = $"{starNames[0]} <-> {starNames[1]} : {star1.starRoutes[star2]}";
                    } else {
                        text.text = $"{starNames[0]} -> {starNames[1]} : {star1.starRoutes[star2]}\n" +
                        $"{starNames[1]} -> {starNames[0]} : {star2.starRoutes[star1]}";
                    }
                    break;

            }
            text.text = text.text.Replace("\r", "");
            return;
        }
        //UI elements have specific tags associated with them, which determine what tooltip is displayed.
        switch (tag) {
            case "StarSlider":
                text.text = "Adjust the number of stars in the system.";
                return;
            case "StarRadius":
                text.text = "Adjust the minimum distance between stars.\n(Might not always work if there's not enough room, but will try)";
                return;
            case "StarScale":
                text.text = "Adjust the minimum and maximum sizes of stars in the system.\n(Sizes are randomly picked within this range)";
                return;
            case "GenRange":
                text.text = "Adjust the bounds of the stars' position within the system.";
                return;
            case "WordLength":
                text.text = "Adjusts how long a word within the star's name can be.";
                return;
            case "WordCount":
                text.text = "Adjusts how many word a star's name can be made up of.\n(Randomly picked within this range.)";
                return;
            case "RouteRadius":
                text.text = "Adjust the minimum distance between stars for routes to be created.\n(Set to 0 to ignore radius.)";
                return;
            case "RouteMax":
                text.text = "Adjust the maximum number of routes between stars.\n(Set to 0 to ignore maximum.)";
                return;
            case "RouteRandom":
                text.text = "Randomly pick routes from a list of this size.\n(Set to 0 to ignore this.)";
                return;
            case "ScaleToggle":
                text.text = "Use the scale of stars in route weight calculations.\n(Easier to leave smaller stars, easier to approach larger stars)";
                return;
            case "SeedInput":
                text.text = "Input a custom seed for random generation here.\n(Leave blank to use a random seed.)";
                return;
            case "GenerateButton":
                text.text = "Press to generate stars within the above parameters.";
                return;
            case "RouteButton":
                text.text = "Press to generate routes between stars in the system, within above parameters";
                return;
            case "ShowUI":
                text.text = "Show/hide UI.";
                return;
            case "ZoomSpeed":
                text.text = "The speed the camera moves through the scene. Adjust with scroll wheel.";
                return;
            case "InfoText":
                return;
            case "RoutePanel":
                text.text = "This panel displays the optimal route between the start and end star selected.\n(Select stars with left click.)";
                return;
            case "MaxDistance":
                text.text = "Adjust the maximum total cost of the path.\n(Set to 0 to ignore maximum.)";
                return;
            case "MaxJump":
                text.text = "Adjust the maximum cost of a path to be useable.\n(Set to 0 to ignore maximum.)";
                return;
        }
        //If an element with no recognised tag is hovered over, it informs the user how to use the panel.
        DisplayDefaultText();
    }

    private void DisplayDefaultText() {
        text.text = "Hover over a UI label to learn its function.\nOr hover over a star or route in the galaxy.";
    }
}
