using System.Collections;
using UnityEngine;

public class MouseHitProvider : MonoBehaviour
{
    private Camera _camera;
    public GameObject MouseHitObject { get; private set; }

    private void Start()
    {
        _camera = Camera.main;
        StartCoroutine(GetMouseHit());
    }
    
    private IEnumerator GetMouseHit()
    {
        while (true)
        {
            if (Camera.main == null) yield return new WaitForFixedUpdate();
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit))
            {
                MouseHitObject = rayHit.collider.gameObject;
            }
            else
            {
                MouseHitObject = null;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}