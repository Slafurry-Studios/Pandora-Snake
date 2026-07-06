using System.Collections;
using Slafurry.Core.Abstract;
using Slafurry.Core.Interface;
using UnityEngine;

namespace Game
{

    public class GameManager : GameSystem<GameManager>
    {
        private BulletManager bulletManager;
        public static BulletManager Bullet => Instance.bulletManager;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
        }

        public override IEnumerator Initialize() { yield return null; }
        public override void PostInitialize() { }


        public void RegisterBulletManager(BulletManager b) => bulletManager = b;
        public void UnregisterBulletManager(BulletManager b) { if (bulletManager == b) bulletManager = null; }


        public void ResetSession()
        {
            foreach (var resettable in FindObjectsOfType<MonoBehaviour>())
            {
                if (resettable is IResettable r) r.ResetState();
            }
        }
    }
}