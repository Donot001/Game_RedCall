using UnityEngine;
using System.Collections;

namespace RedCallFrame
{
    public class GameEvent
    {
        private string type;
        private Object target;

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                type = value;
            }
        }

        public Object Target
        {
            get
            {
                return this.target;
            }
            set
            {
                target = value;
            }
        }

        public GameEvent(string type)
        {
            this.type = type;
        }

        public GameEvent(GameEvent other)
        {
            this.type = other.type;
            this.target = other.target;
        }
    }

}
