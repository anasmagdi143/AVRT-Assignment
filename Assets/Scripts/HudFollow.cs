using UnityEngine;

public class HudFollow : MonoBehaviour
{
    public Transform cameraTransform;   // drag the Main Camera here
    public float distance = 2f;
    public float height = -0.2f;
    public float followSpeed = 4f;

    void Update()
    {
        if (cameraTransform == null)
        {
            return;
        }

        // target spot: in front of the camera, slightly down
        Vector3 target = cameraTransform.position
                       + cameraTransform.forward * distance
                       + Vector3.up * height;

        // ease toward it instead of snapping (this is what stops the nausea)
        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);

        // always face the player
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}