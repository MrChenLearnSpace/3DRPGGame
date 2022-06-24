using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


[Serializable]
public class EventVector3 : UnityEvent<Vector3>{

}

public class MouseManager : Singleton<MouseManager>
{
    
    public Texture2D point, doorway, attack, target, arrow;
    // Start is called before the first frame update
    public Camera cam;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    RaycastHit hitinfo;
    protected  override  void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture(){
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out hitinfo)){
            //切换鼠标贴图
           // print("texture");
           switch(hitinfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }
    void MouseControl() {
        if (Input.GetMouseButtonDown(0) && hitinfo.collider !=null) {
            if (hitinfo.collider.gameObject.CompareTag("Ground")) {
                OnMouseClicked?.Invoke(hitinfo.point);
              //  print("call");
            }
            if (hitinfo.collider.gameObject.CompareTag("Enemy")){
                OnEnemyClicked?.Invoke(hitinfo.collider.gameObject);

                //  print("call");
            }
        }

    }
}
