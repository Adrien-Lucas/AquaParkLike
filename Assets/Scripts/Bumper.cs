using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to : Bumpers in character's prefab
/// Manages what happens with the collisions between players
/// </summary>
public class Bumper : MonoBehaviour
{
    enum BumperType
    {
        LeftBumper,
        RightBumper,
        BackBumper
    }

    [SerializeField] private BumperType type;
    [SerializeField] private Movements movementsComponent;

    [SerializeField]
    private float backBumpSpeedMultiplicator = 2;

    [SerializeField] private float backBumpDuration  = 1;
    //private bool colliding

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            switch (type)
            {
                case BumperType.BackBumper:
                    movementsComponent.ApplyTempMultiplicator(backBumpSpeedMultiplicator, backBumpDuration);
                    break;
                
                case BumperType.LeftBumper:
                    SideBumper(other.GetComponent<Movements>());
                    break;
                
                case BumperType.RightBumper:
                    SideBumper(other.GetComponent<Movements>());
                    break;
            }
        }
    }

    [SerializeField] private float sideBumpTime = 2f;
    [SerializeField] private float bumpDeviation = 0.4f;

    
    //Side bumpers moves the slowest character of the collision out of the bounds of the other
    void SideBumper(Movements other)
    {
        //The function is called on both actors of the collision, only the slowest is affected
        if (other.absDeviationAcceleration > movementsComponent.absDeviationAcceleration)
        {
            movementsComponent.ApplyTempDeviation((type == BumperType.LeftBumper ? -1f : 1f) * bumpDeviation, sideBumpTime);
        }
    }
}
