using UnityEngine;

//On each start and end marker, increases size and adds a basic colour lerp to make it stand out from other stars
public class Marker : MonoBehaviour {
    private float time;
    private Renderer rend;

    private void Awake() {
        rend = gameObject.GetComponent<Renderer>();
    }

    private void Update() {
        float perc = Easing.PP(Easing.Sine.Out(time));
        float greenVal = Mathn.Lerp(0, 1, perc);
        Color tempColour = new Color(0.95f, greenVal, 0, 1);
        rend.material.color = tempColour;
        time += Time.deltaTime * 0.7f;
    }
}
