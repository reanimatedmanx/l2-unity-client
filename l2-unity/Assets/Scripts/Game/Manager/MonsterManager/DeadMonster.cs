using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class DeadMonster : MonoBehaviour
{
    //fly setting
    private bool _useAntiGravity = false;
    public float speed = 0.000001f; // Скорость движения вверх
    private float zeroPos = 0;
    private bool _isRefresh = false;


    //opacity setting
    private Renderer[] _monsterRenderer;
    private Color originalColor;
    private float currentPos = 0;

    //remove setting
    private World _world;
    private int _monster_id;

    void Start()
    {

    }


  
    void Update()
    {
        if (_useAntiGravity)
        {
            

            // Плавно перемещаем персонажа к целевой высоте
            //float newY = Mathf.Lerp(transform.position.y, targetHeight, speed * Time.deltaTime);
            //float newY2 = transform.position.y - transform.position.y;
            float newY3 = transform.position.y;
            float newY4 = newY3 * 2;
            float zeroPosition = newY3 - newY4;
            currentPos = zeroPosition - zeroPosition * 2;
            zeroPos = currentPos;
            currentPos = currentPos + 0.5f;
            float lerp = Mathf.Lerp(transform.position.y, 0.5f, speed * Time.deltaTime);
            _monsterRenderer = gameObject.GetComponentsInChildren<Renderer>();
            _isRefresh = true; 
            _useAntiGravity = false;
            NameplatesManager.Instance.Remove(_monster_id);
        }
        else
        {
            if (_isRefresh)
            {
               
                
                
                //if(transform.position.y > 0)
                //{

                //transform.position = new Vector3(transform.position.x, 5, transform.position.z);
                //   IsRefresh = false;
                //}
                if (transform.position.y <= 3)
                {
                    originalColor = GetOriginalColor(_monsterRenderer);
                    if(originalColor.a > 0.1)
                    {
                        float lerp = Mathf.Lerp(transform.position.y, currentPos, speed * Time.deltaTime);
                        float alpha = Mathf.Lerp(originalColor.a, 0f, 1f * Time.deltaTime);
                        //Debug.Log("NEWWWWWWWW currentPos refresh realPos" + transform.position.y);
                       // Debug.Log("NEWWWWWWWW currentPos refresh y currentPos" + currentPos);
                        //Debug.Log("NEWWWWWWWW currentPos refresh y zeroPos" + zeroPos);
                        //Debug.Log("NEWWWWWWWW currentPos refresh y lerp" + lerp);
                       // Debug.Log("NEWWWWWWWW currentPos refresh y opacity " + alpha);
                        //Debug.Log("NEWWWWWWWW currentPos refresh y original color" + originalColor.a);
                        //transform.position = new Vector3(transform.position.x, -65, transform.position.z);
                        transform.position = new Vector3(transform.position.x, lerp, transform.position.z);
                        SetOpacity(_monsterRenderer, alpha);
                    }
                    else
                    {
                        if (_world != null) _world.RemoveObject(_monster_id);
                        _isRefresh = false;
                    }
                   
                }

            }
            
        }
    }

    private void SetOpacity(Renderer[] monsterRenderer , float opacity)
    {
        if(monsterRenderer[0] != null)
        {
            Material material = monsterRenderer[0].material;
            if (material != null)
            {
                // need change setting material manual _Surface = 1, RenderType = Transperenty
                material.SetFloat("_Surface", 1); // 1 = Transparent, 0 = Opaque
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                // Изменяем цвет и устанавливаем прозрачность
                Color color = material.color;
                color.a = opacity; 
                material.color = color;
            }
        }
      
    }

    public Color GetOriginalColor(Renderer[] monsterRenderer)
    {
        if (monsterRenderer[0] != null)
        {
            Material material = monsterRenderer[0].material;
            if (material != null)
            {
                return material.color;
            }
        }
        return new Color();
    }

    public void OnDeadAntiGravity(int monster_id , bool useAntiGravity , World world)
    {

        _useAntiGravity = useAntiGravity;
        _world = world;
        _monster_id = monster_id;
    }
}
