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
        animator = GetComponentInChildren<Animator>();
    }

    
    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Open");
            if (openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            // Item script
            StartCoroutine("BoxDestroy");
        }
    }

    private IEnumerator BoxDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
