using UnityEngine;
using UnityEngine.EventSystems;

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
                    this.transform.parent.gameObject.transform.Rotate(0, touch.deltaPosition.x * rotateSpeed, 0);

                    //move
                    this.transform.position += new Vector3(0, -touch.deltaPosition.y * moveSpeed / 10, 0);
                    if (this.transform.localPosition.y < -2.0f)
                    {
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x, -2.0f, this.transform.localPosition.z);
                    }
                    if (this.transform.localPosition.y > 2.0f)
                    {
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x, 2.0f, this.transform.localPosition.z);
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
                    this.transform.localPosition += new Vector3(0, 0, deltaMagnitudeDiff * zoomSpeed / 10);

                    if (this.transform.localPosition.z < -5.0f)
                    {
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -5.0f);
                    }
                    if (this.transform.localPosition.z > 5.0f)
                    {
                        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 5.0f);
                    }
                }
            }
        }

        protected virtual void MouseUpdate()
        {
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0.0f)
                MouseWheel(scrollWheel);

            if (Input.GetMouseButtonDown(0))
                preMousePos = Input.mousePosition;

            MouseDrag(Input.mousePosition);
        }

        protected virtual void MouseWheel(float delta)
        {
            //zoom
            this.transform.localPosition += new Vector3(0, 0, delta * zoomSpeed * 10);

            if (this.transform.localPosition.z < -5.0f)
            {
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -5.0f);
            }
            if (this.transform.localPosition.z > 5.0f)
            {
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 5.0f);
            }
        }

        protected virtual void MouseDrag(Vector3 mousePos)
        {

            Vector3 diff = mousePos - preMousePos;

            if (Input.GetMouseButton(0))
            {
                //rotate
                this.transform.parent.gameObject.transform.Rotate(0, diff.x * rotateSpeed, 0);

                //move
                this.transform.position += new Vector3(0, -diff.y * moveSpeed / 10, 0);
                if (this.transform.localPosition.y < -2.0f)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, -2.0f, this.transform.localPosition.z);
                }
                if (this.transform.localPosition.y > 2.0f)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, 2.0f, this.transform.localPosition.z);
                }
            }

            preMousePos = mousePos;
        }
    }
}
