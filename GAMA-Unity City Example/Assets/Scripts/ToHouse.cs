using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToHouse : MonoBehaviour
{
    public int id;

    public Global global;



    public void whenSelected()
    {
        global.sendModificationMessage(id, 0);

    }

}
