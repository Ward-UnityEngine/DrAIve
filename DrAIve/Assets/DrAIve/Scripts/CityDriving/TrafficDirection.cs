using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionOption
{
    TwoWay = 0,
    OneWayOnlyIn = 1,
    OneWayOnlyOut = 2
}


public class TrafficDirection : MonoBehaviour
{
    [SerializeField] public DirectionOption direction;
}
