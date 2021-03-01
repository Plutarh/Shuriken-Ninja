using BzKovSoft.ObjectSlicerSamples;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Destructable : MonoBehaviour
{

    public MeshRenderer mainRenderObject;
    public GameObject rayFireParent;

    public List<GameObject> pieces = new List<GameObject>();

    NavMeshObstacle meshObstacle;

    private void Awake()
    {
        foreach (Transform piece in rayFireParent.transform)
        {
            pieces.Add(piece.gameObject);
        }

        meshObstacle = GetComponent<NavMeshObstacle>();
        mainRenderObject = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Destruction()
    {
        meshObstacle.carving = false;
        mainRenderObject.enabled = false;
        rayFireParent.SetActive(true);

        rayFireParent.transform.DOScale(0,20f).OnComplete(() =>
        {
            rayFireParent.SetActive(false);
            Destroy(gameObject, 0.1f);
        });
    }

   

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        var hit = other.GetComponent<BzKnife>();

        if (hit != null && hit.weapon.owner.pawnType == Pawn.EPawnType.Player)
        {
            Destruction();
        }
    }
}
