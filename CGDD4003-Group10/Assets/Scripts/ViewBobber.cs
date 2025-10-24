using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBobber : MonoBehaviour
{
    [Header("View Bobbing Settings")]
    [SerializeField] float walkBobSpeed = 14f;
    [SerializeField] float bobAmount = 0.05f;
    [SerializeField] PlayerController pc;
    [SerializeField] AudioClip footstep;
    [SerializeField] AudioSource feet;
    [SerializeField] float footstepThreshold;

    float defaultYpos = 0;
    float timer = 0;
    bool stepped = false;

    // Start is called before the first frame update
    void Start()
    {
        defaultYpos = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.canMove == true)
        {
            //Vector3 pos = transform.localPosition;

            if (Mathf.Abs(pc.velocity.x) > 0.1f || Mathf.Abs(pc.velocity.z) > 0.1f)
            {
                //player is walking
                timer += Time.deltaTime * walkBobSpeed;
                //timer += Time.deltaTime * walkBobSpeed * Mathf.PI * 2f;
                //if (timer > Mathf.PI * 2f) timer -= Mathf.PI * 2f;
                float sinedTimer = Mathf.Sin(timer);
                //float offset = Mathf.Sin(timer) * bobAmount;

                //pos.y = defaultYpos + offset;

                if (PlayerPrefs.GetInt("HeadBob") == 1)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, defaultYpos + sinedTimer * bobAmount, transform.localPosition.z);
                    //transform.localPosition = pos;
                } else
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultYpos, Time.deltaTime * walkBobSpeed), transform.localPosition.z);
                }

                /*if (Mathf.Sin(timer - Time.deltaTime * walkBobSpeed * Mathf.PI * 2f) >= footstepThreshold &&
                    Mathf.Sin(timer) < footstepThreshold)
                {
                    feet.PlayOneShot(footstep);
                }*/
                if (sinedTimer < footstepThreshold && !stepped)
                {
                    feet.PlayOneShot(footstep);
                    stepped = true;
                }
                else if (sinedTimer >= footstepThreshold && stepped)
                {
                    stepped = false;
                }
            }
            else
            {
                //player is idle
                timer = 0;
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultYpos, Time.deltaTime * walkBobSpeed), transform.localPosition.z);
            }
        }
    }
}
