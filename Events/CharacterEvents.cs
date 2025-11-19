using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public static class CharacterEvents
{
    public static event UnityAction<GameObject, int> characterDamaged;
    public static event UnityAction<GameObject, int> characterHealed;

    public static void OnCharacterDamaged(GameObject obj, int damage)
    {
        characterDamaged?.Invoke(obj, damage);
    }

    public static void OnCharacterHealed(GameObject obj, int heal)
    {
        characterHealed?.Invoke(obj, heal);
    }
}

