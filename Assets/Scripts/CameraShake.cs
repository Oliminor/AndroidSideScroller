using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    bool isShaking = false;

    Vector3 defaultPos;

    void Awake()
    {
        instance = this;
        defaultPos = transform.position;
    }

    public void TriggerShake(float _shakeStrength, float _shakeTime, float _shakeTimeBetweenTwoShakes)
    {
        StartCoroutine(ShakeTimer(_shakeTime));
        StartCoroutine(ShakeCamera(_shakeStrength, _shakeTimeBetweenTwoShakes));
    }

    IEnumerator ShakeCamera(float shakeStrength, float shakeTimeBetweenTwoShakes)
    {
        while (isShaking)
        {
            float shakeX = Random.Range(-shakeStrength, shakeStrength);
            float shakeY = Random.Range(-shakeStrength, shakeStrength);
            float shakeZ = Random.Range(-shakeStrength, shakeStrength);

            transform.position += new Vector3(shakeX, shakeY, shakeZ);

            yield return new WaitForSeconds(shakeTimeBetweenTwoShakes);

            transform.position = defaultPos;
        }
    }

    IEnumerator ShakeTimer(float shakeTime)
    {
        isShaking = true;
        yield return new WaitForSeconds(shakeTime);
        isShaking = false;
    }
}
