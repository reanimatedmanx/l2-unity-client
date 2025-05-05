using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class BLink
{
    public VisualElement _element;
    public float _blinkInterval = 0.1f;
    public float _intervalUse = 30f;
    private bool _isBlinking = true;
    private Dictionary<int , DataCell> _dict;
    private List<int> _filter = new List<int>();
    private float opacityHide = 0.4f;
    private float opacityShow = 1.0f;
    private float durationOpacity = 0.26f;

    public BLink(BufferPanel bufferPanel)
    {
        _dict = new Dictionary<int, DataCell>();
        _filter = new List<int>();
        bufferPanel.StartCoroutine(Blink(_dict , _filter));
    }

    public void StartBlinking(DataCell element)
    {
        int position = element.GetPosition();
        element.GetElement().style.opacity = opacityShow;
        AddOrUpdate(element, position);
    }

    private void AddOrUpdate(DataCell element , int position)
    {
        if (_dict.ContainsKey(position))
        {
            _dict.Add(element.GetPosition(), element);
        }
        else
        {
            _dict[position] = element;
        }

        Debug.Log("Start blink add element ");
    }

    System.Collections.IEnumerator Blink(Dictionary<int, DataCell> _dict , List<int> filter)
    {
        while (_isBlinking)
        {

           // Debug.Log("Start blink " + _dict.Count);
            if (_dict.Count == 0) yield return new WaitForSeconds(_blinkInterval);

            yield return new WaitForSeconds(_blinkInterval);

            float elapsedTime = Time.time;
            FilterElementElseNoTimeOut(elapsedTime, filter , _dict);
            RemoveFilterElements(filter);

            HideElements(_dict , elapsedTime);
            ShowElements(_dict , elapsedTime);
     
        }
    }


    private void HideElements(Dictionary<int, DataCell> _dict , float elapsedTime)
    {
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            DataCell cell = entry.Value;

           // Debug.Log("Start blink add element isHide INTERVAL USE" + cell.GetIntervalUse() + " remaning time " + cell.GetRemainingTime(elapsedTime));

            if (cell.GetRemainingTime(elapsedTime) <= cell.GetIntervalUse() & cell.IsHideElements())
            {
                cell.SetTimeDeltaTime(Time.deltaTime);
                float newOpacity = Mathf.Lerp(opacityShow, opacityHide, cell.GetSmoothTime() / durationOpacity);
               
                cell.GetElement().style.opacity = newOpacity; 
            }
        }
    }

    private void ShowElements(Dictionary<int, DataCell> _dict , float elapsedTime)
    {
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            DataCell cell = entry.Value;
            VisualElement element = cell.GetElement();

            if (element != null)
            {
               // Debug.Log("Start blink add element INTERVAL USE" + cell.GetIntervalUse() + " remaning time " + cell.GetRemainingTime(elapsedTime));
                if (cell.GetRemainingTime(elapsedTime) <= cell.GetIntervalUse() & cell.IsShowElements())
                {
                    cell.SetTimeDeltaTime(Time.deltaTime);
                    float newOpacity = Mathf.Lerp(opacityHide, opacityShow, cell.GetSmoothTime() / durationOpacity);
                   
                    cell.GetElement().style.opacity = newOpacity;
                }
            }
        }
    }

    private void FilterElementElseNoTimeOut(float currentTime , List<int> filter , Dictionary<int, DataCell> _dict)
    {
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            int position = entry.Key;
            DataCell cell = entry.Value;
            float remainingTime = cell.GetRemainingTime(currentTime);
            //Debug.Log("Start blink remaining time " + remainingTime);
            if (remainingTime <= 0)
            {
                filter.Add(position);
            }
        }
    }

    private void RemoveFilterElements(List<int> filter)
    {
        foreach (int position in filter)
        {
            _dict[position].ResetData();
            _dict[position].ShowCell(false);
            _dict.Remove(position);
        }

        filter.Clear();
    }

    public void OnDisable()
    {
        _isBlinking = false;
        _dict.Clear();
    }
}
