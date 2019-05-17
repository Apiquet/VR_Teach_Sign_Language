using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.IO;
using System;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
	Sign refer_sign = new Sign();
	List<Sign> player_signs = new List<Sign>();
	Comparator comparator = new Comparator();

	public InputField sign_name_to_save;
	float score = 0;
	int letter_index = 0;
	List<string> alphabet = new List<string>();
	int serie_size = 5;
	List<string> serie = new List<string>();
	public Text text_score;
	public Text text_sign;
	public Text mode;
	public Text sign_number;
	public Text msg;
    int frame_number = 0;
    int won_count = 0;
    Texture2D myTexture;
	bool train_mode = true;
	bool won_bool = false;
	bool serie_bool = false;
	bool waitActive = false;
	static string images_dir = "Images/";
    static string images_train_dir = images_dir + "train/";
	static string images_test_dir = images_dir + "test/";
	static string images_win_dir = images_dir + "win/";
    // Start is called before the first frame update
    void Start()
	{
		Debug.Log("Starting script");
		Debug.Log("Sign to learn:");
		load_sign_names();
		Debug.Log(String.Join(", ", alphabet));
        //creating a serie of 5 random images to train the user on batch of 5 images (batch size defined by a global variable)
		create_serie();

		text_sign.text = "Current sign: " + serie[letter_index].ToString();
		mode.text = "Mode: Train (SPACE to switch to test mode)";
		sign_number.text = "Sign " + (letter_index + 1).ToString() + "/" + serie_size.ToString();
        // displaying the good image (image of the sign) within the scene
		display_image();
	}
	void create_serie()
	{
        //creating a serie of 5 random images picked from our dataset
		serie = new List<string>();
		System.Random rnd = new System.Random();

		List<int> numbers_chosen = new List<int>();
		int number = -1;
		for (int i = 0; i < serie_size; i++) {
            //while loop to be sur we don't pick twice the same image
			do {
				number = rnd.Next(1, alphabet.Count);
			} while (numbers_chosen.Contains(number));
			serie.Add(alphabet[number]);
			numbers_chosen.Add(number);
		}
		Debug.Log(String.Join(", ", alphabet));
	}
	void save_sign(Hand hand,string name)
	{
        // function to save a new sign (function call when there is a hand visible and S key pressed 
		Debug.Log("Saving gesture");
		Gesture gesture = new Gesture(hand, name);
		string[] json = File.ReadAllLines("sign.txt");
		string linetoadd = gesture.save_to_json();
		string result = string.Empty;
		for (int i = 0; i < json.Length-1; i++) {
			result += json[i] + "\n";
		}
		if (json.Length > 3) result += ",";
		result += linetoadd + "\n";
		result += json[json.Length-1] + "\n";
        // saving the sign into a txt file, the data is in json format
		File.WriteAllText("sign.txt", result);
	}
	void load_sign_names()
	{
        // loading all the sign names available in our dataset
		string[] txt_json = File.ReadAllLines("sign.txt");
		for (int i = 2; i < txt_json.Length-1; i++) {
			string json = txt_json[i];
			//Debug.Log("json :" + json);
			string[] char_ = new string[] { " ", "\"", "[", "]" };
			for (int j = 0; j < char_.Length; j++) {
				json = json.Replace(char_[j], string.Empty);
			}

			string[] main_json = json.Split(new char[] { '{', '}', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (main_json[0].Length > 0) alphabet.Add(main_json[0]);
			else Debug.Log("pas if");
		}
		
	}
	void load_sign(string name)
	{
        // loading a specific sign with all the fingers' direction
		float[,] fingers_directions = new float[5, 3];
		string[] txt_json = File.ReadAllLines("sign.txt");
		int wish_index = -1;
		for (int i = 0; i < txt_json.Length; i++) {
			if (txt_json[i].Contains(name)) { wish_index = i; break; }

		}
		string json = txt_json[wish_index];
		Debug.Log("json :" + json);
		string[] char_ = new string[] { " ", "\"", "[", "]" };
		for (int i = 0; i < char_.Length; i++) {
			json = json.Replace(char_[i], string.Empty);
		}

		string[] main_json = json.Split(new char[]  { '{' , '}', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
		
		for (int i=0; i < main_json.Length; i++) {
			//Debug.Log("Main json " + i + " :" + main_json[i]);
		}
		refer_sign = new Sign(main_json);
		 

	}
	float compare_sign(Hand hand)
	{
        // comparing the sign of the user (in the parameter) with a reference sign (the one he has to do)
		Debug.Log("Compare gesture");
		player_signs.Add(new Sign(hand));
		score = comparator.compare(refer_sign, player_signs[player_signs.Count - 1]);

		Debug.Log("Score: " + score);
		text_score.text = "Score: " + score.ToString();

		return score;
	}
	void display_image()
	{
        //Displaying the good image to the user: in train mode we display the sign and the meaning, in test mode we only display the meaning
		string dir = "";
		if (train_mode) dir = images_train_dir;
		else dir = images_test_dir;

		myTexture = Resources.Load(dir + serie[letter_index]) as Texture2D;
		GameObject rawImage = GameObject.Find("RawImage");
		rawImage.GetComponent<RawImage>().texture = myTexture;
	}
	void letter_won()
	{
        // function to display an animation when the sign of the user got a succes
		won_bool = true;
		text_sign.text = "Current sign: " + serie[letter_index].ToString();
		sign_number.text = "Sign " + (letter_index + 1).ToString() + "/" + serie_size.ToString();
		text_score.color = Color.green;

		myTexture = Resources.Load(images_win_dir + "good_job") as Texture2D;
		GameObject rawImage = GameObject.Find("RawImage");
		rawImage.GetComponent<RawImage>().texture = myTexture;

	}
	void serie_won()
	{
        // displaying an animation when the user completed a serie 
		serie_bool = true;
		text_sign.text = "You won the serie";
		sign_number.text = "Press SPACE to start a new serie";
		text_score.text = "";

		myTexture = Resources.Load(images_win_dir + "win") as Texture2D;
		GameObject rawImage = GameObject.Find("RawImage");
		rawImage.GetComponent<RawImage>().texture = myTexture;

	}
	void play_game(Hand hand){
		if (score > 80) {
			letter_won();
			if (letter_index == serie_size - 1 && !train_mode) serie_won();
			if (letter_index == serie_size - 1 && train_mode) {
				msg.text = "You seems to be ready for the test mode! Press SPACE to switch!";
			}
		}
		if (frame_number / 50 >= 1) {
			Debug.Log("Sign you must do: " + serie[letter_index]);
			load_sign(serie[letter_index]);
			score = compare_sign(hand);
			frame_number = 0;
		}
		if (Input.GetKeyDown("s")) {
			save_sign(hand, "A");
		}
	}
	// Update is called once per frame
	void Update()
    {
		if (!serie_bool) {
			if (!won_bool) {
				Controller controller = new Controller();
				Frame frame = controller.Frame(); // controller is a Controller object
				List<Hand> hands = frame.Hands;
				Hand hand;
				frame_number++;
				if (frame.Hands.Count == 2) {
					play_game(hands[1]);
				} else if (frame.Hands.Count == 1) {
					play_game(hands[0]);
					
				}
				if (Input.GetKeyDown("space")) {
					if (train_mode) {
						Debug.Log("You switched to Test mode");
						mode.text = "Mode: Test (SPACE to switch to train mode)";
						msg.text = "";
						train_mode = false;
						letter_index = 0;
						text_sign.text = "Current sign: " + serie[letter_index].ToString();
						sign_number.text = "Sign " + (letter_index + 1).ToString() + "/" + serie_size.ToString();
						display_image();
					} else {
						Debug.Log("You switched to Train mode");
						mode.text = "Mode: Train (SPACE to switch to test mode)";
						text_sign.text = "Current sign: " + serie[letter_index].ToString();
						sign_number.text = "Sign " + (letter_index + 1).ToString() + "/" + serie_size.ToString();
						train_mode = true;
						display_image();
					}
				}
			} else {
                won_count++;
                if (won_count / 200 >= 1)
                {
                    won_count = 0;
                    letter_index++;
                    score = 0;
                    if (letter_index >= serie_size) letter_index = 0;
                    text_sign.text = "Current sign: " + serie[letter_index].ToString();
                    sign_number.text = "Sign " + (letter_index + 1).ToString() + "/" + serie_size.ToString();
                    display_image();
                    text_score.color = Color.black;
                    won_bool = false;
                }
			}
		}else {
			if (Input.GetKeyDown("space")) {
				serie_bool = false;
				train_mode = true;
				create_serie();
			}
		}
	}
}

public class Gesture {
	public float[,] fingers_directions = new float[5, 3];
	public string name;
	public Gesture(){ }
	public Gesture(Hand hand, string name)
	{
		float[,] fingers_directions = new float[5, 3];
		this.name = name;
		for (int i = 0; i < hand.Fingers.Count; i++) {
			this.fingers_directions[i, 0] = hand.Fingers[i].Direction.x;
			this.fingers_directions[i, 1] = hand.Fingers[i].Direction.y;
			this.fingers_directions[i, 2] = hand.Fingers[i].Direction.z;
		}
	}
	public string save_to_json()
	{
		string gesture = "\""+this.name+"\": [{";
		for (int i = 0; i < 5; i++) {
			gesture += "\"finger" + i+ "\":[{\"x\": " + this.fingers_directions[i, 0] + ",\"y\": " + this.fingers_directions[i, 1] + ",\"z\": " + this.fingers_directions[i, 2] + "}]";
			if (i != 4) gesture += ",";
		}
		gesture += "}]";
		return gesture;
	}
}
public class Sign {

	public float[,] sign = new float[5, 3];

	public Sign(Hand refer_hand)
	{
		for (int i = 0; i < refer_hand.Fingers.Count; i++) {
			this.sign[i, 0] = refer_hand.Fingers[i].Direction.x;
			this.sign[i, 1] = refer_hand.Fingers[i].Direction.y;
			this.sign[i, 2] = refer_hand.Fingers[i].Direction.z;
		}
	}
	public Sign(string[] main_json)
	{
		int n = 0;

		for (int i = 3; i < main_json.Length; i = i + 7) {
			this.sign[n, 0] = float.Parse(main_json[i]);
			this.sign[n, 1] = float.Parse(main_json[i + 2]);
			this.sign[n, 2] = float.Parse(main_json[i + 4]);

			n++;
		}
	}

	public Sign() { }
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
		for (int i = 0; i < 5; i++) {
			score += compare_finger(refer_sign.sign[i, 0], refer_sign.sign[i, 1], refer_sign.sign[i, 2], player_sign.sign[i, 0], player_sign.sign[i, 1], player_sign.sign[i, 2]);
		}
		if (score < 0) return 0;
		return score / 5;

	}
	public float compare_finger(float refer_finger_x, float refer_finger_y, float refer_finger_z, float player_finger_x, float player_finger_y, float player_finger_z)
	{

		return (float)100 * (refer_finger_x * player_finger_x + refer_finger_y * player_finger_y + refer_finger_z * player_finger_z);
	}
}

