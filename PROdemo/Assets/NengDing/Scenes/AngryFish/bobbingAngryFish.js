#pragma strict

var pos1 : Vector3;
var pos2 : Vector3;
var offset : Vector3;
var moveSpeed : float = 0.1;
var moveTo;

function Start(){

    offset = Vector3.down;
    pos1 = transform.position;
    pos2 = transform.position + offset; 
}

function Update(){

    //switch
    if(transform.position == pos1){
    moveTo = pos2;   
    }
     
    if(transform.position == pos2){
    moveTo = pos1;
    }
	//animate
	transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed);

}