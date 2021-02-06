using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
public class PlacementManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool EditMode = false;
    public Camera cam;
    public LayerMask PlaneMask;
    public LayerMask GizmoMask;
    public LayerMask BoxMask;
    public GameObject PrefabBox;
    public List<GameObject> placedPrefabs;
    private Transform Editing;
    public GameObject Gizmo;
    private Vector3 LastPointerPos;
    public enum GizmoState
    {
        XPos,YPos,ZPos,XRot,YRot,ZRot,XScale,YScale,ZScale
    }
    public GizmoState curState;
    private ARRaycastManager ARRaycaster;
    private List<ARRaycastHit> ARRHit=new List<ARRaycastHit>();
    private void Start() {
        ARRaycaster = GetComponent<ARRaycastManager>();
    }
    private void Place(ARRaycastHit hit) {
        Gizmo.SetActive(false);
        GameObject spawned = Instantiate(PrefabBox, hit.pose.position + hit.pose.up* 0.05f, Quaternion.identity);
        placedPrefabs.Add(spawned);
    }
   private void GizmoPress(Vector3 pos,RaycastHit hit) {
        EditMode = true;
        LastPointerPos = pos;
        curState = hit.collider.GetComponent<GizmoController>().ControlType;
    }
    private void EditCube(Transform t){
        Editing = t;
        Gizmo.SetActive(true);
        Gizmo.transform.position = t.position;
    }
    void Raycast(Vector3 ScreenPos) {
        Ray r = cam.ScreenPointToRay(ScreenPos);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 1000, GizmoMask)) {
            GizmoPress(ScreenPos,hit);
        }
        else if (Physics.Raycast(r, out hit, 1000, BoxMask)) {
            EditCube(hit.collider.transform);
        }
        else if (ARRaycaster.Raycast(ScreenPos,  ARRHit, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)) {
            Place(ARRHit[0]);
        }
    }
    void ChangeTransform(Vector3 pos) {
        Vector3 delta = pos - LastPointerPos;
        delta = cam.transform.InverseTransformDirection(delta);
        switch (curState) {
            case GizmoState.XPos:
                Editing.position += new Vector3(delta.x/200f, 0, 0);
                Gizmo.transform.position += new Vector3(delta.x / 200f, 0, 0);
                break;
            case GizmoState.YPos:
                Editing.position += new Vector3(0, delta.y / 200f, 0);
                Gizmo.transform.position += new Vector3(0, delta.y / 200f, 0);
                break;
            case GizmoState.ZPos:
                Editing.position += new Vector3(0, 0, delta.x / 200f);
                Gizmo.transform.position += new Vector3(0, 0, delta.x / 200f);
                break;
            case GizmoState.XRot:
                Editing.Rotate(delta.y / 2f,0 , 0, Space.World);
                break;
            case GizmoState.YRot:
                Editing.Rotate(0, -delta.x / 2f, 0, Space.World); 
                break;
            case GizmoState.ZRot:
                Editing.Rotate(0, 0, delta.y / 2f, Space.World);
                break;
            case GizmoState.XScale:
                Editing.localScale += new Vector3(delta.x / 200f, 0, 0);  
                break;
            case GizmoState.YScale:
                Editing.localScale+= new Vector3(0, delta.y / 200f, 0);
                Editing.position += new Vector3(0, delta.y / 400f, 0);
                break;
            case GizmoState.ZScale:
                Editing.localScale += new Vector3(0, 0, -delta.x / 200f);
                break;
        }
        LastPointerPos = pos;
    }
    public void Update() {
        if (EditMode) {
#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0)) {
                EditMode = false;
            }
            else {
                ChangeTransform(Input.mousePosition);
            }
#else
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) {
                EditMode = false;
            }else{
            ChangeTransform( Input.touches[0].position);
            }
#endif
        }
        else {
#if UNITY_EDITOR
            if (!EventSystem.current.IsPointerOverGameObject()) {
                if (Input.GetMouseButtonDown(0)) {

                    Raycast(Input.mousePosition) ;

                }
            }
#else
   if (!EventSystem.current.IsPointerOverGameObject()) {
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) {
                    Raycast(Input.touches[0].position);
                }
            }

#endif
        }
    }
}
