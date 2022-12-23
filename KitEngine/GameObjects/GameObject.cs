using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace KitEngine.GameObjects
{
    public class GameObject : IDisposable
    {
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public List<GameObject> Children { get; private set; }
        public List<Voxel> Mesh { get; set; }

        public GameObject(string name, Transform? parent = null)
        : this(name, Vector3.Zero, Quaternion.FromEulerAngles(Vector3.Zero), parent)
        { }

        public GameObject(string name, Vector3 position, Vector3 rotation, Transform? parent = null)
            : this(name, position, Quaternion.FromEulerAngles(rotation), parent)
        {}

        public GameObject(string name, Vector3 position, Quaternion rotation, Transform? parent = null)
        {
            Mesh = new List<Voxel>();
            Transform = new Transform(parent, position, rotation);
            Transform.GameObject = this;
            Name = name;
            Children = new List<GameObject>();
            Transform.Parent?.GameObject.AddChild(this);
        }

        public void Dispose()
        {
            Mesh.ForEach(voxel => voxel.Dispose());
            Children.ForEach(child => child.Dispose());
        }

        public void Render()
        {
            Mesh.ForEach(voxel => voxel.Render());
            Children.ForEach(child=> child.Render());
        }

        public GameObject Copy()
        {
            GameObject copyObject = new GameObject(Name, Transform.Position, Transform.Rotation);
            copyObject.Mesh.AddRange(Mesh.Select(voxel => new Voxel(voxel.Transform.Position, voxel.Color, copyObject.Transform)));
            copyObject.Children.AddRange(Children.Select(child => child.Copy()));
            return copyObject;
        }

        public void Rotate(Vector3 rotation)
        {
            Rotate(Quaternion.FromEulerAngles(rotation));
        }

        public void Rotate(Quaternion rotation)
        {
            Transform.Rotate(rotation);
            Mesh.ForEach(voxel => voxel.Transform.Rotate(rotation));
            Children.ForEach(child => child.Rotate(rotation));
        }

        public void AddChild(GameObject child)
        {
            child.Transform.Parent = Transform;
            child.Rotate(Transform.Rotation);
            Children.Add(child);
        }
    }
}
