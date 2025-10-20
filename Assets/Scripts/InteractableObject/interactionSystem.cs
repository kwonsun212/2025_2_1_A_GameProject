using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class interactionSystem : MonoBehaviour
{
    [Header("상호 작용 설정")]
    public float interactionRange = 2.0f;           //상호작용 범위
    public LayerMask interactionLayerMask = 1;      //상호작용할 레이어
    public KeyCode interactionKey = KeyCode.E;      //상호작용할 키(E)

    [Header("UI 설정")]
    public Text interactionText;           //상호작용 UI 텍스트
    public GameObject interactionUI;                //상호작용 UI 패널

    private Transform playerTransform;
    private InteractableObject currentInteractiable;          //감지된 오브젝트를 담는 클래스


    void Start()
    {
        playerTransform = transform;
        HideInteractionUI();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInteractables();
        HandleinteractionInput();
    }

    void CheckForInteractables()
    {
        Vector3 checkPosition = playerTransform.position + playerTransform.forward * (interactionRange * 0.5f);
        Collider[] hitColliders = Physics.OverlapSphere(checkPosition, interactionRange, interactionLayerMask);

        InteractableObject closestiInteratable = null;
        float closestDistance = float.MaxValue;

        foreach(Collider collider in hitColliders)
        {
            InteractableObject interactable = collider.GetComponent<InteractableObject>();
            if(interactable != null)
            {
                float distance = Vector3.Distance(playerTransform.position, collider.transform.position);

                Vector3 directionToObject = (collider.transform.position - playerTransform.position).normalized;
                float angle = Vector3.Angle(playerTransform.forward, directionToObject);

                if(angle < 90f && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestiInteratable = interactable;
                }
            }
        }

        if(closestiInteratable != currentInteractiable)
        {
            if(currentInteractiable != null)
            {
                currentInteractiable.OnPlayerExit();
            }

            currentInteractiable = closestiInteratable;

            if(currentInteractiable != null)
            {
                currentInteractiable.OnPlayerEnter();
                ShowInteractionUI(currentInteractiable.GetInteractionText());
            }
            else
            {
                HideInteractionUI();
            }
        }
    }

    void HandleinteractionInput()
    {
        if(currentInteractiable != null && Input.GetKeyDown(interactionKey))
        {
            currentInteractiable.Interact();
        }
    }

    void ShowInteractionUI(string text)
    {
        if(interactionUI != null)
        {
            interactionUI.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.text = text;
        }
    }
    void HideInteractionUI()
    {
        if(interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}
