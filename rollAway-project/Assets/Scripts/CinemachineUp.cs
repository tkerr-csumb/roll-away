// using UnityEngine;
// using Cinemachine;

// public class CinemachineGravityUp : MonoBehaviour
// {
//     public Transform player;

//     private GravityControl gravityControl;
//     private CinemachineBrain brain;

//     void Awake() {
//         brain = GetComponent<CinemachineBrain>();
//         gravityControl = player.GetComponent<GravityControl>();
//     }

//     void LateUpdate() {
//         Vector3 gravityDir = gravityControl.GetGravityDirection().normalized;

//         brain.m_WorldUpOverride = -gravityDir;
//     }
// }