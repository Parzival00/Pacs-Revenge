using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Transform camera;

    bool interruptable;

    Coroutine shakeCoroutine;
    public void ShakeCamera(float frequency, float smoothing, float length, bool interruptable = true)
    {
        if (shakeCoroutine != null && this.interruptable == false && interruptable == true) return;

        this.interruptable = interruptable;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(Shake(frequency, smoothing, length));
    }

    public void ShakeCamera(float frequency, bool interruptable = true)
    {
        if (shakeCoroutine != null && this.interruptable == false && interruptable == true) return;

        this.interruptable = interruptable;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(Shake(frequency, 0.5f, 0.2f));
    }

    IEnumerator Shake(float frequency, float smoothing, float length)
    {
        Vector3 originalCamPos = camera.localPosition;
        float t = 0;
        while(t < length)
        {
            Vector2 shakeVector = Random.insideUnitCircle * frequency;
            camera.localPosition = Vector3.Lerp(camera.localPosition, originalCamPos + new Vector3(shakeVector.x, shakeVector.y, 0), smoothing * Time.deltaTime * 15f);
            yield return null;
            t += Time.deltaTime;
        }
        camera.localPosition = originalCamPos;
        interruptable = true;
    }
}
