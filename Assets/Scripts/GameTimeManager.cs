using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public AudioSource alertAudio;
    private bool alarmPlaying;

    public AudioSource freezeAudio;

    public Transform mapDivider;

    private CanvasManager canvasManager;
    private GameObject player;
    private PlayerHealth playerHealth;

    //In seconds
    public float leftMaxTime = 60f;
    public float rightMaxTime = 60f;
    private float leftTimer = 0f;
    private float rightTimer = 0f;

    public float gameTime = 0f;

    private bool gameActive = false;
    private bool gameIsEnding = false;

    private float dmgTick = 4f;
    private int playerDmg = 20;

    public float freezeDuration = 5f; //Also the time gained within the duration
    private bool freezeTime = false;
    private float timer = 0f;
    private float freezeCooldown = 2f;
    private bool canFreeze = true;

    public void Setup(ref CanvasManager canvas, ref GameObject gamePlayer)
    {
        canvasManager = canvas;
        player = gamePlayer;
        playerHealth = player.GetComponentInChildren<PlayerHealth>();
        ResetTimes();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsEnding)
        {
            timer += Time.deltaTime;
        }

        if (gameActive)
        {
            gameTime += Time.deltaTime;
            if (!freezeTime)
            {
                if (player.transform.position.x < mapDivider.position.x)
                {
                    //Player is on left side of map
                    if (leftTimer > 1f)
                    {
                        leftTimer -= Time.deltaTime;
                        AlarmAudio(leftTimer);
                        UpdateSideTimerUI(leftTimer, "left", true);
                    }
                    else
                    {
                        if (gameIsEnding)
                        {
                            //Damage
                            if (timer > dmgTick)
                            {
                                timer = 0f;
                                //Damage player
                                playerHealth.TakeDamage(playerDmg);
                            }
                        } 
                        else
                        {
                            //Game Over
                            gameIsEnding = true;
                        }                        
                    }

                    if (rightTimer > 1f) IncreaseRightTimer(true);
                }
                else
                {
                    //Player is on right side of map
                    if (rightTimer > 1f)
                    {
                        rightTimer -= Time.deltaTime;
                        AlarmAudio(rightTimer);
                        UpdateSideTimerUI(rightTimer, "right", true);
                    }
                    else
                    {
                        if (gameIsEnding)
                        {
                            //Damage
                            if (timer > dmgTick)
                            {
                                timer = 0f;
                                //Damage player
                                playerHealth.TakeDamage(playerDmg);
                            }
                        }
                        else
                        {
                            //Game Over
                            gameIsEnding = true;
                        }
                    }

                    if(leftTimer > 1f) IncreaseLeftTimer(true);
                }

                //Check if times are equal
                if (canFreeze && Mathf.FloorToInt(leftTimer) == Mathf.FloorToInt(rightTimer) && !gameIsEnding)
                {
                    freezeAudio.Play();
                    UpdateFrozenGame();
                    freezeTime = true;
                    canFreeze = false;
                }
            }
            else
            {
                //Freeze is active
                timer += Time.deltaTime;

                IncreaseLeftTimer(false);
                IncreaseRightTimer(false);

                if (timer > freezeDuration)
                {
                    //Unfreeze time
                    freezeTime = false;
                    timer = 0f;

                    //Update colors to remove frozen effect
                    UpdateSideTimerUI(rightTimer, "right", true);
                    UpdateSideTimerUI(leftTimer, "left", true);

                    canvasManager.Unfreeze();

                    StartCoroutine(FreezeCooldown());
                }
      
            }

            UpdateGameTimer(gameTime);
        }
    }

    IEnumerator FreezeCooldown()
    {
        yield return new WaitForSeconds(freezeCooldown);
        canFreeze = true;
    }

    void AlarmAudio(float currentTime)
    {
        int estimatedTime = Mathf.FloorToInt(currentTime);
        if (estimatedTime == 10 && !alarmPlaying)
        {
            alarmPlaying = true;
            StartCoroutine(PlayAlarm());
        }
    }

    IEnumerator PlayAlarm()
    {
        alertAudio.Play();
        yield return new WaitForSeconds(1f);
        alertAudio.Play();
        yield return new WaitForSeconds(1f);
        alertAudio.Play();
        yield return new WaitForSeconds(1f);

        alarmPlaying = false;
    }

    void IncreaseRightTimer(bool display)
    {
        //Increase Right timer
        if (rightTimer <= 60f)
        {
            rightTimer += Time.deltaTime;
            UpdateSideTimerUI(rightTimer, "right", display);
        }
    }

    void IncreaseLeftTimer(bool display) 
    {
        //Increase Left timer
        if (leftTimer <= 60f)
        {
            leftTimer += Time.deltaTime;
            UpdateSideTimerUI(leftTimer, "left", display);
        }
    }

    void UpdateFrozenGame()
    {
        canvasManager.UpdateFreezeColor();
    }

    void UpdateSideTimerUI(float time, string side, bool display)
    {
        float currentTime = Mathf.FloorToInt(time);
        switch (side)
        {
            case "left":
                canvasManager.UpdateLeftTimer(currentTime, display);
                break;
            case "right":
                canvasManager.UpdateRightTimer(currentTime, display);
                break;
        }
    }

    void UpdateGameTimer(float time)
    {
        float minute = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        if(minute >= 60)
        {
            gameIsEnding = true;
            canvasManager.UpdateGameTimeColor("Red");
        }

        string currentTime = string.Format("{00:00} : {1:00}", minute, seconds);
        canvasManager.UpdateGameTime(currentTime);
    }

    public void UpdateGameStatus(bool active)
    {
        gameActive = active;
    }

    public void ResetTimes()
    {
        leftTimer = leftMaxTime;
        rightTimer = rightMaxTime;

        //Update side timer UI
        canvasManager.UpdateLeftTimer(leftTimer, true);
        canvasManager.UpdateRightTimer(rightTimer, true);
        
        gameTime = 0f;
        gameIsEnding = false;

        timer = 0f;
        freezeTime = false;

        //Update game UIs
        string currentTime = string.Format("{00:00} : {1:00}", 0, 0);
        canvasManager.UpdateGameTime(currentTime);
        canvasManager.UpdateGameTimeColor("White");
    }
}
