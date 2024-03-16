using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    public bool IsActivated;

    private Animator[] animators;

    // Start is called before the first frame update
    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    public void Activate()
    {
        foreach(var anim in animators)
        {
            anim.SetBool("IsActivated", true);
            
        }
		IsActivated = true;
	}

	public void Disactivate()
	{
		foreach (var anim in animators)
		{
			anim.SetBool("IsActivated", false);

		}
		IsActivated = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (IsActivated && collision.CompareTag("Player"))
        {
			foreach (var anim in animators)
			{
                anim.SetTrigger("PlayerEnter");

			}
		}
	}
}