using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private void LateUpdate()
    {
        /* UI가 해당 카메라로 Look At 회전
        Vector3 dir = this.transform.position - MainCameraController.MainCameraTransform.position;
        this.transform.LookAt(this.transform.position + dir);
        */
        
        this.transform.rotation = MainCameraController.MainCameraTransform.rotation;
    }
}
