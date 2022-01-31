using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher 
{
    public class InputSystem : MonoBehaviour
    {
        public static PlayerInput Input
        {
            get
            {
                if (_Input == null)
                {
                    _Input = new PlayerInput();
                }
                return _Input;
            }

            set
            {
                _Input = value;
            }
        }
        private static PlayerInput _Input = null;
    }
}