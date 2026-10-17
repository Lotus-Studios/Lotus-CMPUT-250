using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForcedDialogueCollider : MonoBehaviour
{
    [SerializeField] private LotusNPC lotus;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lotus.doFirstDialogue();
        }
    }
}
