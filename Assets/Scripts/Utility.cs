using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements.Experimental;

namespace Game
{
    public static class Utility
    {
        
        /// <summary>
        /// rotate gameobject transform to be looking twords position
        /// </summary>
        /// <param name="position">position to look at</param>
        /// <param name="transform">calling object's transform</param>
        public static void LookAt(Vector2 position, UnityEngine.Transform transform)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
            worldPos.z = 0f;

            Vector3 direction = (worldPos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
}

