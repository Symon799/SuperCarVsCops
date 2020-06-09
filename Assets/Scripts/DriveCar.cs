using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriveCar : MonoBehaviour {

	public GameObject shield;
	public Joystick joystick;
	public Image joystickBtn;
	public Image touchBtn;
	
	public bool ControlModeJoy = true;
	public float rotationSpeed = 20f;
	public float speed = 17f;
	private float axisTurn;
	private GameManager gameManager;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		SetControlsJoy(ControlModeJoy);
	}

	public void SetControlsJoy(bool joy)
	{
		ControlModeJoy = joy;
		if (joy)
		{
			joystickBtn.color = Color.grey;
			touchBtn.color = Color.white;
		}
		else
		{
			joystickBtn.color = Color.white;
			touchBtn.color = Color.grey;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		transform.position += transform.forward * Time.deltaTime * speed;
		if (!gameManager.GetStartMenu())
		{
			if (!ControlModeJoy)
			{
				if (Input.touchCount > 0)
				{
					Touch touch = Input.GetTouch(Input.touchCount - 1);
					if (touch.phase == TouchPhase.Began)
					{
						if (touch.position.x < Screen.width / 2)
							axisTurn = -1;

						if (touch.position.x > Screen.width / 2)
							axisTurn = 1;
					}
				}
				else
				{
					axisTurn = Input.GetAxis("Horizontal");
				}
				transform.Rotate(0, axisTurn * rotationSpeed * Time.deltaTime, 0);
			}
			else
			{
				joystick.gameObject.SetActive(true);
				Vector3 dir = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);
				transform.rotation = Quaternion.LookRotation (Vector3.RotateTowards(transform.forward, dir, 5 *Time.deltaTime, 0f));
			}
		}
		else
		{
			joystick.gameObject.SetActive(false);
		}
		if (!gameManager.IsShield())
		{
			shield.SetActive(false);
		}
		
	}

	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")
		|| other.gameObject.CompareTag("TruckEnemy")
		|| other.gameObject.CompareTag("Obstacle"))
        {
            gameManager.DecreaseLife();
        }
		if (other.CompareTag("Coin"))
        {
            gameManager.IncreaseCoins();
            Destroy(other.gameObject);
        }
		else if (other.gameObject.CompareTag("Shield"))
        {
			shield.SetActive(true);
			if (gameManager.SetShield())
            	Destroy(other.gameObject);
		}
		else if (other.gameObject.CompareTag("Orbs"))
		{
			int orbsLevel = GetComponentInChildren<Orbs>().AddOrbLevel();
			if (orbsLevel != -1)
            	Destroy(other.gameObject);
		}
		else if (other.gameObject.CompareTag("Shoot"))
		{
			int shootLevel = GetComponentInChildren<Shoot>().AddShootLevel();
			if (shootLevel != -1)
            	Destroy(other.gameObject);
		}

    }
	public void ResetSkills()
	{
		GetComponentInChildren<Orbs>().ResetOrbLevel();
		GetComponentInChildren<Shoot>().ResetShootLevel();
	}

	public void RemoveSkill(int nb)
	{
		for (int i = 0; i < nb; i++)
		{
			Orbs orbs = GetComponentInChildren<Orbs>();
			Shoot shoot = GetComponentInChildren<Shoot>();
			int orbLevel = orbs.GetOrbLevel();
			int shootLevel = shoot.GetShootLevel();

			if (orbLevel > 0 && shootLevel > 0)
			{
				int skillNb = Random.Range(0, 2);
				if (skillNb == 0)
					orbs.RemoveOrbLevel();
				else
					shoot.RemoveShootLevel();
			}
			else if (orbLevel > 0)
			{
				orbs.RemoveOrbLevel();
			}
			else if (shootLevel > 0)
			{
				shoot.RemoveShootLevel();
			}
		}
	}
}
