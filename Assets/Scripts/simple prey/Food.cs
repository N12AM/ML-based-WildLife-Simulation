using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
       private void OnTriggerEnter(Collider other)
       {
              var agent = other.gameObject.TryGetComponent<SimplePreyBehavior>(out SimplePreyBehavior behavior);

              if (agent)
              {
                     //send found food signal
                     behavior.gameObject.GetComponent<SimplePreyBehavior>().FoundFood();
              } 
       }
}
