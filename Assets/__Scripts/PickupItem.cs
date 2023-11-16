using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    //TODO: corrigir colisoes entre itens
    //TODO: corrigir comprotamento enquanto esta sendo carregado


    Vector3 objectPos;
    float distance;

    public float maxDistanceToPickUp = 1;
    public bool canHold = true;
    public GameObject item;
    public GameObject tempParent;
    public bool isHolding = false;
    private Collider[] overlapResults = new Collider[10];
    [SerializeField]
    LayerMask itemLayer;
    CubeMovementController cube;

    [SerializeField]
    float dropDistance = 1;

    private void Awake() {
        cube = FindObjectOfType<CubeMovementController>();
    }

    private void OnDrawGizmos() {
        
        Gizmos.DrawWireSphere(tempParent.transform.position,1);

    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.G)) 
        {

            if (isHolding == false) 
            {
                PickUp();
            } else
            {
                Drop();
            }

        }


        if(isHolding)
        {
            distance = Vector3.Distance(item.transform.position, tempParent.transform.position);
            item.transform.SetParent(tempParent.transform);

            item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            if(distance >= dropDistance){
                Drop();
            }

        }

        
    }

    void PickUp(){

        if (isHolding == false) 
        {
            int numFound = Physics.OverlapSphereNonAlloc(tempParent.transform.position,1, overlapResults,itemLayer);

            if(numFound == 0){
                return;
            }

            item = overlapResults[0].gameObject;
            for (int i = 0; i < numFound; i++) {
                if (item.transform.position.y < overlapResults[i].transform.position.y){
                    item = overlapResults[i].gameObject;
                }
                else
                {
                    if (Vector3.Distance(item.transform.position, transform.position) > Vector3.Distance(overlapResults[i].transform.position, transform.position) ){
                        item = overlapResults[i].gameObject;
                    }
                }
            }
            
            distance = Vector3.Distance(item.transform.position, tempParent.transform.position);

            if (distance <= maxDistanceToPickUp) 
            {
                item.GetComponent<Rigidbody>().useGravity = false;

                StartCoroutine(MoveItem(tempParent.transform.position));
                item.GetComponent<Rigidbody>().detectCollisions = true;
            }
        }
    }

    public void Drop(){
        
        isHolding = false; 
        item.GetComponent<Item>().m_falling = true;
        objectPos = item.transform.position;
        item.GetComponent<Rigidbody>().useGravity = true;
        item.transform.position = objectPos;

    }

    IEnumerator MoveItem(Vector3 position){
        float elapsedTime = 0;

        while (Vector3.Distance(item.transform.position, position) > 0.4f )
        {
            elapsedTime += Time.deltaTime;

            item.transform.position = Vector3.Slerp(item.transform.position, position, elapsedTime/2);
            yield return null;

        }

        isHolding = true;

    }
}
