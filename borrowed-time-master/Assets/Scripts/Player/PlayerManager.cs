using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerManager : MonoBehaviour
{
    public static bool isThrowingTimeBomb = false;
    public static Vector3[] pathArray;

    public static int playerHealth = 6;
    public static int playerScore = 0;
    public static bool inConversation = false;
    public static float globalTime = 1;

    public static int mana = 100;

    public static int checkpoint = 1;

    public static float totalTime = 0;
    public static bool playing = false;

    // John - turned the above field static due to unity giving me an err
    // (Yashna)

    public string dialogueUser = "Player" ;// "Other"
    public string dialogue = "";
    public static bool god;

    static float flashRedOnDamageDuration = 0.15f;
    static float flashClock = 0;

    static public bool knockedBack = false;
    static public float knockbackPower = 10;
    static public Vector3 knockbackDirection;
    static public float knockbackClock = 0;
    static public float knockbackDuration = 0.05f;

    public static int isFacing = 0;

    private static Volume v;
    private static ColorAdjustments c;

    // Start is called before the first frame update
    void Start()
    {
        v = GameObject.Find("Post").GetComponent<Volume>();
        v.profile.TryGet(out c);
 
    }  

    // Update is called once per frame
    void Update()
    {
        if (playing) {
            print(totalTime);
            totalTime += Time.deltaTime;
        } else {
            print("no");
        }

        GameObject player = GameObject.Find("Player");

        flashClock += Time.deltaTime;
        if ((float) flashClock >= flashRedOnDamageDuration) {
            player.GetComponent<Renderer>().material.color = Color.white;
        }

        knockbackClock += Time.deltaTime;

        if (knockedBack == true) {
            //print(knockbackDirection*knockbackPower);
            player.transform.position = player.transform.position + knockbackDirection*knockbackPower*Time.deltaTime;

            if (knockbackClock > knockbackDuration) {
                knockedBack = false;
            }
        }
    }

    static void flashRed() {
        GameObject player = GameObject.Find("Player");

        flashClock = 0;
        player.GetComponent<Renderer>().material.color = Color.red;
    }

    static void hitSound() {
        GameObject player = GameObject.Find("Player");

        AudioSource audioSource = player.GetComponent<AudioSource>();
        audioSource.volume = 0.2f;
        audioSource.PlayOneShot(player.GetComponent<PlayerAttack>().hitSound);
    }

    static public void DamagePlayer() {
        flashRed();

        if (god == false) {
            playerHealth  = playerHealth - 1;
            hitSound();
        }
        
        if (playerHealth <= 0) {
            globalTime = 0;
            TimeRealm();
        } else {
            NormalRealm();
        }
    }

    static void TimeRealm() {
        c.saturation.value = -100f;
        PlayerManager.globalTime = 0f;
    }

    public static void NormalRealm()
    {
        c.saturation.value = 0f;
        PlayerManager.globalTime = 1;
    }

    static public void DamagePlayer(Vector3 originOfAttack) {
        GameObject player = GameObject.Find("Player");

        flashRed();

        knockbackDirection = (player.transform.position - originOfAttack).normalized;

        knockedBack = true;
        knockbackClock = 0;

        if (god == false) {
            playerHealth  = playerHealth - 1;
            hitSound();
        }
        
        if (playerHealth <= 0) {
            globalTime = 0;
            TimeRealm();
        } else {
            NormalRealm();
        }
    }

    public static void PlayerScore(int points)
    {
        playerScore += points;
    }
}
