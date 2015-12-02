using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

public class PlayerShooting : MonoBehaviour {

    //public instance variables
    public ParticleSystem muzzleFlash;
    public GameObject impact;
    public Animator rifleAnimator;
    public AudioSource bulletFireSound;
    public AudioSource BulletImpactSound;
    public GameObject explosion;
    public GameController gameController;

    //Private instance variable
    private GameObject[] _impacts;
    private int _currentImpact = 0;
    private int _maxImpact = 5;
    private Transform _transform;


    private bool _shooting = false;

	// Use this for initialization
	void Start () {

        //Reference to gameObject's transform component
        this._transform = gameObject.GetComponent<Transform>();

        //Object Pool for Impacts
        this._impacts = new GameObject[this._maxImpact];
        for(int impactCount = 0; impactCount < this._maxImpact; impactCount++)
        {
            this._impacts[impactCount] = (GameObject) Instantiate(this.impact);
        }
	}
	
	// Update is called once per frame
	void Update () {

        //Play Muzzle Flash and shoot impact when left mouse button pressed
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            this._shooting = true;
            this.muzzleFlash.Play();
            this.bulletFireSound.Play();
            this.rifleAnimator.SetTrigger("Fire");
        }

        if (CrossPlatformInputManager.GetButtonUp("Fire1"))
        {
            this._shooting = false;
        }

    }

    //Physics effects
    void FixedUpdate()
    {
        if (this._shooting)
        {
            this._shooting = false;

            RaycastHit hit;

            Debug.DrawRay(this._transform.position, this._transform.forward);
            if (Physics.Raycast(this._transform.position, this._transform.forward, out hit, 50f))
            {
                if (hit.transform.CompareTag("Wood"))
                {
                    Destroy(hit.transform.gameObject);
                    Instantiate(this.explosion, hit.point, Quaternion.identity);
                    gameController.AddScore(100);

                }
                //If hit by Player, It will get Destroyed but by taking the life of Player
                if (hit.transform.CompareTag("Destroyer"))
                {
                    Destroy(hit.transform.gameObject);
                    Instantiate(this.explosion, hit.point, Quaternion.identity);
                    gameController.SubtractLives(1);//Decrease life of Player if he hits the object

                }
                



                //Assign the position of emitter at the point of hitting(impact)
                this._impacts[_currentImpact].transform.position = hit.point;
                //Play the Particle Effect
                this._impacts[this._currentImpact].GetComponent<ParticleSystem>().Play();
                //Play the bullet sound on hit
                this.BulletImpactSound.Play();

                // ensure that you don't go out of bounds of the object pool
                if (++this._currentImpact >= this._maxImpact)
                {
                    this._currentImpact = 0;
                }

            }
        }

    }  
}
