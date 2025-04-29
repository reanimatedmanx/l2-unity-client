using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BLink
{
    public VisualElement _element;
    public float _blinkInterval;
    private bool _isBlinking = false;


    public void StartBlinking(BufferPanel bufferPanel , VisualElement element , float blinkInterval)
    {
        _element = element;
        _blinkInterval = blinkInterval;
        if (!_isBlinking)
        {
            _isBlinking = true;
            bufferPanel.StartCoroutine(Blink());
        }
    }

    System.Collections.IEnumerator Blink()
    {
        while (_isBlinking)
        {
            _element.style.opacity = 0;
            yield return new WaitForSeconds(_blinkInterval);

            _element.style.opacity = 1;
            yield return new WaitForSeconds(_blinkInterval); 
            
        }
    }

    public void OnDisable()
    {
        _isBlinking = false;
        _element.style.opacity = 1;
    }
}
