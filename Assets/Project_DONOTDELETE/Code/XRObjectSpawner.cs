using UnityEngine;
using UnityEngine.XR;

public class XRObjectSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject objectPrefab;

    [Header("Spawn Settings")]
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    [Header("Rotation Offset (in degrees)")]
    [Tooltip("Adjust this to rotate the object relative to the target direction. For example, x = -90 makes feet point toward the target.")]
    public Vector3 rotationOffset;

    private GameObject spawnedObject;

    void Update()
    {
        // Right trigger spawn (or respawn)
        if (GetRightTriggerDown())
        {
            SpawnObject();
        }

        // Left trigger rotate based on left hand position
        if (spawnedObject != null && GetLeftTrigger())
        {
            RotateObjectBasedOnLeftHand();
        }
    }

    private bool GetRightTriggerDown()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool pressed);
        return pressed;
    }

    private bool GetLeftTrigger()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool pressed);
        return pressed;
    }

    private void SpawnObject()
    {
        // Destroy old object if exists
        if (spawnedObject != null)
            Destroy(spawnedObject);

        spawnedObject = Instantiate(objectPrefab, rightHandTransform.position, rightHandTransform.rotation);
    }

    private void RotateObjectBasedOnLeftHand()
    {
        Vector3 direction = leftHandTransform.position - spawnedObject.transform.position;
        if (direction.sqrMagnitude > 0.001f) // avoid zero-length vector
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Apply rotation offset
            targetRotation *= Quaternion.Euler(rotationOffset);

            spawnedObject.transform.rotation = targetRotation;
        }
    }
}
