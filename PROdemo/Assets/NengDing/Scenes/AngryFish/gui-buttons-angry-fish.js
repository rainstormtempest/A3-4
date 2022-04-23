#pragma strict

var AngryFish: GameObject;
	
	function OnGUI() {
	
		GUILayout.BeginHorizontal ("box");
		
		if (GUILayout.Button("Swim")){
		
		
		AngryFish.GetComponent.<Animation>().Play("swim1");
		}
		
		if (GUILayout.Button("Bite 1")){
		
		AngryFish.GetComponent.<Animation>().Play("bite1");
		}
		
		if (GUILayout.Button("Bite 2")){
		
		AngryFish.GetComponent.<Animation>().Play("bite2");
		}
		
		if (GUILayout.Button("Jump")){
		
		AngryFish.GetComponent.<Animation>().Play("jump");
		}
		
		if (GUILayout.Button("Flip")){
		
		AngryFish.GetComponent.<Animation>().Play("flip");
		}
		
		if (GUILayout.Button("Silly")){
		
		AngryFish.GetComponent.<Animation>().Play("silly");
		}
		
		GUILayout.EndHorizontal ();
	}
	

	