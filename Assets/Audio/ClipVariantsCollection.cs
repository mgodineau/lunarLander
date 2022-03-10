using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "cilVariants", menuName = "ScriptableObjects/ClipVariantsCollection", order = 1)]
public class ClipVariantsCollection : ScriptableObject
{
	
	[SerializeField]
	private AudioClip[] clipVariants = new AudioClip[0];
	
	
	public AudioClip GetRandomClip() {
		
		if( clipVariants.Length == 0 ) {
			throw new System.Exception("The \"clipVariants\" array must not be empty.");
		}
		
		int clipId = Random.Range(0, clipVariants.Length);
		return clipVariants[clipId];
		
	}
	
	
	public bool PlayRandomClip( AudioSource source ) {
		
		
		try {
			AudioClip clip = GetRandomClip();
			source.PlayOneShot(clip);
			
		} catch (System.Exception e) {
			Debug.LogWarning( "Error : " + e.Message );
			return false;
		}
		
		return true;
	}
	
	
}
