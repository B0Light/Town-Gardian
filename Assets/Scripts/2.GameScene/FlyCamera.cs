using UnityEngine;
using System.Collections;

public class FlyCamera : MonoBehaviour
{
    [SerializeField] string _tagOfFollower = "Player";
    private Transform _trFollower;

    [SerializeField] Vector3 _headCorrection = Vector3.up;

    public float cameraSensitivity = 180;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start() {
        //Screen.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _trFollower = GameObject.FindWithTag(_tagOfFollower).transform;
    }

    void Update() {
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        //transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        transform.position = _trFollower.position + _headCorrection;

    }
}