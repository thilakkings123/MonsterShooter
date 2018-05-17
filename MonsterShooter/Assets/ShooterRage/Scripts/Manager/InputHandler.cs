using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class InputHandler : MonoBehaviour {

    private PlayerController _controller;

	// Use this for initialization
	void Start ()
    {
        _controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_controller == null)
        {
            _controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            return;
        }
        else
        {
            _controller.BtnMove(CrossPlatformInputManager.GetAxis("Horizontal"));

            if (CrossPlatformInputManager.GetButtonDown("Fire1"))
                _controller.Firing(true);
            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
                _controller.Firing(false);

            if (CrossPlatformInputManager.GetButtonDown("Jump"))
                _controller.JumpBtn();
            if (CrossPlatformInputManager.GetButtonUp("Jump"))
                _controller.StopJump();
        }
    }
}
