using System;
using Cinemachine;
using UnityEngine;

namespace GameCore
{
    public class CameraSwitchOnGameEvent : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _prepareVM;
        [SerializeField] private CinemachineVirtualCamera _battleVM;

        public enum Vm
        {
            Prepare,
            Battle
        }

        private void Start()
        {
            GameManager.Instance.RoundStartEvent.AddListener(()=>Change(Vm.Prepare));
            GameManager.Instance.WarSimStartEvent.AddListener(()=>Change(Vm.Battle));
        }

        public void Change(Vm vm)
        {
            switch (vm)
            {
                case Vm.Prepare:
                    _prepareVM.Priority = 20;
                    break;
                case Vm.Battle:
                    _prepareVM.Priority = 5;
                    break;
            }
        }
    }
    
}