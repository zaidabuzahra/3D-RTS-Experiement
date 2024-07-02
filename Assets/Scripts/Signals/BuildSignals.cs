using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BuildSignals : MonoSingleton<BuildSignals>
{
    public UnityAction<EntityOS> onSelectEntity = delegate { };
}