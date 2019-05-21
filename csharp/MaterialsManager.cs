using Godot;
using System.Collections.Generic;

namespace Soulslism
{
    public class MaterialsManager
    {
        public enum Material
        {
            ENEMY_BODY
        }

        private Dictionary<Material, Godot.Material> materials = new Dictionary<Material, Godot.Material>();

        static public Godot.Material Get(Material material)
        {
            return ResourceLoader.Load("res://materials/MinionEnemy.tres") as Godot.Material;
        }
    }

}