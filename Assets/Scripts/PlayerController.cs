using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Camera cam;
    private CharacterController charController;
    public Transform eyePoint;
    public Transform hipPoint;
    public SextantController sextant;
    private bool mousePressed = false;
    public Image hoverPip;
    public LayerMask btnLayer;
    void Start() {
        cam = Camera.main;
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update() {
        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { input += Vector3.forward; }
        if (Input.GetKey(KeyCode.A)) { input += Vector3.left; }
        if (Input.GetKey(KeyCode.S)) { input += Vector3.back; }
        if (Input.GetKey(KeyCode.D)) { input += Vector3.right; }
        var oldPos = transform.position;
        charController.Move(transform.rotation*input*10f * Time.deltaTime);
        sextant.transform.position += transform.position - oldPos;

        sextant.focusing = mousePressed || Input.GetKey(KeyCode.LeftShift);
        sextant.targetLocation = sextant.focusing ? eyePoint : hipPoint;

        int clickAction = 0;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, 10, btnLayer)) {
            clickAction = hit.collider.name.Contains("next") ? 1 : 2;
        }

        hoverPip.gameObject.SetActive(clickAction > 0);

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (clickAction == 0) {
                mousePressed = true;
            } else if (clickAction == 1) {
                SlideDeck.instance.Next();
            } else if (clickAction == 2) {
                SlideDeck.instance.Prev();
            }
        }
        if (!Input.GetKey(KeyCode.Mouse0)) {
            mousePressed = false;
        }


        // Pressed LMB controls the sextant
        if (sextant.focusing) {
            if (Input.GetKey(KeyCode.Mouse1)) {
                sextant.sextantAngle = Mathf.Clamp(sextant.sextantAngle + Input.mousePositionDelta.y*.25f, 0, 50);
            } else {
                sextant.sextantAngle = Mathf.Clamp(sextant.sextantAngle + Input.mouseScrollDelta.y, 0, 50);
            }
        }
        
        if (!sextant.focusing || !Input.GetKey(KeyCode.Mouse1)) {
            transform.eulerAngles = transform.rotation.eulerAngles + Vector3.up * Input.mousePositionDelta.x*.06f;
            float xAng = cam.transform.localEulerAngles.x > 180 ? cam.transform.localEulerAngles.x - 360 : cam.transform.localEulerAngles.x;
            cam.transform.localEulerAngles = new Vector3(
                Mathf.Clamp(xAng + Input.mousePositionDelta.y*-.1f + 90, 5, 175) - 90,
                0,
                0
            );
        }
    }
}
