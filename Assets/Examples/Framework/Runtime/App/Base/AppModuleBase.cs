using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppFramework
{
    public abstract class AppModuleBase : MonoBehaviour, IAppModule
    {
        public virtual void OnInit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        private void Update()
        {
        }
    }
}