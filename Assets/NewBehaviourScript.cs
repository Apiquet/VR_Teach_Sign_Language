using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.IO;

public class NewBehaviourScript : MonoBehaviour
{
	Sign refer_sign = new Sign();
	List<Sign> player_signs = new List<Sign>();
	Comparator comparator = new Comparator();
	int score = 0;
	// Start is called before the first frame update
	void Start()
    {
		Debug.Log("Starting script");	
	}

	void save_sign(List<Hand> hands)
	{
		Debug.Log("Saving gesture");
		refer_sign = new Sign(hands[0]);

		/*Data _data = new Data() {
			gesture = {
				finger0 = {
					x = hands[0].Fingers[0].Direction.x,
					y = hands[0].Fingers[0].Direction.y,
					z = hands[0].Fingers[0].Direction.z
				},
				finger1 = {
					x = hands[0].Fingers[1].Direction.x,
					y = hands[0].Fingers[1].Direction.y,
					z = hands[0].Fingers[1].Direction.z
				},
				finger2 = {
					x = hands[0].Fingers[2].Direction.x,
					y = hands[0].Fingers[2].Direction.y,
					z = hands[0].Fingers[2].Direction.z
				},
				finger3 = {
					x = hands[0].Fingers[3].Direction.x,
					y = hands[0].Fingers[3].Direction.y,
					z = hands[0].Fingers[3].Direction.z
				},
				finger4 = {
					x = hands[0].Fingers[4].Direction.x,
					y = hands[0].Fingers[4].Direction.y,
					z = hands[0].Fingers[4].Direction.z
				}
			}
		};
		string dataAsJson = JsonUtility.ToJson(_data);

		File.WriteAllText("sign.txt", dataAsJson);*/

	}
	void load_sign( )
	{

	}
	void compare_sign(List<Hand> hands)
	{
		Debug.Log("Compare gesture");
		player_signs.Add(new Sign(hands[0]));
		float score = comparator.compare(refer_sign, player_signs[player_signs.Count - 1]);

		Debug.Log("Score: " + score);
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

		} else if (frame.Hands.Count == 1) {
			List<Finger> fingers = hands[0].Fingers;
			if (Input.GetKeyDown("s")) {
				save_sign(hands);
			}
			else if (Input.GetKeyDown("space")) {
				compare_sign(hands);
			}
		}
	}
}
public class Data {
	public Gesture gesture { get; set; }
}
public class Gesture {
	public Finger_dir finger0 { get; set; }
	public Finger_dir finger1 { get; set; }
	public Finger_dir finger2 { get; set; }
	public Finger_dir finger3 { get; set; }
	public Finger_dir finger4 { get; set; }
}
public class Finger_dir {
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
}

public class Sign {

	public Hand hand = new Hand();
	public Sign(Hand refer_hand)
	{
		hand = refer_hand;
	}
	public Sign()
	{

	}
}

public class Comparator {

	Sign refer_sign = new Sign();
	Sign player_sign = new Sign();

	public Comparator()
	{
		
	}
	public float compare(Sign refer_sign, Sign player_sign)
	{
		float score = 0;
		for(int i = 0; i < refer_sign.hand.Fingers.Count; i++) {
			score += compare_finger(refer_sign.hand.Fingers[i], player_sign.hand.Fingers[i]);
		}
		return score/ refer_sign.hand.Fingers.Count;

	}
	public float compare_finger(Finger refer_finger, Finger player_finger)
	{
		return (float) 100 * (refer_finger.Direction.x * player_finger.Direction.x + refer_finger.Direction.y * player_finger.Direction.y + refer_finger.Direction.z * player_finger.Direction.z);
	}
}
