using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class InputHandler : MonoBehaviour {

    public string playerInput;

    float horizontal;
    float vertical;
    bool[] attack = new bool [4];

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
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal" + playerInput);
        vertical = Input.GetAxis("Vertical" + playerInput);
        //attack1 = CrossPlatformInputManager.GetButtonDown("A" + playerInput);
        //attack2 = CrossPlatformInputManager.GetButtonDown("B" + playerInput);
        //attack3 = CrossPlatformInputManager.GetButton("X" + playerInput);
        //attack4 = CrossPlatformInputManager.GetButton("Y" + playerInput);

        states.horizontal = horizontal;
        states.vertical = vertical;
        states.attack1 = attack[0];
        states.attack2 = attack[1];
        states.attack3 = attack[2];
        states.attack4 = attack[3];

        attack[0] = false;
        attack[1] = false;
        attack[2] = false;
        attack[3] = false;
    }
	
}
