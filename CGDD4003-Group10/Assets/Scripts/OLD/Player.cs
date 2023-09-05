using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerInput playerInput;
    public float moveMulti;
    Vector2 move; 

    void Update()
    {
        //get and use player movement
        move = playerInput.actions["Movement"].ReadValue<Vector2>() * moveMulti;
        transform.Translate(move.x * Time.deltaTime, 0, move.y * Time.deltaTime);
    }
}
