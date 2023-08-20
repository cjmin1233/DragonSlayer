using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoldenBox : MonoBehaviour
{
    private Animator animator;
    public AudioClip openSound;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
    }

    
    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (openSound != null)
            {
                Debug.Log("soundss");
                audioSource.PlayOneShot(openSound, 2f);
            }
            animator.SetTrigger("Open");
            // Item script
            StartCoroutine("BoxDestroy");
        }
    }

    private IEnumerator BoxDestroy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
