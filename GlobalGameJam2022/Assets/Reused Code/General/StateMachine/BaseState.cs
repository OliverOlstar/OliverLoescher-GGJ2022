using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher 
{
    public class BaseState : MonoBehaviour
    {
        protected StateMachine machine; 

        public virtual void Init(StateMachine pMachine) { machine = pMachine; }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual bool CanEnter() { return true; }
        public virtual bool CanExit() { return true; }

        public virtual void OnFixedUpdate() { }
        public virtual void OnUpdate() { }
    }
}