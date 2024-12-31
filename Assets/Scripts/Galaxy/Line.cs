using UnityEngine;

//On each line between stars, to let them change appearance and give them a mesh collider.
public class Line : MonoBehaviour {
    private Vector2 sizeValues = new Vector2();
    private LineRenderer lr;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    public void SetSizeValues(Star star1, Star star2, bool scaleIncluded) {
        //Sets the default size of the line.
        //If scale is included in route weight, smaller stars get smaller lines (and vice versa).
        if (scaleIncluded) {
            sizeValues.x = 0.16f * (star1.scale * star1.scale * 0.5f);
            sizeValues.y = 0.16f * (star2.scale * star2.scale * 0.5f);
        } else {
            sizeValues = new Vector2( 0.08f, 0.08f );
        }
        
        SetToDefault();

        //https://forum.unity.com/threads/how-to-add-collider-to-a-line-renderer.505307/
        //A mesh collider is added so that the player can raycast the line to see information about it.
        var mc = GetComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        lr.BakeMesh(mesh, true);
        mc.sharedMesh = mesh;
    }


    //Sets the line to the default appearance.
    public void SetToDefault() {
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.startWidth = sizeValues.x;
        lr.endWidth = sizeValues.y;
    }

    //Sets the line to the appearance of a marker (used in a route).
    public void SetToMarker() {
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.startWidth = sizeValues.x * 3;
        lr.endWidth = sizeValues.y * 3;
    }
}
