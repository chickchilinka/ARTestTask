using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
public class ImageTrackingController : MonoBehaviour
{
    private ARTrackedImageManager manager;
    public Dictionary<string, GameObject> spawnedObjs = new Dictionary<string, GameObject>();
    public GameObject[] prefabs;
    public Text CountOnScreen;
    private float count;
    private void Awake() {
        manager = GetComponent<ARTrackedImageManager>();
        foreach(var pref in prefabs) {
            GameObject spawned = Instantiate(pref, Vector3.zero, Quaternion.identity);
            spawned.name = pref.name;
            spawnedObjs.Add(pref.name, spawned);
            spawned.SetActive(false);
        }
    }
    private void Update() {
        count = manager.trackables.count;
        CountOnScreen.text = "Count:" + count;
    }
    private void OnEnable() {
        manager.trackedImagesChanged += ImageChanged;
    }
    private void OnDisable() {
        manager.trackedImagesChanged -= ImageChanged;
    }
    private void ImageChanged(ARTrackedImagesChangedEventArgs args) {
        foreach(var tracked in args.added) {
            UpdateImage(tracked);
        }
        foreach (var tracked in args.updated) {
            UpdateImage(tracked);
        
        }
        foreach (var tracked in args.removed) {
            spawnedObjs[tracked.referenceImage.name].SetActive(false);
        }
    }
    private void UpdateImage(ARTrackedImage im) {
        spawnedObjs[im.referenceImage.name].SetActive(true);
        spawnedObjs[im.referenceImage.name].transform.position = im.transform.position;
        spawnedObjs[im.referenceImage.name].transform.rotation = im.transform.rotation;
    }
}
