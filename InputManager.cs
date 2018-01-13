using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class InputManager: MonoBehaviour {

    GestureRecognizer recognizer;
    bool tapped = false;
    bool audioPlaying = false;
    AudioSource audio_input;
    GameObject selected;

    // Use this for initialization
    void Start () {
        recognizer = new GestureRecognizer();
        recognizer.Tapped += (arg) =>
        {
            tapped = true;
        };
        recognizer.StartCapturingGestures();

        audio_input = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (tapped)
        {
            //Debug.Log("Tapped!");
            if (!audioPlaying)
            {
                audio_input.Play();
                audioPlaying = true;
            }
            else
            {
                audio_input.Stop();
                audioPlaying = false;
            }
            tapped = false;
        }
        //UpdateRaycast();
	}

    void UpdateRaycast ()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo);

        if (hit)
        {
            if (selected != hitInfo.collider.gameObject)
            {
                selected = hitInfo.collider.gameObject;
                Debug.Log(hitInfo.transform.name + "selected");
            }
        }
    }
}
