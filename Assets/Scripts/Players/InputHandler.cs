using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class InputHandler : MonoBehaviour {

    public string playerInput;

    float horizontal;
    float vertical;
    bool attack1;
    bool attack2;
    bool attack3;
    bool attack4;

    StateManager states;

    void Start()
    {
        states = GetComponent<StateManager>();
    }

    private void Update()
    {
        if (!attack1)
        {
            // Read the input in Update so button presses aren't missed.
            attack1 = CrossPlatformInputManager.GetButtonDown("A" + playerInput);
        }

        if (!attack2)
        {
            // Read the input in Update so button presses aren't missed.
            attack2 = CrossPlatformInputManager.GetButtonDown("B" + playerInput);
        }

        if (!attack3)
        {
            // Read the input in Update so button presses aren't missed.
            attack3 = CrossPlatformInputManager.GetButtonDown("X" + playerInput);
        }

        if (!attack4)
        {
            // Read the input in Update so button presses aren't missed.
            attack4 = CrossPlatformInputManager.GetButtonDown("Y" + playerInput);
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
        states.attack1 = attack1;
        states.attack2 = attack2;
        states.attack3 = attack3;
        states.attack4 = attack4;   

        attack1 = false;
        attack2 = false;
        attack3 = false;    
        attack4 = false;
    }
	
}
