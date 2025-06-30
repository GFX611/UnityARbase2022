using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectClass : MonoBehaviour
{
    // Start is called before the first frame update
    public class Insect
        {
        private GameObject insectObject;

        public MonoBehaviour InsectBehaviour;
      

        public GameObject InsectObject
            {
                get { return insectObject; }
                set { insectObject = value; }
        }

        public void SpawnInsect(Insect insect,Transform origin)
        {
            GameObject newInsect = GameObject.Instantiate(insect.InsectObject, origin.position,origin.rotation);
        }
        public Insect(GameObject insectObject)
        {
            this.insectObject = insectObject;
        }

    }



}
