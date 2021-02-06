using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MoveToScene : MonoBehaviour
{
    public int SceneId;
    // Start is called before the first frame update
    public void Load() {
        SceneManager.LoadScene(SceneId);
    }
}
