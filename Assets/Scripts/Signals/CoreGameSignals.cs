using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Signals.Core
{
    public class CoreGameSignals : MonoSingleton<CoreGameSignals>
    {
        public UnityAction<GameState> onChangeGameState = delegate { };
        public UnityAction<GameState> onStateLeave = delegate { };
        public UnityAction<GameState> onStateEnter = delegate { };
    }
}