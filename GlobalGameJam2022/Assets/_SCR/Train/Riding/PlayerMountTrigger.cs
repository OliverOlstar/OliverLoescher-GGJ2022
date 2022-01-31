using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMountTrigger : MonoBehaviour
{
    public Transform follow = null;
    private bool canEnter = true;

    private void OnTriggerEnter(Collider other) 
    {
        if (canEnter && other.tag == "Player")
        {
            PlayerManager player = other.GetComponentInParent<PlayerManager>();

            if (!player.riding.IsValid)
                return;

            player.riding.SetTarget(this);
            OnEnter();
        }
    }
    
    public virtual void OnMove(Vector2 pInput) { }
    public virtual void OnTrigger(bool pBool) { }
    public virtual void OnLook(Vector2 pDirection) { }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit() 
    {
        canEnter = false;
        Invoke(nameof(CanEnterTrue), 0.5f);
    }
    private void CanEnterTrue() => canEnter = true;
}
