// SeekSteer.cs
// Written by Matthew Hughes
// 19 April 2009
// Uploaded to Unify Community Wiki on 19 April 2009
// URL: http://www.unifycommunity.com/wiki/index.php?title=SeekSteer
 
//remade in JavaScript by Pier Shaw 
// URL: www.firecg.com 
//,,see my other sites http://www.industrialclubs.com/
 
    var waypoints : Transform[];
    var waypointRadius : float  = 1.5;
    var damping : float = 0.1;
    var loop : boolean = false;
    var speed : float = 2.0;
    var faceHeading : boolean = true;
 
    private var targetHeading : Vector3;
    private var currentHeading : Vector3;
    private var targetwaypoint : int;
    private var xform : Transform;
    private var useRigidbody : boolean;
    private var rigidmember : Rigidbody;
 
 
    // Use this for initialization
   function Start() {
        xform = transform;
        currentHeading = xform.forward;
        if(waypoints.Length<=0)
        {
            Debug.Log("No waypoints on "+name);
            enabled = false;
        }
        targetwaypoint = 0;
        if(rigidbody!=null)
        {
            useRigidbody = true;
            rigidmember = rigidbody;
        }
        else
        {
            useRigidbody = false;
        }
    }
 
 
    // calculates a new heading
    function FixedUpdate() {
        targetHeading = waypoints[targetwaypoint].position - xform.position;
 
        currentHeading = Vector3.Lerp(currentHeading,targetHeading,damping*Time.deltaTime);
    }
 
    // moves us along current heading
    function Update(){
 
        if(useRigidbody)
            rigidmember.velocity = currentHeading * speed;
        else
            xform.position +=currentHeading * Time.deltaTime * speed;
        if(faceHeading)
            xform.LookAt(xform.position+currentHeading);
 
        if(Vector3.Distance(xform.position,waypoints[targetwaypoint].position)<=waypointRadius)
        {
            targetwaypoint++;
            if(targetwaypoint>=waypoints.Length)
            {
                targetwaypoint = 0;
                if(!loop)
                    enabled = false;
            }
        }
    }
 
 
    // draws red line from waypoint to waypoint
    function OnDrawGizmos(){
 
        Gizmos.color = Color.red;
        for(var i : int = 0; i< waypoints.Length;i++)
        {
           var pos : Vector3 = waypoints[i].position;
            if(i>0)
            {
                var prev : Vector3 = waypoints[i-1].position;
                Gizmos.DrawLine(prev,pos);
            }
        }
    }