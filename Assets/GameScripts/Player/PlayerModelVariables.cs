using UnityEngine;

namespace Player
{
    public static class PlayerModelVariables
    {
        public static readonly int SwordEquippedBool = Animator.StringToHash("Sword Equipped");
        public static readonly int SwordAttackTrigger = Animator.StringToHash("Sword Attack");
        public static readonly int LaserAttackTrigger = Animator.StringToHash("Laser Attack");
        public static readonly int TurningBool = Animator.StringToHash("Turning");
        public static readonly int WalkingBool = Animator.StringToHash("Walking");


    }
}