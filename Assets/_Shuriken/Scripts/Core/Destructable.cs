using BzKovSoft.ObjectSlicerSamples;
using DG.Tweening;
using RayFire;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(RayfireShatter))]
public class Destructable : MonoBehaviour
{

    public MeshRenderer mainRenderObject;
    public GameObject rayFireParent;
    public RayfireShatter shatter;
    public List<GameObject> shatterObjects = new List<GameObject>();
    public int totalPartsCount;
    public int shatterAmount;

    NavMeshObstacle meshObstacle;

    private void Awake()
    {

        meshObstacle = GetComponent<NavMeshObstacle>();
        mainRenderObject = GetComponent<MeshRenderer>();
        shatter = GetComponent<RayfireShatter>();
        FragmentObject();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FragmentObject()
    {
        //shatter.centerPosition = point;
        shatter.voronoi.centerBias = 0.3f;
        shatter.voronoi.amount = shatterAmount;
        shatter.Fragment();

        rayFireParent = new GameObject("Shatter_Parent");
        rayFireParent.transform.SetParent(this.transform);
        rayFireParent.transform.localPosition = Vector3.zero;

        foreach (var fragment in shatter.fragmentsLast)
        {
            fragment.AddComponent<MeshCollider>().convex = true;
            Rigidbody body = fragment.AddComponent<Rigidbody>();
            body.isKinematic = false;
            body.useGravity = true;
            shatterObjects.Add(fragment);
            fragment.transform.SetParent(rayFireParent.transform);
        }
        if (totalPartsCount == 0) totalPartsCount = shatterObjects.Count;
        rayFireParent.SetActive(false);
        shatter.fragmentsLast[0].transform.parent.parent = transform;
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
