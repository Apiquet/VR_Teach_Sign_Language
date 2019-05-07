using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class NewBehaviourScript : MonoBehaviour
{
	
    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("I am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am aliveI am alive!");
		
	}

    // Update is called once per frame
    void Update()
    {
		Controller controller = new Controller();
		Frame frame = controller.Frame(); // controller is a Controller object
		List<Hand> hands = frame.Hands;
		if (frame.Hands.Count == 2) {
			Debug.Log(" first hand: " + hands[0].Direction.x);
			Debug.Log(" second hand: " + hands[1].Direction.x);

		}
		else if (frame.Hands.Count == 1) {
			List<Finger> fingers = hands[0].Fingers;
			Debug.Log(" first finger: " + fingers[0].Direction.x);
		}
	}
}
