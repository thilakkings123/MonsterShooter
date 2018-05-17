using UnityEngine;

public class TankBoss : BoomerEnemy {

    [SerializeField]
    private float idleTime, attackTime;
    [SerializeField]
    private bool attackMode = false;
    private float currentIdleTime, currentAttackTime;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
        currentIdleTime = idleTime;
        currentAttackTime = attackTime;
	}

    // Update is called once per frame
    protected override void Update ()
    {
        if (damageScript.CurrentHealth <= 0)
        {
            AudioManager.instance.PlayZombieDie();
            Instantiate(coin, transform.position, Quaternion.identity);
            GameObject death = Instantiate(deathEffect, transform.position, Quaternion.identity);
            death.GetComponent<DeactivateObject>().BasicSettings(2f);
            gameObject.SetActive(false);
        }

        if (!attackMode)
        {
            currentIdleTime -= Time.deltaTime;
            if (currentIdleTime <= 0)
            {
                attackMode = true;
                currentAttackTime = attackTime;
            }
        }
        else if (attackMode)
        {
            currentAttackTime -= Time.deltaTime;

            if (currentAttackTime > 0)
            {
                currentTime -= Time.deltaTime;
                if (CheckPlayer())
                {
                    LookAtPlayer();
                    if (currentTime <= 0)
                    {
                        currentTime = coolDown;
                        Shoot();
                    }
                }
            }
            else if (currentAttackTime <= 0)
            {
                attackMode = false;
                currentIdleTime = idleTime;
                currentTime = 0;
            }
        }
	}

    protected override bool CheckPlayer()
    {
        return base.CheckPlayer();
    }

    protected override void LookAtPlayer()
    {
        base.LookAtPlayer();
    }

    protected override void Shoot()
    {
        base.Shoot();
    }
}
