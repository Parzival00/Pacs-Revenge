using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost
{
    //Blinky's chase is the simplest being go to the player location so just uses base functionality of Chase()
    protected override void Chase()
    {
        base.Chase();
    }
}
