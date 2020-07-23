using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GoogleARCore;

public class RayCastItemTarget : MonoBehaviour
{
    [SerializeField]
    Camera ar_camera = null;

    //[SerializeField]
    //Text test_test;

    // Update is called once per frame
    void Update()
    {
     

        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = ar_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue);

            //Debug.Log(ray);

            Debug.Log(hits.Length);

            if (hits.Length == 0)
                return;

            foreach (var hit in hits)
            {
                var cm = hit.transform.parent.gameObject.GetComponent<CanvasManager>();
                DecisionWindowManager.Instance.addCanvasManager(cm);

                //test_test.text = hit.transform.parent.gameObject.name + "," + hit.transform.gameObject.name + "," + cm.getItem().image_num +  " touched!" ;
            }

            DecisionWindowManager.Instance.openWindow();

            
        }
    }
}
