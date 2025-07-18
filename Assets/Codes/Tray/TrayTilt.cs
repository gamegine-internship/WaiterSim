using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrayTilt : MonoBehaviour
{
    [Header("References")]
    public Transform targetLocation;
    public Transform playerLocation;

    [Header("Tilt Settings")]
    [SerializeField] private float maxTiltZ = 30f;
    [SerializeField] private float tiltSpeedZ = 0.5f; // 1 means max tilt, 0 means no tilt
    
    [Header("Tween Settings")]
    [SerializeField] private float positionUpdateThreshold = 0.1f;
    [SerializeField] private float rotationUpdateThreshold = 0.5f;
    [SerializeField] private float tiltTweenDuration = 0.2f;

    private Vector3 lastTargetPos;

    private Tween positionTween;
    private Tween yawTween;
    private Tween tiltTween;
    private Tween manualTiltTween;

    private float currentTiltZ = 0f;
    private float manualTiltZ = 0f;

    void Update()
    {
        StickToTargetPosition();
        HandleCombinedTrayTilt();
    }

    private void StickToTargetPosition()
    {
        if (Vector3.Distance(transform.position, targetLocation.position) > positionUpdateThreshold)
        {
            positionTween?.Kill();
            positionTween = transform.DOMove(targetLocation.position, 0.1f).SetEase(Ease.OutSine);
        }
        positionTween = transform.DOMove(targetLocation.position, 0.1f).SetEase(Ease.OutSine);
        
        //yaw
        float yawDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetLocation.eulerAngles.y);
        if (Mathf.Abs(yawDifference) > rotationUpdateThreshold)
        {
            yawTween?.Kill();
            Vector3 targetEuler = new Vector3(0f, targetLocation.eulerAngles.y, currentTiltZ);
            yawTween = transform.DORotate(targetEuler, 0.1f).SetEase(Ease.OutSine);
        }

    }

    private void HandleCombinedTrayTilt()
    {
        float moveX = Input.GetAxis("Horizontal");
        // Handle manual tilt offset input
        if (Input.GetKey(KeyCode.Q))
        {
            manualTiltTween?.Kill();
            //manualTiltZ += 30f * Time.deltaTime;
            manualTiltTween = DOTween.To(() => manualTiltZ, x => manualTiltZ = x, 20f, 2f).SetEase(Ease.OutQuad);
        } 
        else if (Input.GetKey(KeyCode.E))
        {
            manualTiltTween?.Kill();
            //manualTiltZ -= 30f * Time.deltaTime;
            manualTiltTween = DOTween.To(() => manualTiltZ, x => manualTiltZ = x, -20f, 2f).SetEase(Ease.OutQuad);
        }
        else
        {
            manualTiltTween?.Kill();
            //manualTiltZ = Mathf.MoveTowards(manualTiltZ, 0f, Time.deltaTime * 15f); // 15 degrees/sec
            manualTiltTween = DOTween.To(() => manualTiltZ, x => manualTiltZ = x, 0f, 2f).SetEase(Ease.OutQuad);
        }

        manualTiltZ = Mathf.Clamp(manualTiltZ, -maxTiltZ, maxTiltZ);

        // Auto tilt based on movement
        float autoTiltZ = -moveX * maxTiltZ * tiltSpeedZ;

        // Final tilt is combination of auto + manual
        float targetTiltZ = Mathf.Clamp(autoTiltZ + manualTiltZ, -maxTiltZ, maxTiltZ);

        if (Mathf.Abs(currentTiltZ - targetTiltZ) > 0.5f)
        {
            // Cancel any ongoing tilt tween
            tiltTween?.Kill();

            // Preserve the current yaw 
            float yaw = transform.eulerAngles.y;
            Vector3 targetRotation = new Vector3(0f, yaw, targetTiltZ);

            // Smoothly rotate
            tiltTween = transform.DORotate(targetRotation, tiltTweenDuration)
                .SetEase(Ease.OutQuad)
                .OnUpdate(() =>
                {
                    // Update currentTiltZ
                    currentTiltZ = transform.eulerAngles.z;
                    if (currentTiltZ > 180f) currentTiltZ -= 360f;
                });
        }

    }
}
