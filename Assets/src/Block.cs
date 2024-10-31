using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    internal class Block : MonoBehaviour
    {
        private BlockType type;

        public BlockType Type { get => type; set { type = value; OnSetType(); } }

        private void OnSetType()
        {
            var renderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;

            switch(type)
            {
                case BlockType.Dirt: renderer.material.color = Color.HSVToRGB(0.166f,0.75f,0.5f); break;
                    case BlockType.Grass: renderer.material.color = Color.green; break;
            }
        }
    }
}
