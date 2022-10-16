using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

//namespace MyInput
//{

    public class InputHandler : MonoBehaviour
    {

        public string playerInput;

        float horizontal;
        float vertical;

        bool block;

        bool[] attack = new bool[5];

        StateManager states;



        void Start()
        {
            states = GetComponent<StateManager>();

        }

        private void Update()
        {
            if (!attack[0])
            {
                // Read the input in Update so button presses aren't missed.
                attack[0] = CrossPlatformInputManager.GetButtonDown("A" + playerInput);
            }

            if (!attack[1])
            {
                // Read the input in Update so button presses aren't missed.
                attack[1] = CrossPlatformInputManager.GetButtonDown("B" + playerInput);
            }

            if (!attack[2])
            {
                // Read the input in Update so button presses aren't missed.
                attack[2] = Input.GetButtonDown("X" + playerInput);
            }

            if (!attack[3])
            {
                // Read the input in Update so button presses aren't missed.
                attack[3] = Input.GetButtonDown("Y" + playerInput);
            }

             if (!attack[4])
             {
            //headbutt?
            // Read the input in Update so button presses aren't missed.
            attack[4] = Input.GetButtonDown("LStickClick" + playerInput) || Input.GetButtonDown("RStickClick" + playerInput);
            attack[4] = Input.GetButtonDown("LStickClick" + playerInput) || Input.GetButtonDown("RStickClick" + playerInput);
             }


    }

        void FixedUpdate()
        {
            horizontal = Input.GetAxis("Horizontal" + playerInput);
            vertical = Input.GetAxis("Vertical" + playerInput);
            
            block = Input.GetButton("XBox LBumper" + playerInput) || Input.GetButton("XBox RBumper" + playerInput);

            states.horizontal = horizontal;
            states.vertical = vertical;

            states.block = block;

            states.attack1 = attack[0];
            states.attack2 = attack[1];
            states.attack3 = attack[2];
            states.attack4 = attack[3];
            states.attack5 = attack[4];

            attack[0] = false;
            attack[1] = false;
            attack[2] = false;
            attack[3] = false;
            attack[4] = false;
        }

    }

//}
