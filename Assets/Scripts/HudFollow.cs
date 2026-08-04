using UnityEngine;

public class HudFollow : MonoBehaviour
{
    public Transform cameraTransform;   
    public float distance = 2f;
    public float height = -0.2f;
    public float followSpeed = 4f;

    void Update()
    {
        if (cameraTransform == null)
        {
            return;
        }

        
        Vector3 target = cameraTransform.position
                       + cameraTransform.forward * distance
                       + Vector3.up * height;

        
        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);

        
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}