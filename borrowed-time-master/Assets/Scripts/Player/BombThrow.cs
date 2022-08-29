using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class BombThrow : MonoBehaviour
{
    public Transform timeBombPrefab;
    private float chargingTime = 0;

    private float manaClock = 0;

    private AudioSource audioSource;

    public AudioClip throwSound;
    public AudioClip timeRealmSound;

    private Volume v;
    private ColorAdjustments c;

    private int cancelMana = 0;
    //private Saturation s;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        v = GameObject.Find("Post").GetComponent<Volume>();
        v.profile.TryGet(out c);

        audioSource = this.GetComponent<AudioSource>();
    }   

    void TimeRealm() {
        c.saturation.value = -100f;
        PlayerManager.globalTime = 0.1f;
        anim.SetTrigger("BombOut");

        audioSource.loop = true;
        audioSource.volume = 0.3f;
        audioSource.Play();
    }

    void NormalRealm()
    {
        c.saturation.value = 0f;
        PlayerManager.globalTime = 1;
        anim.SetTrigger("BombIn");
        
        audioSource.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.playerHealth > 0) {
            if (Input.GetMouseButtonDown(1))
            {
                // need to later check if no. bombs
                PlayerManager.isThrowingTimeBomb = true;
                TimeRealm();

            } else if (Input.GetMouseButtonUp(1) & PlayerManager.isThrowingTimeBomb | (PlayerManager.mana == 0 & PlayerManager.isThrowingTimeBomb)) {
                PlayerManager.isThrowingTimeBomb = false;
                audioSource.Stop();

                audioSource.volume = 0.7f;
                audioSource.PlayOneShot(throwSound);

                Transform timeBomb = Instantiate(timeBombPrefab, transform.position, transform.rotation);
                timeBomb.transform.rotation = this.transform.rotation;

                float charge = Mathf.Clamp(0.3f + chargingTime*0.2f*1.5f, 0.3f, 0.95f);
                timeBomb.transform.GetComponent<Path>().timeSpeed = 1 - charge;
                timeBomb.transform.Find("Glow").GetComponent<Light2D>().pointLightOuterRadius = chargingTime;
                print(timeBomb.transform.Find("Glow").GetComponent<Light2D>().pointLightOuterRadius);

                cancelMana = 0;
                chargingTime = 0;
                NormalRealm();
            } else if (Input.GetMouseButtonDown(0)) {
                PlayerManager.isThrowingTimeBomb = false;

                PlayerManager.mana += cancelMana;
                cancelMana = 0;

                chargingTime = 0;
                NormalRealm();
            }

            if (PlayerManager.isThrowingTimeBomb == true) {
                chargingTime += Time.deltaTime*1.5f;
                manaClock += Time.deltaTime;

                if (manaClock >= 0.025) {
                    manaClock = 0;
                    PlayerManager.mana -= 1;
                    cancelMana += 1;

                    Mathf.Clamp(PlayerManager.mana, 0, 100);
                }

            } else {
                manaClock += Time.deltaTime;
                
                if (manaClock >= 0.1 & PlayerManager.mana < 100) {
                    manaClock = 0;
                    PlayerManager.mana += 1;

                    Mathf.Clamp(PlayerManager.mana, 0, 100);
                }
            }
        }

        GameObject.Find("Mana").GetComponent<RectTransform>().sizeDelta = new Vector2 (PlayerManager.mana/100f*300f, 20f);
    }
}
