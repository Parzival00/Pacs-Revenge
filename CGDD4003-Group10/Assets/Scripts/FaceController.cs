using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceController : MonoBehaviour
{
    [Header("Damage 1 Faces")]
    [SerializeField] Sprite mouthOpenFace_d1;
    [SerializeField] Sprite mouthClosedFace_d1;
    [SerializeField] Sprite happyFace_d1;

    [Header("Damage 2 Faces")]
    [SerializeField] Sprite mouthOpenFace_d2;
    [SerializeField] Sprite mouthClosedFace_d2;
    [SerializeField] Sprite happyFace_d2;

    [Header("Damage 3 Faces")]
    [SerializeField] Sprite mouthOpenFace_d3;
    [SerializeField] Sprite mouthClosedFace_d3;
    [SerializeField] Sprite happyFace_d3;

    [Header("Death Face")]
    [SerializeField] Sprite deathFace;

    [Header("Settings")]
    [SerializeField] float mouthClosedLength = 0.5f;
    [SerializeField] float happyFaceLength = 1f;

    Image pacManFace;

    bool mouthClosed;
    bool happyFace;
    bool fruitEaten;

    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        pacManFace = GetComponent<Image>();

        switch (PlayerController.playerLives)
        {
            case 3:
                pacManFace.sprite = mouthOpenFace_d1;
                break;
            case 2:
                pacManFace.sprite = mouthOpenFace_d2;
                break;
            case 1:
                pacManFace.sprite = mouthOpenFace_d3;
                break;
        }

        dead = false;
        mouthClosed = false;
        happyFace = false;
        fruitEaten = false;
    }

    public void EatPellet()
    {
        if(!happyFace && !dead && !fruitEaten)
            StartCoroutine(CloseMouth());
    }

    public void RailgunPickup()
    {
        StartCoroutine(HappyFace());
    }

    public void EatFruit()
    {
        StartCoroutine(FruitEaten());
    }

    public void Die()
    {
        pacManFace.sprite = deathFace;
        dead = true;
    }

    public void Respawn()
    {
        switch (PlayerController.playerLives)
        {
            case 3:
                pacManFace.sprite = mouthOpenFace_d1;
                break;
            case 2:
                pacManFace.sprite = mouthOpenFace_d2;
                break;
            case 1:
                pacManFace.sprite = mouthOpenFace_d3;
                break;
        }

        dead = false;
    }

    IEnumerator CloseMouth()
    {
        if (!dead)
        {
            yield return new WaitUntil(() => (!happyFace && !mouthClosed && !fruitEaten) || dead);

            if (!dead)
            {
                mouthClosed = true;
                switch (PlayerController.playerLives)
                {
                    case 3:
                        pacManFace.sprite = mouthClosedFace_d1;
                        break;
                    case 2:
                        pacManFace.sprite = mouthClosedFace_d2;
                        break;
                    case 1:
                        pacManFace.sprite = mouthClosedFace_d3;
                        break;
                    default:
                        pacManFace.sprite = mouthClosedFace_d1;
                        break;

                }
            }

            yield return new WaitForSeconds(mouthClosedLength);

            mouthClosed = false;

            if (!dead)
            {
                switch (PlayerController.playerLives)
                {
                    case 3:
                        pacManFace.sprite = mouthOpenFace_d1;
                        break;
                    case 2:
                        pacManFace.sprite = mouthOpenFace_d2;
                        break;
                    case 1:
                        pacManFace.sprite = mouthOpenFace_d3;
                        break;
                    default:
                        pacManFace.sprite = mouthOpenFace_d1;
                        break;
                }
            }
        }
    }


    IEnumerator HappyFace()
    {
        if (!dead)
        {
            yield return new WaitUntil(() => (!mouthClosed && !happyFace && !fruitEaten) || dead);

            if (!dead)
            {
                happyFace = true;
                switch (PlayerController.playerLives)
                {
                    case 3:
                        pacManFace.sprite = happyFace_d1;
                        break;
                    case 2:
                        pacManFace.sprite = happyFace_d2;
                        break;
                    case 1:
                        pacManFace.sprite = happyFace_d3;
                        break;
                    default:
                        pacManFace.sprite = happyFace_d1;
                        break;
                }
            }

            yield return new WaitForSeconds(happyFaceLength);

            happyFace = false;

            if (!dead)
            {
                switch (PlayerController.playerLives)
                {
                    case 3:
                        pacManFace.sprite = mouthOpenFace_d1;
                        break;
                    case 2:
                        pacManFace.sprite = mouthOpenFace_d2;
                        break;
                    case 1:
                        pacManFace.sprite = mouthOpenFace_d3;
                        break;
                    default:
                        pacManFace.sprite = mouthOpenFace_d1;
                        break;
                }
            }
        }
    }

    IEnumerator FruitEaten()
    {
        if (!dead)
        {
            yield return new WaitUntil(() => (!mouthClosed && !happyFace && !fruitEaten) || dead);

            if (!dead)
            {
                fruitEaten = true;

                switch (PlayerController.playerLives)
                {
                    case 3:
                        pacManFace.sprite = mouthClosedFace_d1;
                        break;
                    case 2:
                        pacManFace.sprite = mouthClosedFace_d2;
                        break;
                    case 1:
                        pacManFace.sprite = mouthClosedFace_d3;
                        break;
                    default:
                        pacManFace.sprite = mouthClosedFace_d1;
                        break;
                }
            }

            yield return new WaitForSeconds(mouthClosedLength);

            if (!dead)
            {
                switch (PlayerController.playerLives)
                {
                    case 3:
                        pacManFace.sprite = happyFace_d1;
                        break;
                    case 2:
                        pacManFace.sprite = happyFace_d2;
                        break;
                    case 1:
                        pacManFace.sprite = happyFace_d3;
                        break;
                    default:
                        pacManFace.sprite = happyFace_d1;
                        break;
                }
            }

            yield return new WaitForSeconds(happyFaceLength);

            fruitEaten = false;

            if (!dead)
            {
                switch (PlayerController.playerLives)
                {
                    case 3:
                        pacManFace.sprite = mouthOpenFace_d1;
                        break;
                    case 2:
                        pacManFace.sprite = mouthOpenFace_d2;
                        break;
                    case 1:
                        pacManFace.sprite = mouthOpenFace_d3;
                        break;
                    default:
                        pacManFace.sprite = mouthOpenFace_d1;
                        break;
                }
            }
        }
    }
}
