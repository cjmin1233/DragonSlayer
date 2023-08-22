using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoldenBox : MonoBehaviour, IInteractable
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

    
    // private void OnCollisionStay(Collision other)
    // {
    //     if(other.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
    //     {
    //         if (openSound != null)
    //         {
    //             Debug.Log("soundss");
    //             audioSource.PlayOneShot(openSound, 2f);
    //         }
    //         animator.SetTrigger("Open");
    //         // Item script
    //         StartCoroutine("BoxDestroy");
    //     }
    // }

    private IEnumerator BoxDestroy()
    {
        var item = ItemSpawner.Instacne.GetRandomItem();
        item.transform.position = transform.position;
        item.SetActive(true);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void Interact(GameObject target)
    {
        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound, 2f);
        }
        animator.SetTrigger("Open");
        // Item script
        StartCoroutine(BoxDestroy());
    }

    public void EnterInteract(GameObject target)
    {
        UiManager.Instance.ShowInteractInfo("Open");
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.Add2InteractList(this);
    }    
    private void OnTriggerExit(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.RemoveInteractable(this);
    }

}
