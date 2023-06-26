using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToOffice : MonoBehaviour
{
    public int id;

    public Global global;

   
    public void whenSelected()
    {
        global.sendModificationMessage(id, 1);

    }

}
 