using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArea : MonoBehaviour
{
    public TutorialShow tutorialShow;
    public Transform triggerArea;
    public float triggerWidth;
    public float triggerHeight;
    private Vector2 botLeft;
    private Vector2 topRight;
    public LayerMask whatIsPlayer;

    private void Update()
    {
        Show();
    }

    public void Show()
    {
        botLeft.Set(triggerArea.position.x - (triggerWidth / 2), triggerArea.position.y - (triggerHeight / 2));
        topRight.Set(triggerArea.position.x + (triggerWidth / 2), triggerArea.position.y + (triggerHeight / 2));
        Collider2D context = Physics2D.OverlapArea(botLeft, topRight, whatIsPlayer);


        if (context != null)
        {
            tutorialShow.Enable();
        }
        else
        {
            tutorialShow.Disable();
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 botLeft = new Vector2(triggerArea.position.x - (triggerWidth / 2), triggerArea.position.y - (triggerHeight / 2));
        Vector2 botRight = new Vector2(triggerArea.position.x + (triggerWidth / 2), triggerArea.position.y - (triggerHeight / 2));
        Vector2 topRight = new Vector2(triggerArea.position.x + (triggerWidth / 2), triggerArea.position.y + (triggerHeight / 2));
        Vector2 topLeft = new Vector2(triggerArea.position.x - (triggerWidth / 2), triggerArea.position.y + (triggerHeight / 2));

        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }
}
