using System;
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
            if (_camera == null) yield return new WaitForFixedUpdate();
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Single.MaxValue, LayerMask.GetMask("Tile")))
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