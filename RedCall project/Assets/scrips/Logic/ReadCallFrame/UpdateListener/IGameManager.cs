using UnityEngine;
using System.Collections;

namespace RedCallFrame
{
    public interface IGameManager
    {
        //TODO:以後要添加参数System.Object
        void Startup(System.Object param = null);
        void Terminate(System.Object param = null);
    }
}
