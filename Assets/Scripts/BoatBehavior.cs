using UnityEngine;

public class BoatBehavior : MonoBehaviour
{
    public float sitDepth = 3.1f;
    void Update() {
        float waterHeight = -999;
        for (int step = -3; step <= 3; step++) {
            waterHeight = Mathf.Max(waterHeight, WaveMatMgr.instance.HeightAt(transform.position.XZ() + transform.forward.XY()*step));
        }
        float newHeight = waterHeight - sitDepth;
        float deltaY = newHeight - transform.position.y;

        transform.position += Vector3.up * deltaY;
        PlayerController.instance.boatYDrift += deltaY;
    }
}
