using System;
using UnityEngine;

namespace CocoDoogy.Animation
{
    public class TransportHandler : MonoBehaviour
    {
        private static readonly int VehicleType = Animator.StringToHash("VehicleType");
        private static readonly int Transport = Animator.StringToHash("Transport");
        
        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// 탈 것으로 이동시 애니메이션
        /// </summary>
        /// <param name="vehicleType">탈것 </param>
        public void ChangeVehicleType(VehicleType vehicleType)
        {
            anim.SetInteger(VehicleType, (int)vehicleType);
            anim.SetTrigger(Transport);
        }
        
        /// <summary>
        /// 이동후 탈것 애니메이션 초기화
        /// </summary>
        public void VehicleToIdle()
        {
            anim.SetInteger(VehicleType, 0);
            anim.SetTrigger(Transport);
        }
        
        
    }
}