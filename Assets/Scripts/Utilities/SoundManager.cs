﻿using UnityEngine;
using System.Collections;

namespace Completed
{
	public class SoundManager : MonoBehaviour
	{
		public AudioSource efxSource;                   //Drag a reference to the audio source which will play the sound effects.
        public AudioSource efxSource2;
        public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.
		public static SoundManager instance = null;     //Needed as part of the functionality in Awake so there can only be one instance.             
		public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
		public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
        public bool isPlaying = false;

        //public GameObject[] Player;

        void Awake()
		{
			//Check if there is already an instance of SoundManager
			if (instance == null)
				//if not, set it to this.
				instance = this;
			//If instance already exists:
			else if (instance != this)
				//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
				Destroy(gameObject);

			//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
			DontDestroyOnLoad(gameObject);
		}


		//Used to play single sound clips.
		public void PlaySingle(AudioClip clip)
		{
			//Set the clip of our efxSource audio source to the clip passed in as a parameter.
			efxSource.clip = clip;

			//Play the clip.
			efxSource.Play();
		}

        //Random but not pitched.
        public void RandomOnly(AudioClip[] clips)
        {
            //Generate a random number between 0 and the length of our array of clips passed in.
            int randomIndex = Random.Range(0, clips.Length);

            //Set the clip to the clip at our randomly chosen index.
            efxSource.clip = clips[randomIndex];
            // musicSource.clip = clips[randomIndex];

            //Play the clip, as one shot so multiple clips can play at once.
            efxSource.PlayOneShot(efxSource.clip);
        }


        //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
        public void RandomizeSfxOnce(params AudioClip[] clips)
		{
            //Generate a random number between 0 and the length of our array of clips passed in.
            int randomIndex = Random.Range(0, clips.Length);

			//Choose a random pitch to play back our clip at between our high and low pitch ranges.
			//float randomPitch = Random.Range(lowPitchRange, highPitchRange);

			//Set the pitch of the audio source to the randomly chosen pitch.
			//efxSource.pitch = randomPitch;
            //musicSource.pitch = randomPitch;

            //Set the clip to the clip at our randomly chosen index.
            efxSource.clip = clips[randomIndex];
            // musicSource.clip = clips[randomIndex];


            efxSource.Play();

        }

        //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
        public void RandomizeMusicOnce(params AudioClip[] clips)
        {
            //Generate a random number between 0 and the length of our array of clips passed in.
            int randomIndex = Random.Range(0, clips.Length);

            //Choose a random pitch to play back our clip at between our high and low pitch ranges.
            //float randomPitch = Random.Range(lowPitchRange, highPitchRange);

            //Set the pitch of the audio source to the randomly chosen pitch.
           // efxSource2.pitch = randomPitch;

            //Set the clip to the clip at our randomly chosen index.
            efxSource2.clip = clips[randomIndex];
            // musicSource.clip = clips[randomIndex];


            efxSource2.Play();

        }

        //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
        public void RandomizeSfx(params AudioClip[] clips)
		{
			//Generate a random number between 0 and the length of our array of clips passed in.
			int randomIndex = Random.Range(0, clips.Length);

			//Choose a random pitch to play back our clip at between our high and low pitch ranges.
			//float randomPitch = Random.Range(lowPitchRange, highPitchRange);

			//Set the pitch of the audio source to the randomly chosen pitch.
			//efxSource.pitch = randomPitch;
			//musicSource.pitch = randomPitch;

			//Set the clip to the clip at our randomly chosen index.
			efxSource.clip = clips[randomIndex];
			// musicSource.clip = clips[randomIndex];

			//Play the clip, as one shot so multiple clips can play at once.
			efxSource.PlayOneShot(efxSource.clip);
		}
	}

}