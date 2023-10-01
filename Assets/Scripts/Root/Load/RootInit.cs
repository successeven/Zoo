using System;
using Tools.Framework;
using UnityEngine;

namespace Logic
{
    public class RootInit : BaseMonoBehaviour
    {
        private const string ROOT_UNDESTR = "Core/RootDontDestroy";
        private static bool _inited;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            if (_inited)
                return;
            _inited = true;

            // create object
            GameObject undestrPrefab = Resources.Load<GameObject>(ROOT_UNDESTR);
            GameObject undestrObj = Instantiate(undestrPrefab);
            DontDestroyOnLoad(undestrObj);

            // fill with context
            RootUndestructable rootUndestructable = undestrObj.GetComponent<RootUndestructable>();
            rootUndestructable.SetCtx(new RootUndestructable.Ctx());
        }
    }
}