using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToEmptySpace : MonoBehaviour
{
    public int id;

    public Global global;

  
    public void whenSelected()
    {
        global.sendModificationMessage(id, 2);

    }
  
   
}
