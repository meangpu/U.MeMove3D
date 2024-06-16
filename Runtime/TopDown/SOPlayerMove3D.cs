using UnityEngine;

namespace Meangpu.Move3D.TopDown
{
    [CreateAssetMenu(menuName = "Meangpu/Move/3D")]
    public class SOPlayerMove3D : ScriptableObject
    {
        public float MoveSpeed = 5f;

        public float DashSpeed = 20f;
        public float DashDuration = .15f;
        public float DashCooldown = .35f;

        public float RotateLerpSpeed = 20f;
        [Header("Change this to match player collider")]
        public float PlayerHeight = 2f;
        public float PlayerRadius = .7f;
    }
}
