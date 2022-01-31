using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
    public class Health : CharacterValue, IDamageable
    {
        [Header("Death")]
        [SerializeField] private bool disableCollidersOnDeath = true;
        private Collider[] colliders = new Collider[0];

        [Space]
        [ColorPalette("UI")] public Color deathColor = Color.grey;
        [ColorPalette("UI")] public Color damageColor = Color.red;
        [ColorPalette("UI")] public Color healColor = Color.green;

        [SerializeField] private bool ShakeOnDamage = false;
        [SerializeField] private EnemyMovement eMovement = null;
        [SerializeField] private CharacterMovement cMovement = null;

        protected override void Start() 
        {
            base.Start();

            if (disableCollidersOnDeath)
            {
                colliders = GetComponentsInChildren<Collider>();
            }

            onValueOut.AddListener(Death);

            Init();
        }

        protected virtual void Init() { }

        public virtual void Damage(float pValue, GameObject pSender, Vector3 pPoint, Vector3 pDirection, Color pColor)
        {
            Damage(pValue, pSender, pPoint, pDirection);
        }

        [Button()]
        public virtual void Damage(float pValue, GameObject pSender, Vector3 pPoint, Vector3 pDirection)
        {
            if (ShakeOnDamage)
                EZCameraShake.CameraShaker.Instance.ShakeOnce(11.0f * pValue, 8.0f * pValue, 0.1f, 0.15f);

            Modify(-pValue);
            
            if (value > 0 && pValue < 0)
            {
                if (eMovement != null)
                    eMovement.AddForce(pDirection.normalized * pValue * 100.0f);

                if (cMovement != null)
                    cMovement.AddForce(pDirection.normalized * pValue * 10.0f);
            }
        }

        public virtual void Death() 
        {
            if (disableCollidersOnDeath)
            {
                foreach (Collider c in colliders)
                {
                    c.enabled = false;
                }
            }
        }

        public GameObject GetGameObject() { return gameObject; }
        public IDamageable GetParentDamageable() { return this; }

        public void Respawn()
        {
            value = maxValue;
            foreach (BarValue bar in UIBars)
                bar.InitValue(1);
            isOut = false;
        }
    }
}