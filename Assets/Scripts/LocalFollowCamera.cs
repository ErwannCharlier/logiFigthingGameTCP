using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LocalFollowCamera : NetworkBehaviour
{
    [Header("Camera")]
    public Vector3 offset = new Vector3(0f, 2.2f, -4.5f);
    public float positionSmooth = 12f;
    public float rotationSmooth = 12f;

    Transform cam;

    public override void OnStartLocalPlayer()
    {
        cam = Camera.main.transform;

        cam.position = transform.TransformPoint(offset);
        cam.rotation = Quaternion.LookRotation(transform.position + Vector3.up * 1.5f - cam.position);
    }

    void LateUpdate()
    {
        if (!isLocalPlayer) return;
        if (cam == null) return;

        Vector3 desiredPos = transform.TransformPoint(offset);
        cam.position = Vector3.Lerp(cam.position, desiredPos, 1f - Mathf.Exp(-positionSmooth * Time.deltaTime));

        Vector3 lookTarget = transform.position + Vector3.up * 1.5f;
        Quaternion desiredRot = Quaternion.LookRotation(lookTarget - cam.position);
        cam.rotation = Quaternion.Lerp(cam.rotation, desiredRot, 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
    }
}