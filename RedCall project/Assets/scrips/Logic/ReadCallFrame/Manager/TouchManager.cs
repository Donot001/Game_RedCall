using UnityEngine;
using System.Collections.Generic;

namespace RedCallFrame
{
    public class TouchManager
    {
        private static TouchManager instance;
        private bool useMouseEvent;
        private TouchEvent cachedEvent;
        private List<Camera> cameraList;
        private Object captureObject;
        private RaycastHit cachedHit;
        private Object fallbackObject;
        private int clickRangeThreshold;

        // States maintained between a touch begin and touch end.
        // They must be reset when a touch begins.
        private Object touchedObject;
        private Vector3 touchedPosition;
        private bool needEmitClickEvent;

        public static TouchManager GetInstance()
        {
            if (instance == null)
            {
                instance = new TouchManager();
            }
            return instance;
        }

        public TouchManager()
        {
            useMouseEvent = (Application.platform != RuntimePlatform.Android)
                && (Application.platform != RuntimePlatform.IPhonePlayer)
            ;
            cachedEvent = new TouchEvent("");
            cachedHit = new RaycastHit();
            clickRangeThreshold = 128; // 8^2 + 8^2
        }

        public void Startup(System.Object param = null)
        {

        }
        public void Terminate(System.Object param = null)
        {

        }

        public Object TouchedObject
        {
            get
            {
                return this.touchedObject;
            }
            set
            {
                touchedObject = value;
            }
        }

        public int ClickRangeThreshold
        {
            get
            {
                return this.clickRangeThreshold;
            }
            set
            {
                clickRangeThreshold = value;
            }
        }

        public bool withinClickRange()
        {
            return needEmitClickEvent;
        }

        public void SetCaptureObject(Object captureObject)
        {
            this.captureObject = captureObject;
        }

        public void ReleaseCaptureObject()
        {
            this.captureObject = null;
        }

        public void SetFallbackObject(Object fallbackObject)
        {
            this.fallbackObject = fallbackObject;
        }

        public void ReleaseFallbackObject()
        {
            this.fallbackObject = null;
        }

        public void ResetCameraInformation()
        {
            this.cameraList = null;
        }

        public void ProcessTouchEvents()
        {
            if (Input.touchCount == 0 && !useMouseEvent)
            {
                return;
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        cachedEvent.Type = TouchEvent.TOUCH_EVENT_BEGIN;
                        break;

                    case TouchPhase.Moved:
                        cachedEvent.Type = TouchEvent.TOUCH_EVENT_MOVE;
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        cachedEvent.Type = TouchEvent.TOUCH_EVENT_END;
                        break;

                    default:
                        return;
                }
                cachedEvent.Position = touch.position;
                cachedEvent.DeltaPosition = touch.deltaPosition;
            }
            else
            {
                // We only support mouse input on desktop for test purpose,
                // so the performance is not important
                // thus we can put the code in a seperated method.
                if (!ProcessMouseInput())
                {
                    return;
                }
            }

            FindTargetUnderTouch();
            if (captureObject != null)
            {
                cachedEvent.Target = captureObject;
            }

            bool emitClick = false;
            bool isEnd = false;
            bool canEmitMove = true;
            if (cachedEvent.Type == TouchEvent.TOUCH_EVENT_BEGIN)
            {
                touchedObject = cachedEvent.Target;
                touchedPosition = cachedEvent.Position;
                needEmitClickEvent = true;
            }
            else if (cachedEvent.Type == TouchEvent.TOUCH_EVENT_MOVE)
            {
                // always emit click event if clickRangeThreshold <= 0
                if (clickRangeThreshold > 0)
                {
                    float deltaX = cachedEvent.Position.x - touchedPosition.x;
                    float deltaY = cachedEvent.Position.y - touchedPosition.y;
                    if (deltaX * deltaX + deltaY * deltaY > clickRangeThreshold)
                    {
                        needEmitClickEvent = false;
                    }
                    else
                    {
                        canEmitMove = false;
                    }
                }
            }
            else if (cachedEvent.Type == TouchEvent.TOUCH_EVENT_END)
            {
                isEnd = true;
                if (needEmitClickEvent
                   && (ObjectUtil.ObjectIsSameOrParented(cachedEvent.Target, touchedObject)
                        || ObjectUtil.ObjectIsSameOrParented(touchedObject, cachedEvent.Target)
                    )
                )
                {
                    emitClick = true;
                }
            }

            if (cachedEvent.Target == null)
            {
                cachedEvent.Target = fallbackObject;
            }

            //			if(cachedEvent.Target != null)
            {
                //				Logger.LogMsg("Touched     " + cachedEvent.Target);
                string eventType = cachedEvent.Type;
                if (emitClick)
                {
                    cachedEvent.Type = TouchEvent.TOUCH_EVENT_CLICK;
                    EventDispatcher.GetInstance().Dispatch(cachedEvent);
                }
                if (eventType == TouchEvent.TOUCH_EVENT_MOVE && !canEmitMove)
                {
                }
                else
                {
                    cachedEvent.Type = eventType;
                    EventDispatcher.GetInstance().Dispatch(cachedEvent);
                }
            }

            if (isEnd)
            {
                touchedObject = null;
                // reset the cached event
                cachedEvent.Camera = null;
                cachedEvent.Target = null;
            }
        }

        private void FindTargetUnderTouch()
        {
            CreateCameraList();

            cachedEvent.ObjectUnderTouch = null;

            foreach (Camera camera in cameraList)
            {
                Ray ray = camera.ScreenPointToRay(cachedEvent.Position);
                bool found = Physics.Raycast(ray, out cachedHit, Mathf.Infinity, 1 << camera.gameObject.layer);
                if (found)
                {
                    cachedEvent.ObjectUnderTouch = cachedHit.collider.gameObject;
                    cachedEvent.Target = cachedEvent.ObjectUnderTouch;
                    cachedEvent.Camera = camera;
                    break;
                }
            }
        }

        private int CompareCameraDepth(Camera a, Camera b)
        {
            return (int)(b.depth - a.depth);
        }

        private void CreateCameraList()
        {
            if (cameraList == null)
            {
                cameraList = new List<Camera>();
            }
            else
            {
                cameraList.Clear();
            }

            foreach (Camera camera in Camera.allCameras)
            {
                cameraList.Add(camera);
            }
            cameraList.Sort(CompareCameraDepth);
        }

        private bool ProcessMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                cachedEvent.Type = TouchEvent.TOUCH_EVENT_BEGIN;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                cachedEvent.Type = TouchEvent.TOUCH_EVENT_END;
            }
            else if (Input.GetMouseButton(0))
            {
                if (cachedEvent.Position.x == Input.mousePosition.x
                    && cachedEvent.Position.y == Input.mousePosition.y
                    )
                {
                    return false;
                }
                cachedEvent.Type = TouchEvent.TOUCH_EVENT_MOVE;
                cachedEvent.DeltaPosition = Input.mousePosition;
                cachedEvent.DeltaPosition = cachedEvent.DeltaPosition - cachedEvent.Position;
            }
            else
            {
                return false;
            }
            cachedEvent.Position = Input.mousePosition;
            return true;
        }

    }

}
