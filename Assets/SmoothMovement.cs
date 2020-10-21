using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//Script for our continous movement system
//Teleport is disabled since it's a MP Game, and teleport is gay

public class SmoothMovement : MonoBehaviour
{
    //Variables
    //This is our speed variable, it needs to be able to be changed for different movement types
    public float speed = 1;
    //Controller or HMD
    public XRNode inputSource;
    //Our gravity, or players will just float in the air
    public float gravity = -9.81f;
    private float fallingSpeed;
    //Our ground layer var
    public LayerMask groundLayer;
    //Variable so our capsule sticks with the IRL HMD
    public float additionalHeight = 0.2f;
    //Our rig variable
    private XRRig rig;
    //axis input
    private Vector2 inputAxis;
    //our character variable
    private CharacterController character;



    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        character.Move(direction * Time.fixedDeltaTime * speed);

        //gravity system
        bool isGrounded = onGround();
        if (isGrounded)
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;

        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    //This function is so the ingame capsule will follow the players HMD in real life.
    void CapsuleFollowHeadset()
    {
      //  character.height = rig.cameraInRigSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }

    //check if we're on the ground
    bool onGround()
        {
            Vector3 rayStart = transform.TransformPoint(character.center);
            float rayLength = character.center.y + 0.01f;
            bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
            return hasHit;
        }

}
