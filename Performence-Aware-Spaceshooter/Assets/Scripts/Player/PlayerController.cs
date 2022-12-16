using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private KeyCode[] _input = { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D };
	[SerializeField] private float _thrust = 20;
	[SerializeField] private float _torque = 20;
	[SerializeField] private Rigidbody rb; 
	
	private void Update()
	{
		if (Input.GetKey(_input[0])) //forward
		{
			rb.AddForce(transform.forward * _thrust * Time.deltaTime); 
		}
		if (Input.GetKey(_input[1])) //Reverse
		{
			rb.AddForce(-transform.forward * _thrust * Time.deltaTime);
		}
		if (Input.GetKey(_input[2])) //Left
		{
			transform.eulerAngles += new Vector3(0, -_torque, 0) * Time.deltaTime; 
		}
		if (Input.GetKey(_input[3])) //Right
		{
			transform.eulerAngles += new Vector3(0, _torque, 0) * Time.deltaTime;
		}
	}

	private void Shoot()
	{

	}
}
