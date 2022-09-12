using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

    public string playerInput;

    float horizontal;
    float vertical;
    bool attack1;
    bool attack2;
    bool attack3;

    StateManager states;

    void Start()
    {
        states = GetComponent<StateManager>();
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal" + playerInput);
        vertical = Input.GetAxis("Vertical" + playerInput);
        attack1 = Input.GetButton("A" + playerInput);
        attack2 = Input.GetButton("B" + playerInput);
        attack3 = Input.GetButton("X" + playerInput);

        states.horizontal = horizontal;
        states.vertical = vertical;
        states.attack1 = attack1;
        states.attack2 = attack2;
        states.attack3 = attack3;
    }
	
}
