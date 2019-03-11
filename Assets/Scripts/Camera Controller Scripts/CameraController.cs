
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class CameraController : MonoBehaviour{



	public static CameraController instance;

	public bool useOrbitInTPSCameraMode;
	// Currently rendering?
	public bool isRendering = true;

	// The target we are following transform and rigidbody.
	public GameObject playerCar;
	private Rigidbody playerRigid;
	private float playerSpeed = 0f;
	private Vector3 playerVelocity = new Vector3 (0f, 0f, 0f);

	public Camera thisCam;			
	public GameObject pivot;		// Pivot center of the camera. Used for making offsets and collision movements.

	// Camera Modes.

					// Shall we use camera occlusion?
	public LayerMask occlusionLayerMask = -1;

	public bool useAutoChangeCamera = false;			// Shall we change camera mode by auto?
	private float autoChangeCameraTimer = 0f;

	public Vector3 topCameraAngle = new Vector3(45f, 45f, 0f);		// If so, we will use this Vector3 angle for top camera mode.

	private float distanceOffset = 0f;	
	public float maximumZDistanceOffset = 10f;		// Distance offset for top camera mode. Related with vehicle speed. If vehicle speed is higher, camera will move to front of the vehicle.
	public float topCameraDistance = 100f;				// Top camera height / distance.

	// Used for smooth camera movements. Smooth camera movements are only used for TPS and Top camera mode.
	private Vector3 targetPosition, lastFollowerPosition = Vector3.zero;
	private Vector3 lastTargetPosition = Vector3.zero;

	// Used for resetting orbit values when direction of the vehicle has been changed.
	private int direction = 1;
	private int lastDirection = 1;

	public float TPSDistance = 6f;				// The distance for TPS camera mode.
	public float TPSHeight = 2f;					// The height we want the camera to be above the target for TPS camera mode.
	public float TPSHeightDamping = 10f;	// Height movement damper.
	public float TPSRotationDamping = 5f;	// Rotation movement damper.
	public float TPSTiltMaximum = 15f;		// Maximum tilt angle related with rigidbody local velocity.
	public float TPSTiltMultiplier = 2f;		// Tilt angle multiplier.
	private float TPSTiltAngle = 0f;			// Current tilt angle.
	public float TPSYawAngle = 0f;			// Yaw angle.
	public float TPSPitchAngle = 7f;			// Pitch angle.

	internal float targetFieldOfView = 60f;	// Camera will adapt its field of view to this target field of view. All field of views below this line will feed this value.

	public float TPSMinimumFOV = 50f;			// Minimum field of view related with vehicle speed.
	public float TPSMaximumFOV = 70f;			// Maximum field of view related with vehicle speed.
	public float hoodCameraFOV = 60f;			// Hood field of view.
	public float wheelCameraFOV = 60f;			// Wheel field of view.
	public float minimumOrtSize = 10f;			// Minimum ortho size related with vehicle speed.
	public float maximumOrtSize = 20f;			// Maximum ortho size related with vehicle speed.

	internal int cameraSwitchCount = 0;					// Used in switch case for running corresponding camera mode method.
		// Cinematic Camera System.

	private Vector3 collisionVector = Vector3.zero;				// Collision vector.
	private Vector3 collisionPos = Vector3.zero;					// Collision position.
	private Quaternion collisionRot = Quaternion.identity;	// Collision rotation.

	private float index = 0f;				// Used for sinus FOV effect after hard crashes. 

	private Quaternion orbitRotation = Quaternion.identity;		// Orbit rotation.

	// Orbit X and Y inputs.
	internal float orbitX = 0f;
	internal float orbitY = 0f;

	// Minimum and maximum Orbit X, Y degrees.
	public float minOrbitY = -20f;
	public float maxOrbitY = 80f;

	//	Orbit X and Y speeds.
	public float orbitXSpeed = 7.5f;
	public float orbitYSpeed = 5f;
	private float orbitResetTimer = 0f;

	// Calculate the current rotation angles for TPS mode.
	private Quaternion currentRotation = Quaternion.identity;
	private Quaternion wantedRotation = Quaternion.identity;
	private float currentHeight = 0f;
	private float wantedHeight = 0f;

	void Awake(){
		if (instance == null) 
		{
			instance = this;
		}
		// Getting Camera.
		thisCam = GetComponentInChildren<Camera>();
        Debug.LogError("Camera" + thisCam.name);
		// Proper settings for selected behavior type.
//		switch(RCCSettings.behaviorType){
//
//		case RCC_Settings.BehaviorType.SemiArcade:
//			break;
//
//		case RCC_Settings.BehaviorType.Drift:
//			break;
//
//		case RCC_Settings.BehaviorType.Fun:
//			break;
//
//		case RCC_Settings.BehaviorType.Racing:
//			break;
//
//		case RCC_Settings.BehaviorType.Simulator:
//			break;
//
//		}
		GetTarget();

	}

	public void GetTarget(){

		// Return if we don't have the player vehicle.
		if (!playerCar)
			return;
			//playerCar = GameObject.FindWithTag ("Player");
		// If player vehicle has RCC_CameraConfig, distance and height will be adjusted.
		if (playerCar.GetComponent<CameraConfig> ())
		{

			TPSDistance = playerCar.GetComponent<CameraConfig> ().distance;
			TPSHeight = playerCar.GetComponent<CameraConfig> ().height;

		}

		// Getting rigid of the player vehicle.
		playerRigid = playerCar.GetComponent<Rigidbody>();

        Debug.LogError("player" + playerRigid);
	}



	void Update(){



		if(index > 0)
			index -= Time.deltaTime * 5f;

		// Lerping current field of view to target field of view.
		thisCam.fieldOfView = Mathf.Lerp (thisCam.fieldOfView, targetFieldOfView, Time.deltaTime * 5f);

	}
	float distance;
	Vector3 directions;
	void LateUpdate (){
		
		if (playerCar == null)
			return;
		distance = (playerCar.transform.position - transform.position).magnitude;
		directions = ((playerCar.transform.position - transform.position) / distance);
		TPS ();
		ORBIT ();
		// Run the corresponding method with choosen camera mode.


	}

	// Change camera by increasing camera switch counter.

	// Change camera by directly setting it to specific mode.



	void TPS(){

		if (lastDirection != 1) {

			direction = 1;
			orbitX = 0f;
			orbitY = 0f;

		}

		lastDirection = 1;

		// Calculate the current rotation angles for TPS mode.
		wantedRotation = playerCar.transform.rotation * Quaternion.AngleAxis ((direction == 1 ? 0 : 180) + (useOrbitInTPSCameraMode ? orbitX : 0), Vector3.up);
		wantedRotation = wantedRotation * Quaternion.AngleAxis ((useOrbitInTPSCameraMode ? orbitY : 0), Vector3.right);

		if(Input.GetKey(KeyCode.B))
			wantedRotation = wantedRotation * Quaternion.AngleAxis ((180), Vector3.up);

		// Wanted height.
		wantedHeight = playerCar.transform.position.y + TPSHeight;

		// Damp the height.
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, TPSHeightDamping * Time.fixedDeltaTime);

		// Damp the rotation around the y-axis.
		if(Time.time > 1)
			currentRotation = Quaternion.Lerp(currentRotation, wantedRotation, TPSRotationDamping * Time.deltaTime);
		else
			currentRotation = wantedRotation;

		// Rotates camera by Z axis for tilt effect.
		TPSTiltAngle = Mathf.Lerp(0f, TPSTiltMaximum * Mathf.Clamp(-playerVelocity.x, -1f, 1f), Mathf.Abs(playerVelocity.x) / 50f);
		TPSTiltAngle *= TPSTiltMultiplier;

		// Set the position of the camera on the x-z plane to distance meters behind the target.
		targetPosition = playerCar.transform.position;
		targetPosition -= (currentRotation) * Vector3.forward * (TPSDistance * Mathf.Lerp(1f, .75f, (playerRigid.velocity.magnitude * 3.6f) / 100f));
		targetPosition += Vector3.up * (TPSHeight * Mathf.Lerp(1f, .75f, (playerRigid.velocity.magnitude * 3.6f) / 100f));

		// SMOOTHED.
//		transform.position = SmoothApproach(pastFollowerPosition, pastTargetPosition, targetPosition, Mathf.Clamp(10f, Mathf.Abs(playerSpeed / 2f), Mathf.Infinity));
		// RAW.
		transform.position = targetPosition;

		thisCam.transform.localPosition = Vector3.Lerp(thisCam.transform.localPosition, new Vector3 (TPSTiltAngle / 10f, 0f, 0f), Time.deltaTime * 3f);

		// Always look at the target.
		transform.LookAt (playerCar.transform);
		transform.eulerAngles = new Vector3(currentRotation.eulerAngles.x + (TPSPitchAngle * Mathf.Lerp(1f, .75f, (playerRigid.velocity.magnitude * 3.6f) / 100f)), transform.eulerAngles.y, -Mathf.Clamp(TPSTiltAngle, -TPSTiltMaximum, TPSTiltMaximum) + TPSYawAngle);

		// Past positions used for proper smooting related with speed.
		lastFollowerPosition = transform.position;
		lastTargetPosition = targetPosition;

		// Collision positions and rotations that affects pivot of the camera.
		collisionPos = Vector3.Lerp(new Vector3(collisionPos.x, collisionPos.y, collisionPos.z), Vector3.zero, Time.unscaledDeltaTime * 5f);

		if(Time.deltaTime != 0)
			collisionRot = Quaternion.Lerp(collisionRot, Quaternion.identity, Time.deltaTime * 5f);

		// Lerping position and rotation of the pivot to collision.
		pivot.transform.localPosition = Vector3.Lerp(pivot.transform.localPosition, collisionPos, Time.deltaTime * 10f);
		pivot.transform.localRotation = Quaternion.Lerp(pivot.transform.localRotation, collisionRot, Time.deltaTime * 10f);

		// Lerping targetFieldOfView from TPSMinimumFOV to TPSMaximumFOV related with vehicle speed.
		targetFieldOfView = Mathf.Lerp(TPSMinimumFOV, TPSMaximumFOV, Mathf.Abs(playerSpeed) / 150f);

		// Sinus FOV effect on hard crashes.
		targetFieldOfView += (5f * Mathf.Cos (index));


	}

	void ORBIT(){

		// Clamping Y.
		orbitY = Mathf.Clamp(orbitY, minOrbitY, maxOrbitY);

		orbitRotation = Quaternion.Euler(orbitY, orbitX, 0f);

		if(playerSpeed > 10f && Mathf.Abs(orbitX) > 1f)
			orbitResetTimer += Time.deltaTime;

		if (playerSpeed > 10f && orbitResetTimer >= 2f) {

			orbitX = 0f;
			orbitY = 0f;
			orbitResetTimer = 0f;

		}

	}

	public void OnDrag(PointerEventData pointerData){

		// Receiving drag input from UI.
		orbitX += pointerData.delta.x * orbitXSpeed * .02f;
		orbitY -= pointerData.delta.y * orbitYSpeed * .02f;

		orbitResetTimer = 0f;

	}



}