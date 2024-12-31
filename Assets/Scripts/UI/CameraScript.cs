using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

//On the camera, controls its position, direction facing, starting a path and getting info on a component.
//Uses the new input system to control behaviour, which sends messages.
public class CameraScript : MonoBehaviour {
    [SerializeField] private GalaxyGenerator gg;
    private InputAction mousePos;
    private CanvasScript canvasScript;
    private InfoText infoScript;

    private Star startStar;
    private GameObject startMarker;
    private GameObject endMarker;
    private Vector3 centre = new Vector3();

    private float distance;
    private float maxDist;
    public float zoomSpeed = 10f;
    private float zoomVal;
    private float zoomSpeedVal;

    private Vector3 position;
    private Vector3 outerPos;
    private InputAction xYDisplacement;
    private InputAction zDisplacement;
    private Vector3 centreOffset;
    private InputAction rotateVal;
    private Vector2 rotation;

    private void Awake() {
        mousePos = GetComponent<PlayerInput>().actions.FindAction("MousePos");
        canvasScript = GameObject.Find("Canvas").GetComponent<CanvasScript>();
        infoScript = GameObject.Find("Canvas/Info Panel/Scroll View/Viewport/Content/Info Text").GetComponent<InfoText>();
        xYDisplacement = GetComponent<PlayerInput>().actions.FindAction("MoveCentre");
        zDisplacement = GetComponent<PlayerInput>().actions.FindAction("MoveCentreZ");
        rotateVal = GetComponent<PlayerInput>().actions.FindAction("RotateCamera");
    }

    //When the player clicks, raycasts from that position to find the star clicked.
    //Stores the first in 'tempStar', and after selecting the second, prompts GalaxyGenerator to calculate the path.
    private void OnSelectStar() {
        //Doesn't allow the user to select stars if no routes are generated between them, to prevent errors.
        if (gg.lineList.Count == 0) {
            return;
        }
        Ray ray = GetComponent<Camera>().ScreenPointToRay(mousePos.ReadValue<Vector2>());
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Star tempStar = null;
        GameObject hitObject = null;
        foreach (var hit in hits) {
            tempStar = hit.transform.gameObject.GetComponent<Star>();
            if (tempStar != null) {
                hitObject = hit.transform.gameObject;
                break;
            }
        }
        if (tempStar == null) {
            return;
        }

        if (startStar == null) {
            startStar = tempStar;
            canvasScript.SetStartStar(tempStar.starName);
            gg.StartPath();
            if (startMarker != null) {
                Destroy(startMarker);
                Destroy(endMarker);
            }
            startMarker = Instantiate(hitObject);
            MakeMarker(startMarker, hitObject.name);
        } else {
            canvasScript.StartPath(startStar, tempStar);
            endMarker = Instantiate(hitObject);
            MakeMarker(endMarker, hitObject.name);
            StartCoroutine(DelayCalcPath(tempStar));
        }
    }

    //Delays the start of calculating the path, so the UI can display a message before the game pauses to calculate.
    private IEnumerator DelayCalcPath(Star tempStar) {
        yield return new WaitForFixedUpdate();
        gg.CalcPath(startStar, tempStar);
        startStar = null;
    }

    //The marker of a star in the route is a clone scaled up, with the Marker component.
    //This adds support for stars of any shape and size, but other star meshes are unused in the project.
    private void MakeMarker(GameObject marker, string name) {
        marker.transform.localScale = marker.transform.localScale * 1.5f;
        marker.AddComponent<Marker>();
        marker.name = $"{name} (Already Selected)";
    }

    //Resets all the values when new stars are generated in the galaxy.
    //Called by event from GalaxyGenerator.
    public void NewGenerate() {
        startStar = null;
        centre = gg.centre;
        centreOffset = new Vector3();
        distance = gg.bounds[2].y;
        
        //Calculates the approximate distance from the galaxy to see the whole thing.
        if ((gg.bounds[0].y - gg.bounds[0].x) / 16 > (gg.bounds[1].y - gg.bounds[1].x) / 9) {
            distance += (gg.bounds[0].y - gg.bounds[0].x) / 2;
        } else {
            distance += (gg.bounds[1].y - gg.bounds[1].x) / 1.125f;
        }

        maxDist = distance;
        outerPos = new Vector3(centre.x, centre.y, distance);
        if (startMarker != null) {
            Destroy(startMarker);
        }
        if (endMarker != null) {
            Destroy(endMarker);
        }
    }

    //Controls for the camera's moving towards the centre.
    private void OnZoom(InputValue value) {
        zoomVal = value.Get<float>();
    }

    private void OnZoomRespeed(InputValue value) {
        zoomSpeedVal = value.Get<float>();
    }

    public void ReCentre() {
        centre = gg.centre;
        centreOffset = new Vector3();
    }

    private void OnResetCamera() {
        ReCentre();
        rotation = Vector3.zero;
        distance = maxDist;
    }

    //The camera faces towards the centre each frame, with its position set relative to the centre.
    //The player can change the distance from the centre, move the centre, and rotate around it.
    private void Update() {
        zoomSpeed += zoomSpeedVal * Time.deltaTime;
        zoomSpeed = Mathf.Clamp(zoomSpeed, 1, 50);
        distance -= zoomVal * zoomSpeed * Time.deltaTime;
        
        centreOffset.x += -xYDisplacement.ReadValue<Vector2>().x * zoomSpeed * Time.deltaTime;
        centreOffset.y += xYDisplacement.ReadValue<Vector2>().y * zoomSpeed * Time.deltaTime;
        centreOffset.z += zDisplacement.ReadValue<float>() * zoomSpeed * Time.deltaTime;
        rotation += rotateVal.ReadValue<Vector2>() * zoomSpeed * 0.125f * Time.deltaTime;
        rotation.x %= 2 * Mathf.PI;
        rotation.y = Mathf.Clamp(rotation.y, -Mathf.PI / 2, Mathf.PI / 2);

        position = outerPos;
        //The x and z position are a circle around the centre.
        //There isn't full sphere support, but the y position changes enough to see all of the stars.
        position.x = distance * Mathf.Cos(rotation.x) + centre.x;
        position.y = distance * Mathf.Sin(rotation.y) + centre.y;
        position.z = (distance * Mathf.Sin(rotation.x) + centre.z);
        transform.position = position;
        transform.LookAt(centre + centreOffset);

        //Every frame it raycasts to see if any UI elements, stars or lines are selected by the cursor.
        //Passes the tag of any UI and the tag & name of any gameObjects.
        //https://stackoverflow.com/questions/69986439/interact-with-ui-via-raycast-unity
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePos.ReadValue<Vector2>();
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool starSelected = false;
        if (results.Where(r => r.gameObject.layer == 5).Count() > 0) {
            bool untagged = true;
            int ind = -1;
            while (untagged && ind < results.Where(r => r.gameObject.layer == 5).Count() - 1) {
                ind++;
                if (results[ind].gameObject.tag != "Untagged") {
                    untagged = false;
                }
            }
            infoScript.ChangeText(results[ind].gameObject.tag);
        } else {
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(mousePos.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out hit)) {
                infoScript.ChangeText(hit.transform.gameObject.tag, hit.transform.gameObject);
                //If a star is hovered over, displays it in the Route Panel.
                if (hit.transform.gameObject.CompareTag("Star") && gg.lineList.Count > 0) {
                    starSelected = true;
                    canvasScript.DisplayStartEnd(hit.transform.gameObject.GetComponent<Star>().starName);
                }
            }
        }
        if (!starSelected) {
            canvasScript.DisplayStartEnd("");
        }
    }

    //Pressing escape closes the application.
    private void OnQuit() {
        Application.Quit();
    }
}
