using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace KitEngine.Models
{
    class Camera
    {
        public Vector3 Position;
        public Vector3 Rotation;
        private Vector3 normal = new Vector3(0,0,1);
        public Vector3 Target
        {
            get
            {
                Vector3 _normal = new Vector3(0,0,1);

                //rotate around axis X
                float angleX = (float)(Rotation.X * Math.PI / 180);
                float angleY = (float)(Rotation.Y * Math.PI / 180);
                float angleZ = (float)(Rotation.Z * Math.PI / 180);
                _normal.Y = (float)(_normal.Y * Math.Cos(angleX) + _normal.Z * Math.Sin(angleX));
                _normal.Z = (float)(-_normal.Y * Math.Sin(angleX) + _normal.Z * Math.Cos(angleX));

                //rotate around axis Y
                _normal.X = (float)(_normal.X * Math.Cos(angleY) + _normal.Z * Math.Sin(angleY));
                _normal.Z = (float)(-_normal.X * Math.Sin(angleY) + _normal.Z * Math.Cos(angleY));

                //rotate around axis Z
                _normal.X = (float)(_normal.X * Math.Cos(angleZ) + _normal.Z * Math.Sin(angleZ));
                _normal.Y = (float)(-_normal.Y * Math.Cos(angleZ) + _normal.Z * Math.Sin(angleZ));

                return _normal+Position;
            }
        }

        public Camera()
        {
        }

        public Camera(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
