using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

namespace CVVTuber
{
    
    public class CameraTouchController : MonoBehaviour
    {
        
        public float rotateSpeed = 1.0f;
        public float moveSpeed = 0.1f;
        public float zoomSpeed = 0.1f;

        void Update ()
        {
            if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) {
                if (EventSystem.current != null) {
                    
                    if (EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId)) {
                        return;
                    }
                }


                if (Input.touchCount == 1) {

                    Touch touch = Input.GetTouch (0);

                    //rotate
                    this.transform.parent.gameObject.transform.Rotate (0, touch.deltaPosition.x * rotateSpeed, 0);

                    //move
                    this.transform.position += new Vector3 (0, -touch.deltaPosition.y * moveSpeed / 10, 0);
                    if (this.transform.localPosition.y < -2.0f) {
                        this.transform.localPosition = new Vector3 (this.transform.localPosition.x, -2.0f, this.transform.localPosition.z);
                    }
                    if (this.transform.localPosition.y > 2.0f) {
                        this.transform.localPosition = new Vector3 (this.transform.localPosition.x, 2.0f, this.transform.localPosition.z);
                    }
                   
                } else if (Input.touchCount == 2) {
              
                    Touch touchZero = Input.GetTouch (0);
                    Touch touchOne = Input.GetTouch (1);

                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    //zoom
                    this.transform.localPosition += new Vector3 (0, 0, deltaMagnitudeDiff * zoomSpeed / 10);

                    if (this.transform.localPosition.z < -5.0f) {
                        this.transform.localPosition = new Vector3 (this.transform.localPosition.x, this.transform.localPosition.y, -5.0f);
                    }
                    if (this.transform.localPosition.z > 0.0f) {
                        this.transform.localPosition = new Vector3 (this.transform.localPosition.x, this.transform.localPosition.y, 0.0f);
                    }
                }
            }
        }
    }
}
