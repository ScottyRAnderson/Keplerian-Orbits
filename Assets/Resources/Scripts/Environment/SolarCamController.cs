using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class SolarCamController : MonoBehaviour
{
    [SerializeField]
    private float panRate = 1f;
    [SerializeField]
    private float yawRate = 90f;
    [SerializeField]
    private float pitchRate = 45f;
    [SerializeField]
    private float zoomRate = 10000f;

    [SerializeField]
    private float maxPanDistance = 30000f;
    [SerializeField][MinMax()]
    private Vector2 minMaxPitchAngle = new Vector2(30, 90);
    [SerializeField][MinMax()]
    private Vector2 minMaxZoomDistance = new Vector2(3000f, 50000f);

    [SerializeField]
    private float defaultYawAngle = 0f;
    [SerializeField]
    private float defaultPitchAngle = 20f;
    [SerializeField]
    private float defaultZoom = 100f;
    [SerializeField]
    private float camResetRate = 2f;

    private Camera attachedCamera;

    private Vector3 targetPos;
    private float yaw;
    private float pitch;
    private float zoom;

    private float resetTimer;
    private bool resettingCamera;
    private bool cameraLocked;

    KeyCode zoomInKey = KeyCode.LeftShift;
    KeyCode zoomOutKey = KeyCode.LeftControl;
    KeyCode forwardKey = KeyCode.W;
    KeyCode backwardKey = KeyCode.S;
    KeyCode leftKey = KeyCode.A;
    KeyCode rightKey = KeyCode.D;

    public Camera AttachedCamera { get { return attachedCamera; } }

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        attachedCamera = GetComponent<Camera>();
        ResetCamera();
    }

    public void ResetCamera()
    {
        resettingCamera = true;
        resetTimer = camResetRate;
    }

    public void ToggleCameraLockState(){
        SetCameraLockState(!cameraLocked);
    }

    public void SetCameraLockState(bool locked)
    {
        cameraLocked = locked;
        Cursor.visible = cameraLocked ? true : false;
        Cursor.lockState = cameraLocked ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void Update()
    {
        // Camera lock state handling
        if(Input.GetKeyDown(KeyCode.Escape)){
            ToggleCameraLockState();
        }

        if(cameraLocked && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()){
            SetCameraLockState(false);
        }

        if(resettingCamera)
        {
            if(resetTimer > camResetRate / 1.2f)
            {
                // Interpolate parameters back to defaults
                resetTimer -= Time.deltaTime;
                yaw = Mathf.Lerp(yaw, defaultYawAngle, (camResetRate - resetTimer) / camResetRate);
                pitch = Mathf.Lerp(pitch, defaultPitchAngle, (camResetRate - resetTimer) / camResetRate);
                zoom = Mathf.Lerp(zoom, defaultZoom, (camResetRate - resetTimer) / camResetRate);
                targetPos = Vector3.Lerp(targetPos, CelestialManager.centralBody.transform.position, (camResetRate - resetTimer) / camResetRate);
                targetPos.y = 0f;
            }
            else
            {
                resettingCamera = false;
                resetTimer = camResetRate;
            }
        }

        // Input
        Vector2 keyInput = (!resettingCamera && !cameraLocked) ? new Vector2(GetInputAxis(rightKey, leftKey), GetInputAxis(forwardKey, backwardKey)) : Vector2.zero;
        Vector2 mouseInput = (!resettingCamera && !cameraLocked) ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) : Vector2.zero;
        float zoomInput = (!resettingCamera && !cameraLocked) ? GetInputAxis(zoomOutKey, zoomInKey) : 0f;

        Vector3 planarProjection = Vector3.Scale(transform.forward + transform.up, new Vector3(1f, 0f, 1f)).normalized;
        Vector3 planarPosition = transform.right * keyInput.x + planarProjection * keyInput.y;
        targetPos += planarPosition * panRate * zoom * Time.deltaTime;

        if (targetPos.sqrMagnitude > maxPanDistance * maxPanDistance){
            targetPos = targetPos.normalized * maxPanDistance;
        }

        // Advance properties
        zoom += zoomInput * zoomRate * Time.deltaTime;
        zoom = Mathf.Clamp(zoom, minMaxZoomDistance.x, minMaxZoomDistance.y);

        yaw += mouseInput.x * yawRate * Time.deltaTime;
        yaw = MathHelper.WrapAngle(yaw);

        pitch += mouseInput.y * -1f * pitchRate * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minMaxPitchAngle.x, minMaxPitchAngle.y);

        // Apply transforms
        Quaternion quaternion = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 position = targetPos + quaternion * Vector3.back * zoom;
        transform.rotation = quaternion;
        transform.position = position;
    }

    private int GetInputAxis(KeyCode positiveAxis, KeyCode negativeAxis){
        return Input.GetKey(positiveAxis) ? 1 : Input.GetKey(negativeAxis) ? -1 : 0;
    }
}