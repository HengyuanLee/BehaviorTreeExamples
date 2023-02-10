

using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AppFramework
{
    public abstract class UIPanelControllerBase : IUIPanelController
    {
        public abstract UIPanelInfo panelInfo { get; }

        private bool isLoadCompleted;
        public UIShowState UIShowState { get; private set; }

        protected GameObject gameObject { get; private set; }
        private bool isOnLoading;

        public virtual void OnInit() { }
        public virtual void OnLoadCompeleted() { }
        public virtual void OnShow() { }
        public virtual void OnHide() { }
        public virtual void OnDestroy() { }
        public virtual void OnUpdate() { }

        public void Show()
        {
            UIShowState = UIShowState.Show;
            CheckUIState();
        }
        public void Hide()
        {
            UIShowState = UIShowState.Hide;
            CheckUIState();
        }
        public void Destroy()
        {
            UIShowState = UIShowState.Unload;
            isLoadCompleted = false;
            CheckUIState();
        }

        private void CheckUIState()
        {
            switch (UIShowState)
            {
                case UIShowState.Show:
                    if (gameObject == null || !gameObject)
                    {
                        if (!isOnLoading)
                        {
                            isOnLoading = true;
                            Addressables.LoadAssetAsync<GameObject>(panelInfo.prefabPath).Completed += (handle) =>
                            {
                                isOnLoading = false;
                                GameObject gameObject = GameObject.Instantiate(handle.Result, AppMain.Instance.UI.UIRoot);
                                Addressables.Release(handle.Result);
                                OnGameObjectLoadCompleted(gameObject);
                                CheckUIState();
                            };
                        }
                    }
                    else if (!gameObject.activeSelf)
                    {
                        gameObject.SetActive(true);
                        OnShow();
                    }
                    else {
                        OnShow();
                    }
                    break;
                case UIShowState.Hide:
                    if (gameObject != null && gameObject.activeSelf)
                    {
                        gameObject.SetActive(false);
                    }
                    OnHide();
                    break;
                case UIShowState.Unload:
                    if (gameObject)
                    {
                        GameObject.Destroy(gameObject);
                        gameObject = null;
                        OnDestroy();
                    }
                    break;
            }
        }
        private void OnGameObjectLoadCompleted(GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.CheckUIState();
            this.isLoadCompleted = true;
            OnLoadCompeleted();
        }
    }
}
