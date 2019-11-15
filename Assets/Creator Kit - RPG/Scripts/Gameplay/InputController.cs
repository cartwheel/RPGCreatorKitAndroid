using RPGM.Core;
using RPGM.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace RPGM.UI
{
    /// <summary>
    /// Sends user input to the correct control systems.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        public float stepSize = 0.1f;
        public float moveSpeed = 5f;

        public Text text;
        public Text text2;

        GameModel model = Schedule.GetModel<GameModel>();

        // Touch controls
        Touch touch;
        Vector3 touchPosition, whereToMove;
        bool isMoving = false;
        float previousDistanceToTouchPos, currentDistanceToTouchPos;
        Rigidbody2D rb;

        public enum State
        {
            CharacterControl,
            DialogControl,
            Pause
        }

        State state;

        public void ChangeState(State state) => this.state = state;

        // Hopefully this is called
        private void Start()
        {
            rb = model.player.GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            switch (state)
            {
                case State.CharacterControl:
                    CharacterControl();
                    break;
                case State.DialogControl:
                    DialogControl();
                    break;
            }
        }

        void DialogControl()
        {
            model.player.nextMoveCommand = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                model.dialog.FocusButton(-1);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                model.dialog.FocusButton(+1);
            if (Input.GetKeyDown(KeyCode.Space))
                model.dialog.SelectActiveButton();

            if (Input.touchCount > 0)
            {

            }
        }

        void CharacterControl()
        {


            if (isMoving)
            {
                currentDistanceToTouchPos = (touchPosition - transform.position).magnitude;
                Debug.Log(currentDistanceToTouchPos);
                text.text = currentDistanceToTouchPos.ToString();
            }

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                text2.text = touch.position.ToString();

                if (touch.phase == TouchPhase.Began) { 
                    previousDistanceToTouchPos = 0;
                    currentDistanceToTouchPos = 0;
                    isMoving = true;
                    touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPosition.z = 0f;

                    whereToMove = (touchPosition - transform.position).normalized;
                    rb.velocity = new Vector2(whereToMove.x * moveSpeed, whereToMove.y * moveSpeed);
                }
            }

            if (currentDistanceToTouchPos > previousDistanceToTouchPos)
            {
                isMoving = false;
                rb.velocity = Vector2.zero;
            }

            if (isMoving)
                previousDistanceToTouchPos = (touchPosition - transform.position).magnitude;


            if (Input.GetKey(KeyCode.UpArrow))
                model.player.nextMoveCommand = Vector3.up * stepSize;
            else if (Input.GetKey(KeyCode.DownArrow))
                model.player.nextMoveCommand = Vector3.down * stepSize;
            else if (Input.GetKey(KeyCode.LeftArrow))
                model.player.nextMoveCommand = Vector3.left * stepSize;
            else if (Input.GetKey(KeyCode.RightArrow))
                model.player.nextMoveCommand = Vector3.right * stepSize;
            else
                model.player.nextMoveCommand = Vector3.zero;
        }
    }
}