using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Pattern")]
public class BossPatternScriptableObject : ScriptableObject
{
    public string patternName;
    // public AnimatorOverrideController[] animatorOv;
    public AnimationClip[] animationClips;
    public int priority;
    public float patternCooldown;
    public float fieldOfView;
    public float viewDistance;
    public bool fly;
    public Boss.BossState nextState;
    public string componentName;
    // public GameObject patternPrefab;
    public BossPatternAction Init(Transform patternContainer)
    {
        // var container = Instantiate(patternPrefab, patternContainer);
        // container.name = patternName;

        var container = new GameObject(patternName);
        container.transform.SetParent(patternContainer);
        if (!AddMyComponent(container, componentName)) return null;

        var patternAction = container.GetComponent<BossPatternAction>();
        if (patternAction is null)
        {
            Debug.LogError("패턴 액션 컴포넌트 생성 오류");
            return null;
        }

        patternAction.Init(animationClips, priority, patternCooldown, fieldOfView, viewDistance, fly, nextState);

        
        return patternAction;
    }

    private bool AddMyComponent(GameObject obj, string componentName)
    {
        if (!string.IsNullOrEmpty(componentName))
        {
            // 문자열로부터 타입 가져오기
            System.Type componentType = System.Type.GetType(componentName);

            if (componentType != null && typeof(Component).IsAssignableFrom(componentType))
            {
                // 해당 타입을 가진 컴포넌트 추가
                Component newComponent = obj.AddComponent(componentType);
                if (newComponent != null)
                {
                    Debug.Log("Added component: " + newComponent.GetType().FullName);
                    return true;
                } else Debug.LogWarning("Failed to add component: " + componentName);
            } else Debug.LogWarning("Invalid component type name: " + componentName);
        }

        return false;
    }
}
