using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

namespace CVVTuber
{
    public class CameraTouchController : MonoBehaviour
    {
        [SerializeField, Range(0.0f, 1.0f)]
        protected float moveSpeed = 0.01f;

        [SerializeField, Range(0.0f, 1.0f)]
        protected float rotateSpeed = 0.3f;

        [SerializeField, Range(0.0f, 1.0f)]
        protected float zoomSpeed = 0.03f;

        protected Vector3 preMousePos;

#if ENABLE_INPUT_SYSTEM
        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }
#endif

        protected virtual void Update()
        {
#if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)
            TouchUpdate ();
#else
            MouseUpdate();
#endif
        }

        protected virtual void TouchUpdate()
        {
#if ENABLE_INPUT_SYSTEM
            // New Input System
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
            {
                var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

                if (EventSystem.current != null)
                {
                    if (touches.Count == 1 && EventSystem.current.IsPointerOverGameObject(touches[0].finger.index))
                        return;
                    if (touches.Count == 2 && (EventSystem.current.IsPointerOverGameObject(touches[0].finger.index) || EventSystem.current.IsPointerOverGameObject(touches[1].finger.index)))
                        return;
                }

                if (touches.Count == 1)
                {
                    var touch = touches[0];
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                    {
                        // rotate
                        this.transform.parent.gameObject.transform.Rotate(0, touch.delta.x * rotateSpeed, 0);

                        // move
                        this.transform.position += new Vector3(0, -touch.delta.y * moveSpeed / 10, 0);
                        if (this.transform.localPosition.y < -2.0f)
                            this.transform.localPosition = new Vector3(this.transform.localPosition.x, -2.0f, this.transform.localPosition.z);
                        if (this.transform.localPosition.y > 2.0f)
                            this.transform.localPosition = new Vector3(this.transform.localPosition.x, 2.0f, this.transform.localPosition.z);
                    }
                }
                else if (touches.Count == 2)
                {
                    var t0 = touches[0];
                    var t1 = touches[1];

                    Vector2 t0Prev = t0.screenPosition - t0.delta;
                    Vector2 t1Prev = t1.screenPosition - t1.delta;

                    float prevDist = (t0Prev - t1Prev).magnitude;
                    float currDist = (t0.screenPosition - t1.screenPosition).magnitude;
                    float deltaMag = prevDist - currDist;

                    // zoom
                    this.transform.localPosition += new Vector3(0, 0, deltaMag * zoomSpeed / 10);

                    if (this.transform.localPosition.z < -5.0f)
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -5.0f);
                    if (this.transform.localPosition.z > 5.0f)
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 5.0f);
                }
            }
#else
            // Old Input System
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (EventSystem.current != null)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        return;
                    }
                }

                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);

                    //rotate
                    transform.parent.gameObject.transform.Rotate(0, touch.deltaPosition.x * rotateSpeed, 0);

                    //move
                    transform.position += new Vector3(0, -touch.deltaPosition.y * moveSpeed / 10, 0);
                    if (transform.localPosition.y < -2.0f)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, -2.0f, transform.localPosition.z);
                    }

                    if (transform.localPosition.y > 2.0f)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, 2.0f, transform.localPosition.z);
                    }
                }
                else if (Input.touchCount == 2)
                {
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    //zoom
                    transform.localPosition += new Vector3(0, 0, deltaMagnitudeDiff * zoomSpeed / 10);

                    if (transform.localPosition.z < -5.0f)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -5.0f);
                    }

                    if (transform.localPosition.z > 5.0f)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 5.0f);
                    }
                }
            }
#endif
        }

        protected virtual void MouseUpdate()
        {
#if ENABLE_INPUT_SYSTEM
            // New Input System
            var mouse = Mouse.current;
            if (mouse == null) return;

            float scrollWheel = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scrollWheel) > 0.01f)
                MouseWheel(scrollWheel);

            if (mouse.leftButton.wasPressedThisFrame)
                preMousePos = mouse.position.ReadValue();

            MouseDrag(mouse.position.ReadValue());
#else
            // Old Input System
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0.0f)
            {
                MouseWheel(scrollWheel);
            }

            if (Input.GetMouseButtonDown(0))
            {
                preMousePos = Input.mousePosition;
            }

            MouseDrag(Input.mousePosition);
#endif
        }

        protected virtual void MouseWheel(float delta)
        {
            //zoom
            transform.localPosition += new Vector3(0, 0, delta * zoomSpeed * 10);

            if (transform.localPosition.z < -5.0f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -5.0f);
            }
            if (transform.localPosition.z > 5.0f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 5.0f);
            }
        }

        protected virtual void MouseDrag(Vector3 mousePos)
        {

            Vector3 diff = mousePos - preMousePos;

#if ENABLE_INPUT_SYSTEM
            // New Input System
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.isPressed)
            {
                // rotate
                this.transform.parent.gameObject.transform.Rotate(0, diff.x * rotateSpeed, 0);

                // move
                this.transform.position += new Vector3(0, -diff.y * moveSpeed / 10, 0);
                if (this.transform.localPosition.y < -2.0f)
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, -2.0f, this.transform.localPosition.z);
                if (this.transform.localPosition.y > 2.0f)
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, 2.0f, this.transform.localPosition.z);
            }
#else
            // Old Input System
            if (Input.GetMouseButton(0))
            {
                // rotate
                transform.parent.gameObject.transform.Rotate(0, diff.x * rotateSpeed, 0);

                // move
                transform.position += new Vector3(0, -diff.y * moveSpeed / 10, 0);
                if (transform.localPosition.y < -2.0f)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, -2.0f, transform.localPosition.z);
                }

                if (transform.localPosition.y > 2.0f)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, 2.0f, transform.localPosition.z);
                }
            }
#endif

            preMousePos = mousePos;
        }
    }
}
