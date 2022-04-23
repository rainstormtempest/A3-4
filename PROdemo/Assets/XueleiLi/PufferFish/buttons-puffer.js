#pragma strict

var myCharacter1: GameObject;
var myCharacter2: GameObject;




function Start ()
{
    myCharacter2.SetActive(false); //hide puffed guy on start
    
}
	
	function OnGUI() {
	
		GUILayout.BeginHorizontal ("box");
		
		if (GUILayout.Button("Normal")){
		myCharacter2.SetActive(false); //hide
		myCharacter1.SetActive(true); //show
		myCharacter1.GetComponent.<Animation>().CrossFade("swimming");	
		
		
		}
		
		if (GUILayout.Button("Puffed")){
		myCharacter1.SetActive(false); //hide
		myCharacter2.SetActive(true); //show
		myCharacter2.GetComponent.<Animation>().CrossFade("swimmingHuge");
		
		
		}
		
		if (GUILayout.Button("Biting")){
		myCharacter1.GetComponent.<Animation>().CrossFade("biting");	
		myCharacter2.GetComponent.<Animation>().CrossFade("bitingHuge");	
		
		}
		

		
		
		
	
		GUILayout.EndHorizontal ();
	}
