using UnityEngine;

public class SlideDeckSextant : MonoBehaviour
{
    public Transform arm;
    public TMPro.TMP_Text reading;
    public bool animated;
    private float angle = 0;
    void Update() {
        if (animated) {
            float t = Time.time / 3f;
            if (((int)t) % 4 == 0) {
                t = 0;
            } else if (((int)t) % 4 == 1) {
                t = t%1;
            } else if (((int)t) % 4 == 2) {
                t = 1;
            } else {
                t = 1 - (t%1);
            }
            angle = 75 * t;
        }
        reading.text = Mathf.RoundToInt(angle).ToString();
        arm.transform.localRotation = Quaternion.Euler(
            arm.transform.localRotation.x,
            arm.transform.localRotation.y,
            -25 + angle*.666f
        );
    }
}
