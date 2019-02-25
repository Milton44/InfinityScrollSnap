using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class InfinityScroll : MonoBehaviour , IBeginDragHandler, IEndDragHandler
{
    [Space(10)]
    public float lineSpacingElements;
    
    [Space(10)]
    [Range(0, 15)]
    public float snapSpeed;

    [Space(10)]
    public RectTransform choicePosition;

    [Space(10)]
    public bool typeTransitionNextState;
    
    private RectTransform[] allRects;
    private bool isScrolling;
    private int firstItem , lastItem;
    private RectTransform content;
    
    private RectTransform target;
    private RectTransform viewPort;
    private ScrollRect scroll;
    private Vector2 contentVector;
    private const float minVelocity = 200;

    void Start()
    {
       
        OrganizeLabel();
        scroll.movementType = ScrollRect.MovementType.Unrestricted;

    }

    private void FixedUpdate()
    {
        FixedContent();
    }

    void Update()
    {
          
        if (scroll.velocity.y < 0)
        {

            CheckTopPosition(); 
           
        }
        if(scroll.velocity.y > 0)
        {
            CheckBotPosition();
            
        }

    }

    public void OrganizeLabel()
    {
         
        scroll = GetComponent<ScrollRect>();
        scroll.StopMovement();
        viewPort = scroll.viewport;
        content = scroll.content;
        float positionPrevious = 0;
        allRects = new RectTransform[content.childCount];

        for (int cont = 0; cont < content.childCount; cont++)
        {
            allRects[cont] = content.GetChild(cont).GetComponent<RectTransform>();
        }

        foreach (RectTransform rect in allRects)
        {
            rect.anchoredPosition = new Vector2(0, positionPrevious - lineSpacingElements);
            positionPrevious = positionPrevious- 2*lineSpacingElements;
          
        }
        firstItem = 0;
        lastItem = allRects.Length - 1;
        target = allRects[firstItem];
        CheckTopPosition();
        CheckBotPosition();
        
    }

    private RectTransform AproximelyRect()
    {

        float midViewY = choicePosition.position.y;
        float bestDistance = 200000f;
        RectTransform target = this.target;

        for (int cont = 0; cont < allRects.Length; cont++)
        {
            float distance = midViewY - allRects[cont].position.y;
            if (Mathf.Abs(distance) < bestDistance)
            {
                if (scroll.velocity.y > 0 )
                {
                    if (distance > 0 || !typeTransitionNextState)
                    {
                        target = allRects[cont];
                        bestDistance = Mathf.Abs(midViewY - allRects[cont].position.y);
                    }

                }
                else if(scroll.velocity.y < 0 )
                {
                    if (distance < 0 || !typeTransitionNextState)
                    {
                        target = allRects[cont];
                        bestDistance = Mathf.Abs(midViewY - allRects[cont].position.y);
                    }
                }
            }
        }
        
        
        return target;
    } 

    private void CheckTopPosition()
    {
        while (!RectTransformUtility.RectangleContainsScreenPoint(viewPort, allRects[lastItem].position, null) && choicePosition.position.y > allRects[lastItem].position.y)
        {
            allRects[lastItem].anchoredPosition = new Vector2(allRects[firstItem].anchoredPosition.x, allRects[firstItem].anchoredPosition.y + 2 * lineSpacingElements);
            firstItem = lastItem;
            lastItem = (lastItem - 1 >= 0) ? lastItem - 1 : allRects.Length - 1;

        }
    }

    private void CheckBotPosition()
    {
        while (!RectTransformUtility.RectangleContainsScreenPoint(viewPort, allRects[firstItem].position, null) && choicePosition.position.y < allRects[firstItem].position.y)
        {
            allRects[firstItem].anchoredPosition = new Vector2(allRects[lastItem].anchoredPosition.x, allRects[lastItem].anchoredPosition.y - 2 * lineSpacingElements);
            lastItem = firstItem;
            firstItem = (firstItem + 1 < allRects.Length) ? firstItem + 1 : 0;

        }
    }

    private  void FixedContent()
    {
        target = AproximelyRect();

        if (isScrolling) return;
        if (Mathf.Abs(scroll.velocity.y) < minVelocity) scroll.StopMovement();

        contentVector.y = Mathf.SmoothStep(content.anchoredPosition.y, choicePosition.anchoredPosition.y - target.anchoredPosition.y, Time.fixedDeltaTime * snapSpeed);
        contentVector.x = content.anchoredPosition.x;
        content.anchoredPosition = contentVector;
    }

    public bool isRunning()
    {
        return scroll.velocity.y != 0;
    }

    public string SnapValor()
    {
        return target.GetComponent<Text>().text;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isScrolling = false;
    
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isScrolling = true;
 
    }

}
