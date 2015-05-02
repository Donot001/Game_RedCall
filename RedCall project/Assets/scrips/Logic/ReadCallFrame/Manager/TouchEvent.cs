using UnityEngine;
using System.Collections;

namespace RedCallFrame
{
    public class TouchEvent : GameEvent
    {
        // The event name starts with special parts, then common parts,
        // this will make the dictionary look up a little faster.
        public const string TOUCH_EVENT_BEGIN = "begin.touch.event";
        public const string TOUCH_EVENT_MOVE = "move.touch.event";
        public const string TOUCH_EVENT_END = "end.touch.event";
        public const string TOUCH_EVENT_CLICK = "click.touch.event";

        private Vector3 position;
        private Vector3 deltaPosition;
        private Camera camera;
        private GameObject objectUnderTouch;

        public Vector3 DeltaPosition
        {
            get
            {
                return this.deltaPosition;
            }
            set
            {
                deltaPosition = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                position = value;
            }
        }

        public Camera Camera
        {
            get
            {
                return this.camera;
            }
            set
            {
                camera = value;
            }
        }

        public GameObject ObjectUnderTouch
        {
            get
            {
                return objectUnderTouch;
            }
            set
            {
                objectUnderTouch = value;
            }
        }

        public TouchEvent(string type)
            : base(type)
        {
            this.position = new Vector3();
            this.deltaPosition = new Vector3();
        }

        public TouchEvent(TouchEvent other)
            : base(other)
        {
            this.position = other.position;
            this.deltaPosition = other.deltaPosition;
            this.camera = other.camera;
        }

    }

}
