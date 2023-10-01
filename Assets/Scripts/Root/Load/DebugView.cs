using System.Collections.Generic;
using System.Linq;
using Containers;
using TMPro;
using Tools.Extensions.Reactive;
using Tools.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class DebugView : BaseMonoBehaviour
    {
        public struct Ctx
        {
            public ReactiveEvent<LoadSt> reloadEvent;
        }

        [SerializeField] private GameObject _holder;
        [SerializeField] private Button _reloadButton;

        [SerializeField] private Toggle _generalToggle;

        private Ctx _ctx;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            // offline reload
            _reloadButton.OnClickAsObservable().Subscribe(_ =>
            {
                LoadSt loadSt = GetBaseValues();
                _ctx.reloadEvent.Notify(loadSt);
            }).AddTo(this);

            _generalToggle.OnValueChangedAsObservable().Subscribe(toggled => _holder.SetActive(toggled)).AddTo(this);
            _generalToggle.isOn = true;
            _ctx.reloadEvent.SubscribeWithSkip(loadSt => { _generalToggle.isOn = false; }).AddTo(this);
        }

        private LoadSt GetBaseValues()
        {
            return new LoadSt
            {
            };
        }
    }
}