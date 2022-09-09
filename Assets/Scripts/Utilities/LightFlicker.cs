using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

    Light lgt;

    public float maxInt = 8;
    public float minInt = 1.09f;

    float timer;

    public float maxTim = 3f;
    public float minTim = 1f;
    float targetTim;

    float targetIntensity;

    public float lerpSpeed = 3;

	void Start () {
        lgt = GetComponent<Light>();
        targetTim = minTim;
	}
	

	void Update () {

        timer += Time.deltaTime;

        if(timer > targetTim)
        {
            timer = 0;
            targetTim = Random.Range(minTim, maxTim);

            targetIntensity = maxInt;
            StartCoroutine("ResetIntensity");
        }

        lgt.intensity = Mathf.Lerp(lgt.intensity, targetIntensity, Time.deltaTime * lerpSpeed);
	}

    IEnumerator ResetIntensity()
    {
        yield return new WaitForSeconds(0.3f);
        targetIntensity = minInt; 
    }
}
