using UnityEngine;
using System.Collections;

public class AICharacter : MonoBehaviour
{
    #region Variables
    //Our stored Components
    StateManager states;
    public StateManager enStates;

    public float changeStateTolerance = 3; //How close is considered close combat

    public float normalRate = 1; //How fast will his AI decide state will cycle on the normal state
    float nrmTimer;

    public float closeRate = 0.5f;//How fast will his AI decide state will cycle on the close state
    float clTimer;

    public float blockingRate = 1.5f; //For how long will he block
    float blTimer;

    public float aiStateLife = 1; //How much time does it take to reset the AI state
    float aiTimer;

    bool initiateAI; //When it has an AI state to run
    bool closeCombat;//if we are in close combat

    bool gotRandom; //Helps so that we are not updating our random variable every frame
    float storeRandom; //Stores our random float

    //blocking variables
    bool checkForBlocking;
    bool blocking;
    float blockMultiplier; //we didn't use this in the end, but you can add to the percentage of if it's going to block or take damage

    //How many times we will attack variables
    bool randomizeAttacks;
    int numberOfAttacks;
    int curNumAttacks;

    //Jump variables
    public float JumpRate = 1;
    float jRate;
    bool jump;
    float jtimer;
    #endregion 

    public AttackPatterns[] attackPatterns;

    //Our AI states
    public enum AIState
    { 
        closeState,
        normalState,
        resetAI
    }

    public AIState aiState;

	void Start () {

        states = GetComponent<StateManager>();

        AISnapshots.GetInstance().RequestAISnapshot(this);
	}
		
	void Update () 
    {
        //Call our functions
        CheckDistance();
        States();
        AIAgent();
	}

    //Holds our states
    void States()
    {
        //This switch decides which timer to run or not
        switch (aiState)
        { 
            case AIState.closeState:
                CloseState();
                break;
            case AIState.normalState:
                NormalState();
                break;
            case AIState.resetAI:
                ResetAI();
                break;
        }

        //This functions are called independent of the AI decide cycle so that it's more responsive
        //Blocking();
        Jumping();
    }

    //This function manages the stuff that the agent has to do
    void AIAgent()
    { 
        //If it has something to do, meaning that the AI cycle has made a full run
        if(initiateAI)
        {
            //Start the reset AI process, note this is not instant
            aiState = AIState.resetAI;
            //Create a multiplier
            float multiplier = 0;

            //Get our random value
            if (!gotRandom)
            {
                storeRandom = ReturnRandom();
                gotRandom = true;
            }

            //If we are not in close combat
            if (!closeCombat)
            {
                //We have 30% more chances of moving
                multiplier += 30;
            }
            else 
            {
                //..we have 30% more chances to attack 
                multiplier -= 30;
            }
      
            //Compare our random value with the added modifiers
            if (storeRandom + multiplier < 50)
            {
                Attack(); //...and either Attack
            }
            else
            {
                Movement();
            }
        }
    }

    //Our Attack logic goes here
    void Attack()
    {
        //Take a random value
        if (!gotRandom)
        {
            storeRandom = ReturnRandom();
            gotRandom = true;
        }

        //There's 75 chances of doing a normal attack...
        //if (storeRandom < 75)
        //{
            //See how many attacks he will do...
            if (!randomizeAttacks)
            {
                //..by getting a random int between 1 and 4, the 1 is because we want him to attack at least once
                numberOfAttacks = (int)Random.Range(1, 4);
                randomizeAttacks = true;
            }

            //if we haven't attacked more than the maximum times
            if (curNumAttacks < numberOfAttacks)
            {              
                //Then decide at random which attack we want to do, the max number of the random range is the legth of our attack array
                int attackNumber = Random.Range(0, attackPatterns.Length);

                StartCoroutine(OpenAttack(attackPatterns[attackNumber],0));
                

                //..and increment to the times we attacked
                curNumAttacks++;
            }
        /*}
        else//... or special one
        {
            if(curNumAttacks < 1)//We want the special attack to happen only once
            {
                //states.SpecialAttack = true;
                curNumAttacks++;
            }
        }*/
    }

    void Movement()
    {
        //Take a random value
        if (!gotRandom)
        {
            storeRandom = ReturnRandom();
            gotRandom = true;
        }

        //There's 90% chance of moving close to the enemy
        if (storeRandom < 90)
        {
            if (enStates.transform.position.x < transform.position.x)
                states.horizontal = -1;
            else
                states.horizontal = 1;
        }
        else//..or away from him
        {
            if (enStates.transform.position.x < transform.position.x)
                states.horizontal = 1;
            else
                states.horizontal = -1;
        }

        //Note: You can create a modifier based from Health to manipulate the chances of him moving away when injured
    }

    //This function resets all our variables
    void ResetAI()
    {
        aiTimer += Time.deltaTime;

        if(aiTimer > aiStateLife)
        {
            initiateAI = false;
            states.horizontal = 0;
            states.vertical = 0;
            aiTimer = 0;

            gotRandom = false;

            //And also there's a chance of switching the ai state from normal to close so that we get a bit more randomness
            storeRandom = ReturnRandom();
            if (storeRandom < 50)
                aiState = AIState.normalState;
            else
                aiState = AIState.closeState;

            curNumAttacks = 1;
            randomizeAttacks = false;
        }
    }
    //Checks the distance from our position and the enemy and changes the state accordingly
    void CheckDistance()
    {
        //take the distance
        float distance = Vector3.Distance(transform.position, enStates.transform.position);

        //compare it with our tolerance
        if (distance < changeStateTolerance)
        {
            //If we are not in the process of reset the AI, then change the state
            if (aiState != AIState.resetAI)
                aiState = AIState.closeState;

            //If we are close, then we are in close combat
            closeCombat = true;
        }
        else
        {
            //If we are not in the process of reset the AI, then change the state
            if (aiState != AIState.resetAI)
                aiState = AIState.normalState;

            //If we were close to the enemy and then we start moving away...
            if (closeCombat)
            { 
                //take a random value
                if(!gotRandom)
                {
                    storeRandom = ReturnRandom();
                    gotRandom = true;
                }

                //...and then there's 60% chances of our agent following the enemy
                if(storeRandom < 60)
                {
                    Movement();
                }
            }

            //We probably are no longer in close combat
            closeCombat = false;
        }
    }
    //Our Blocking Logic goes here
    void Blocking()
    { 
        //If we are about to recieve damage
        if (states.gettingHit)
        {
            //..get the random value
            if (!gotRandom)
            {
                storeRandom = ReturnRandom();
                gotRandom = true;
            }

            //...there's 50% chances of us blocking
            if (storeRandom < 50)
            {
                blocking = true;
                states.gettingHit = false;
                //states.blocking = true;
            }
        }

        //If we are blocking, then start counting so that we do not block forever
        if(blocking)
        {
            blTimer += Time.deltaTime;

            if (blTimer > blockingRate)
            {
                //states.blocking = false;
                blTimer = 0;
            }
        }
    }   
    //The normal state AI decide state cycle
    void NormalState()
    {
        nrmTimer += Time.deltaTime;

        if (nrmTimer > normalRate)
        {
            initiateAI = true;
            nrmTimer = 0;
        }
    }
    //The close state AI decide state cycle
    void CloseState()
    {
        clTimer += Time.deltaTime;

        if (clTimer > closeRate)
        {
            clTimer = 0;
            initiateAI = true;
        }
    }
    //Our jumping logic goes here
    void Jumping()
    {
        //if the player jumps, or we want to jump
        if(!enStates.onGround)
        {
            float ranValue = ReturnRandom();

            if(ranValue < 50)
            {
                jump = true;
            }
        }
        
        if (jump)
        {
            //then add to vertical input
            states.vertical = 1;
            jRate = ReturnRandom();
            jump = false;//we don't want to keep jumping
        }
        else
        {
            //we still need to reset the vertical input otherwise it will be always jumping
            states.vertical = 0;
        }

        //Our jump timer determines on how many secs it will run a check if he wants to jump or not
        jtimer += Time.deltaTime;

        if(jtimer > JumpRate*10)
        {
            //get a random value again
            

            //then there's 50% chances of jumping or not
            if (jRate < 50)
            {
                jump = true;
            }
            else
            {
                jump = false;
            }

            jtimer = 0;
        }

    }
    //A simple float that returns a random value, we use this a lot
    float ReturnRandom()
    {
        float retVal = Random.Range(0, 101);
        return retVal;
    }

    IEnumerator OpenAttack(AttackPatterns a, int i)
    {
        int index = i;
        float delay = a.attacks[index].delay;
        states.attack1 = a.attacks[index].attack1;
        states.attack2 = a.attacks[index].attack2;
        yield return new WaitForSeconds(delay);

        states.attack1 = false;
        states.attack2 = false;

        if(index < a.attacks.Length - 1 )
        {
            index++;
            StartCoroutine(OpenAttack(a, index));
        }
       
    }

    [System.Serializable]
    public class AttackPatterns
    {
        public AttacksBase[] attacks;
    }

    [System.Serializable]
    public class AttacksBase
    {
        public bool attack1;
        public bool attack2;
        public float delay;
    }
}

/*Keep in mind that whenever you do a percentage check you can even randomize that.
 * You can also manipulate the percentages based on difficulty settings.
 * You can also randomize the attack rates further by adding and removing secs.
 * You can also make different variables for each random value you are taking.
 * Basically, you can randomize everything and you will have more unpredictable AI.
 */
