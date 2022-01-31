using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    [HideInInspector] public PlayerManager manager = null;
    [HideInInspector] public InputBridge input = null;

    public virtual void Init() { }
}
