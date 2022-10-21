using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRController : MonoBehaviour
{

    public float moveSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var leftHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandedControllers);

        foreach (var device in leftHandedControllers)
        {
            Vector2 value;

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out value))
            {
                Debug.Log("VRINput: " + value.x + " | " + value.y);

                Vector3 movementX = Camera.main.transform.right * value.x * moveSpeed * Time.deltaTime;
                Vector3 movementZ = Camera.main.transform.forward * value.y * moveSpeed * Time.deltaTime;

                movementX.y = movementZ.y = 0;

                transform.position += movementX;
                transform.position += movementZ;
            }
        }
    }
}
